using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Meal
{

    public string Name;

    public List<Allergens> Allergens;
    public Countries Country;
    public MealTypes MealType;

    public GameObject MealPrefab;

    public Meal(string name, List<Allergens> allergens, Countries country, MealTypes mealType)
    {
        Name = name;
        Allergens = allergens;
        Country = country;
        MealType = mealType;
    }
}
