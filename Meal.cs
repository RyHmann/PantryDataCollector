using System;
using System.Collections.Generic;
using System.Text;

namespace PantryDataCollector
{
    class Meal
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public List<string> Ingredients { get; set; }
        public string URL { get; set; }
        public string Thumbnail { get; set; }

        public Meal()
        {
            Ingredients = new List<string>();
        }
    }
}
