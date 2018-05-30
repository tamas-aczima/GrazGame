using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Queue : NetworkBehaviour {

    [SerializeField] private int queueLength;
    [SerializeField] private int queuePositionDistance;
    private Transform queueStartPosition;
    private Transform[] queuePositions;
    [SerializeField] private Transform queueLeavePosition;
    private int rear;

    private enum QueueOrientation
    {
        x,
        z
    }
    [SerializeField] private QueueOrientation queueOrientation;
    [SerializeField] private GameObject[] customerPrefabs;
    [SerializeField] private int maxCustomers;
    private int customersSpawned;
    private List<GameObject> customers = new List<GameObject>();
    [SerializeField] private float customerSpawnTime;
    private float customerSpawnTimer;
    
    public override void OnStartServer()
    {
        queueStartPosition = GetComponent<Transform>();
        GenerateQueue();
    }

    private void Update()
    {
        if (!isServer) return;
        SpawnCustomer();
        ManageQueue();
    }

    private void GenerateQueue()
    {
        queuePositions = new Transform[queueLength];
        queuePositions[0] = queueStartPosition;

        for (int i = 1; i < queueLength; i++)
        {
            GameObject queuePos = new GameObject("QueuePosition" + (i + 1));
            queuePos.transform.parent = queueStartPosition;
            queuePositions[i] = queuePos.transform;

            switch (queueOrientation)
            {
                case QueueOrientation.x:
                    queuePositions[i].position = queuePositions[i - 1].position - new Vector3(queuePositionDistance, 0, 0);
                    break;
                case QueueOrientation.z:
                    queuePositions[i].position = queuePositions[i - 1].position - new Vector3(0, 0, queuePositionDistance);
                    break;
            }
        }
    }

    private void SpawnCustomer()
    {
        if (customersSpawned >= maxCustomers || customers.Count >= queueLength) return;

        customerSpawnTimer += Time.deltaTime;
        if (customerSpawnTimer < customerSpawnTime) return;

        customerSpawnTimer = 0f;
        GameObject customer = null;
        switch (queueOrientation)
        {
            case QueueOrientation.x:
                customer = Instantiate(customerPrefabs[Random.Range(0, customerPrefabs.Length)], queuePositions[queuePositions.Length - 1].position - new Vector3(3, 0, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
                break;
            case QueueOrientation.z:
                customer = Instantiate(customerPrefabs[Random.Range(0, customerPrefabs.Length)], queuePositions[queuePositions.Length - 1].position - new Vector3(0, 0, 3), Quaternion.identity);
                break;
        }

        CustomerController cController = customer.GetComponent<CustomerController>();
        cController.numberOfAllergens = Random.Range(0, cController.maxAllergens);
        for (int i = 0; i < cController.numberOfAllergens; i++)
        {
            //cController.allergens.Add(GetRandomEnum<Allergens>());
            cController.allergens.Add(Random.Range(0, System.Enum.GetNames(typeof(Allergens)).Length));
        }
        cController.country = Random.Range(0, System.Enum.GetNames(typeof(Countries)).Length);
        cController.mealType = Random.Range(0, System.Enum.GetNames(typeof(MealTypes)).Length);

        NetworkServer.Spawn(customer);
        customers.Add(customer);
        customersSpawned++;
        customer.GetComponent<CustomerController>().TargetPos = queuePositions[rear].position;
        customer.GetComponent<CustomerController>().Queue = this;
        rear++;
    }

    private void ManageQueue()
    {
        if (customers.Count == 0) return;

        CustomerController firstCustomer = customers[0].GetComponent<CustomerController>();
        if (!firstCustomer.IsFirst)
        {
            firstCustomer.IsFirst = true;
        }

        if (firstCustomer.IsServed && !firstCustomer.IsDead)
        {
            firstCustomer.IsFirst = false;
            customers.RemoveAt(0);
            rear = 0;

            for (int i = 0; i < customers.Count; i++)
            {
                customers[i].GetComponent<CustomerController>().TargetPos = queuePositions[rear].position;
                rear++;
            }
        }
    }

    public Transform QueueLeavePosition
    {
        get { return queueLeavePosition; }
    }

    static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(Random.Range(0, A.Length));
        return V;
    }
}
