using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class FoodGenerationScript : MonoBehaviour
    {
        [SerializeField] private GameObject chef;
        private GameObject target;
        public System.Action alertChef;

        public GameObject Target
        {
            get { return target; }
            set { target = value; }
        }

        public Restaurant Restaurant;

        [HideInInspector]
        public GameObject MealRight, MealLeft;

        [HideInInspector]
        public Dictionary<string, Meal> currentMeals;

        public GameObject MealInfoPrefab;

        [HideInInspector]
        public GameObject InfoMealRight, InfoMealLeft;

        public void Start()
        {
            MealRight = null;
            MealLeft = null;

            currentMeals = new Dictionary<string, Meal>(2);

            alertChef += AlertChef;
        }

        private void Awake()
        {
        }

        public void Update()
        {
            DoMealGenerationThings();
        }

        private void AlertChef()
        {
            chef.GetComponent<ChefController>().ShouldChase = true;
            chef.GetComponent<ChefController>().Target = target;
        }

        public void DoMealGenerationThings()
        {
            GenerateMeals();
        }

        private int rightMealIndex;
        private int leftMealIndex;

        private void GenerateMeals()
        {
            if (MealRight == null)
            {
                int prefabIndex = Random.Range(0, Restaurant.Meals.Count);
                while (prefabIndex == leftMealIndex)
                {
                    prefabIndex = Random.Range(0, Restaurant.Meals.Count);
                }
                rightMealIndex = prefabIndex;
                currentMeals.Add("MealRight", Restaurant.Meals[prefabIndex]);
                MealRight = currentMeals["MealRight"].MealPrefab;

                //////////////// not good:
                var rightPosition = gameObject.transform.position + gameObject.transform.forward * 2.5f - gameObject.transform.right;

                MealRight = Instantiate(MealRight, rightPosition, MealRight.transform.rotation);
                MealRight.transform.SetParent(gameObject.transform, true);
                MealRight.name = "MealRight";

                InfoMealRight = Instantiate(MealInfoPrefab, MealRight.transform);
                (InfoMealRight.transform.Find("Canvas").Find("Text").GetComponent<Text>()).text = currentMeals["MealRight"].ToString();
                InfoMealRight.transform.position = new Vector3(MealRight.transform.position.x, MealRight.transform.position.y + 1.2f, MealRight.transform.position.z - 1f);
            }
            if (MealLeft == null)
            {
                int prefabIndex = Random.Range(0, Restaurant.Meals.Count);
                while (prefabIndex == rightMealIndex)
                {
                    prefabIndex = Random.Range(0, Restaurant.Meals.Count);
                }
                leftMealIndex = prefabIndex;
                currentMeals.Add("MealLeft", Restaurant.Meals[prefabIndex]);
                MealLeft = currentMeals["MealLeft"].MealPrefab;

                var leftPosition = gameObject.transform.position + gameObject.transform.forward * 2.5f + gameObject.transform.right;

                MealLeft = Instantiate(MealLeft, leftPosition, MealLeft.transform.rotation);
                MealLeft.transform.SetParent(gameObject.transform, true);
                MealLeft.name = "MealLeft";

                InfoMealLeft = Instantiate(MealInfoPrefab, MealLeft.transform);
                (InfoMealLeft.transform.Find("Canvas").Find("Text").GetComponent<Text>()).text = currentMeals["MealLeft"].ToString();
                InfoMealLeft.transform.position = new Vector3(MealLeft.transform.position.x, MealLeft.transform.position.y + 1.2f, MealLeft.transform.position.z - 1f);
            }
        }
    }
}
