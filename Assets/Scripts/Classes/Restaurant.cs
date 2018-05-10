using Assets.Scripts.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
