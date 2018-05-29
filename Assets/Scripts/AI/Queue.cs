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
    [SyncVar] private int rear;

    private enum QueueOrientation
    {
        x,
        z
    }
    [SerializeField] private QueueOrientation queueOrientation;
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private int maxCustomers;
    private int customersSpawned;
    private List<GameObject> customers = new List<GameObject>();
    [SerializeField] private float customerSpawnTime;
    private float customerSpawnTimer;

    private void Start()
    {
        queueStartPosition = GetComponent<Transform>();
        GenerateQueue();
    }

    private void Update()
    {
        if (!isServer) return;
        SpawnCustomer();
        RpcManageQueue();
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
                customer = Instantiate(customerPrefab, queuePositions[queuePositions.Length - 1].position - new Vector3(3, 0, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
                NetworkServer.Spawn(customer);
                break;
            case QueueOrientation.z:
                customer = Instantiate(customerPrefab, queuePositions[queuePositions.Length - 1].position - new Vector3(0, 0, 3), Quaternion.identity);
                NetworkServer.Spawn(customer);
                break;
        }

        customers.Add(customer);
        customersSpawned++;
        customer.GetComponent<CustomerController>().TargetPos = queuePositions[rear].position;
        customer.GetComponent<CustomerController>().Queue = this;
        rear++;
    }

    [ClientRpc]
    private void RpcManageQueue()
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
}
