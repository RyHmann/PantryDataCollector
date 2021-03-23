using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PantryDataCollector

{
    public class MealIngredient
    {
        public Meal Meal { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}
