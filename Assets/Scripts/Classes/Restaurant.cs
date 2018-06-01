using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class Restaurant
    {

        public string Name;
        public List<Meal> Meals;

        public Restaurant(string name, List<Meal> meals)
        {
            Name = name;
            Meals = meals;
        }
    }
}
