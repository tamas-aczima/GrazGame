using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Meal
{

    public string Name;

    public List<Allergens> Allergens;

    public GameObject MealPrefab;

    public Meal(string name, List<Allergens> allergens)
    {
        Name = name;
        Allergens = allergens;
    }
}
