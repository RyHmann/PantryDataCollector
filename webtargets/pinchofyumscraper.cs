using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PantryDataCollector.webtargets
{
    public class PinchOfYumScraper
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Thumbnail { get; set; }

        public PinchOfYumScraper()
        {
            this.Title = "//h1[@class='entry-title']";
            this.Description = "//div[@class='tasty-recipes-description-body']/p";
            this.Instructions = "//div[@class='tasty-recipes-instructions']/div/ol/li";
            this.Ingredients = new List<string> { "//div[@class='tasty-recipes-ingredients']", ".//ul/li/strong" };
            this.Thumbnail = new List<string> { "//div[@class='entry-content']", ".//a/img" };
        }
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
