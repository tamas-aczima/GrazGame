using Assets.Scripts.Classes;
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

        private void GenerateMeals()
        {
            if (MealRight == null)
            {
                int prefabIndex = Random.Range(0, Restaurant.Meals.Count);
                currentMeals.Add("MealRight", Restaurant.Meals[prefabIndex]);
                MealRight = currentMeals["MealRight"].MealPrefab;
            }
            if (MealLeft == null)
            {
                int prefabIndex = Random.Range(0, Restaurant.Meals.Count);
                currentMeals.Add("MealLeft", Restaurant.Meals[prefabIndex]);
                MealLeft = currentMeals["MealLeft"].MealPrefab;
            }
            MealsOffered = true;
        }

        private void OfferMeals()
        {
            var leftPosition = new Vector3(gameObject.transform.localPosition.x + gameObject.transform.localScale.x * 0.15f, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z + 2.2f);
            MealLeft = Instantiate(MealLeft, leftPosition, MealLeft.transform.rotation);
            MealLeft.transform.SetParent(gameObject.transform, true);
            MealLeft.name = "MealLeft";


            var rightPosition = new Vector3(gameObject.transform.localPosition.x - gameObject.transform.localScale.x * 0.15f, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z + 2.2f);
            MealRight = Instantiate(MealRight, rightPosition, MealRight.transform.rotation);
            MealRight.transform.SetParent(gameObject.transform, true);
            MealRight.name = "MealRight";

        }

        public void DoMealGenerationThings()
        {
            GenerateMeals();
            OfferMeals();
        }

    }
}
