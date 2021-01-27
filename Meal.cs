using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PantryDataCollector

{
    public class Meal
    {
        public int MealId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string URL { get; set; }
        public string Thumbnail { get; set; }
        public bool Editable { get; set; }

        public List<MealIngredient> MealIngredients { get; set; }


        public Meal()
        {
            MealIngredients = new List<MealIngredient>();
        }

    }
}
