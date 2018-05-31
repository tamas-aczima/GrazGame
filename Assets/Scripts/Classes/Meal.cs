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
        string returnString = string.Empty;
        returnString += "<b>";
        returnString += Name;
        returnString += "</b>";
        returnString += "\n\n";
        returnString += "Allergens:\n";


        if (Allergens.Count > 0)
        {
            returnString += "<i>";

            foreach (var allergen in Allergens)
            {
                returnString += Enum.GetName(typeof(Allergens), allergen);
                returnString += ", ";
            }

            returnString = returnString.Substring(0, returnString.Length - 2);
            returnString += "</i>";
        }

        return returnString;
    }
}
