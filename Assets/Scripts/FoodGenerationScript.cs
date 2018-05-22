using Assets.Scripts.Classes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class FoodGenerationScript : MonoBehaviour
    {
        public Restaurant Restaurant;

        public bool MealsOffered;

        [HideInInspector]
        public GameObject MealRight, MealLeft;

        [HideInInspector]
        public Dictionary<string, Meal> currentMeals;

        private int nonChoosenMealIndex;

        public void Start()
        {
            nonChoosenMealIndex = -1;
            MealRight = null;
            MealLeft = null;
            MealsOffered = false;
            currentMeals = new Dictionary<string, Meal>(2);
        }

        public void Update()
        {
            if (MealsOffered)
            {
                if (MealRight != null) MealRight.SetActive(true);
                if (MealLeft != null) MealLeft.SetActive(true);
            }
            else
            {
                if (MealRight != null) MealRight.SetActive(false);
                if (MealLeft != null) MealLeft.SetActive(false);
            }

        }

        public void DoMealGenerationThings()
        {
            GenerateMeals();
            OfferMeals();
        }

        private void GenerateMeals()
        {
            if (MealRight == null)
            {
                int prefabIndex = UnityEngine.Random.Range(0, Restaurant.Meals.Count);
                currentMeals.Add("MealRight", Restaurant.Meals[prefabIndex]);
                MealRight = currentMeals["MealRight"].MealPrefab;

                var rightPosition = gameObject.transform.position + gameObject.transform.forward * 2f - gameObject.transform.right;

                MealRight = Instantiate(MealRight, rightPosition, MealRight.transform.rotation);
                MealRight.transform.SetParent(gameObject.transform, true);
                MealRight.name = "MealRight";
            }
            if (MealLeft == null)
            {
                int prefabIndex = UnityEngine.Random.Range(0, Restaurant.Meals.Count);
                currentMeals.Add("MealLeft", Restaurant.Meals[prefabIndex]);
                MealLeft = currentMeals["MealLeft"].MealPrefab;

                var leftPosition = gameObject.transform.position + gameObject.transform.forward * 2f + gameObject.transform.right;

                MealLeft = Instantiate(MealLeft, leftPosition, MealLeft.transform.rotation);
                MealLeft.transform.SetParent(gameObject.transform, true);
                MealLeft.name = "MealLeft";
            }
            MealsOffered = true;
        }

        private void OfferMeals()
        {
            if (MealLeft == null)
            {


            }

            if (MealRight == null)
            {

            }
        }


        internal void HideOfferedMeals()
        {
            MealsOffered = false;
        }
    }
}
