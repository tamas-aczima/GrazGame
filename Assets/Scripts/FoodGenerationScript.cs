using Assets.Scripts.Classes;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class FoodGenerationScript : MonoBehaviour
    {
        public Restaurant Restaurant;

        [HideInInspector]
        public GameObject MealRight, MealLeft;

        [HideInInspector]
        public Dictionary<string, Meal> currentMeals;

        public GameObject RightMealInfoPrefab, LeftMealInfoPrefab;

        [HideInInspector]
        public GameObject InfoMealRight, InfoMealLeft;

        public void Start()
        {
            MealRight = null;
            MealLeft = null;

            currentMeals = new Dictionary<string, Meal>(2);
        }

        private void Awake()
        {
        }

        public void Update()
        {
            DoMealGenerationThings();
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
                var rightPosition = gameObject.transform.position + gameObject.transform.forward * 2f - gameObject.transform.right;

                MealRight = Instantiate(MealRight, rightPosition, MealRight.transform.rotation);
                InfoMealRight = Instantiate(RightMealInfoPrefab, MealRight.transform);

                MealRight.transform.SetParent(gameObject.transform, true);
                MealRight.name = "MealRight";
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

                var leftPosition = gameObject.transform.position + gameObject.transform.forward * 2f + gameObject.transform.right;

                MealLeft = Instantiate(MealLeft, leftPosition, MealLeft.transform.rotation);
                InfoMealLeft = Instantiate(LeftMealInfoPrefab, MealLeft.transform);
                MealLeft.transform.SetParent(gameObject.transform, true);
                MealLeft.name = "MealLeft";
            }
        }
    }
}
