using System;
using System.Collections.Generic;

namespace Assets.Scripts.Classes
{
    [Serializable]
    public class Meal
    {

        public string Name;

        public List<Allergens> Allergens;

        public Meal(string name, List<Allergens> allergens)
        {
            Name = name;
            Allergens = allergens;
        }
    }


}
