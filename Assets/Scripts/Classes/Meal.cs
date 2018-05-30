using System;
using System.Collections.Generic;
using System.Text;
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


    public override string ToString()
    {
        var str = new StringBuilder();
        str.Append("Meal: ");
        str.Append(Name);
        str.Append("\n");
        str.Append("Allergens: ");

        foreach (var allergen in Allergens)
        {
            str.Append(Enum.GetName(typeof(Allergens), allergen));
        }

        return str.ToString();
    }
}
