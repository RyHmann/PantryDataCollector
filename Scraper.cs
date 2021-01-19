using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PantryDataCollector
{
    public class Scraper
    {
        public bool RecipePageCheck(HtmlDocument webpage)
        {
            //Confirm if this is a recipe page
            var recipePageCheck = webpage.DocumentNode.SelectSingleNode("//header[@class='entry-header']/p/a/text()");
            var confirmText = "Jump To Recipe";
            if (recipePageCheck != null)
            {
                var headerText = recipePageCheck.InnerText;
                if (String.Equals(headerText, confirmText, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
