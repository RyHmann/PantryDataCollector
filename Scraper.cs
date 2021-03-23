using HtmlAgilityPack;
using PantryDataCollector.webtargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PantryDataCollector
{
    public class Scraper
    {
        private HtmlWeb _web { get; set; }
        private PinchOfYumScraper _webTarget { get; set; }
        private string _url { get; set; }

        public Scraper(string url)
        {
            _web = new HtmlWeb();
            _webTarget = new PinchOfYumScraper();
            _url = url;
            StartScrape();
        }

        public void StartScrape()
        {
            HtmlDocument webData = _web.Load(_url);
            if (RecipePageCheck(webData))
            {

            }
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
