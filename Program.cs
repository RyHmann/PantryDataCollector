using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Serilog;
using Abot2.Poco;
using Abot2.Crawler;
using Abot2.Core;

namespace PantryDataCollector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Demo starting up!");

            await DemoSimpleCrawler();
            await DemoSinglePageRequest();
        }

        private static async Task DemoSimpleCrawler()
        {
            var config = new CrawlConfiguration
            {
                MaxPagesToCrawl = 100, //Only crawl 10 pages
                MinCrawlDelayPerDomainMilliSeconds = 10000, //Wait this many millisecs between requests,
                MaxCrawlDepth = 1
            };
            var crawler = new PoliteWebCrawler(config);

            crawler.PageCrawlCompleted += PageCrawlCompleted;//Several events available...
            var crawlResult = await crawler.CrawlAsync(new Uri("https://xxx/pg10"));
        }

        private static async Task DemoSinglePageRequest()
        {
            var pageRequester = new PageRequester(new CrawlConfiguration(), new WebContentExtractor());

            var crawledPage = await pageRequester.MakeRequestAsync(new Uri("http://google.com"));

            Log.Logger.Information("{result}", new
            {
                url = crawledPage.Uri,
                status = Convert.ToInt32(crawledPage.HttpResponseMessage.StatusCode)
            });
        }

        private static void PageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            var url = crawledPage.Uri.ToString();
            ScrapeData(url);
        }

        private static void ScrapeData(string url)
        {
            Scraper webScraper = new Scraper();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument webData = web.Load(url);

            if (webScraper.RecipePageCheck(webData))
            {
                Console.WriteLine("Scraping data...");
                // Scrape Data
                //Title
                var recipeName = webData.DocumentNode.SelectSingleNode("//h1[@class='entry-title']");

                //Description
                var recipeDescription = webData.DocumentNode.SelectSingleNode("//div[@class='tasty-recipes-description-body']/p");

                //Instructions
                var recipeInstructions = webData.DocumentNode.SelectNodes("//div[@class='tasty-recipes-instructions']/div/ol/li");

                //Ingredients
                var recipeIngredients = webData.DocumentNode
                    .SelectSingleNode("//div[@class='tasty-recipes-ingredients']")
                    .SelectNodes(".//ul/li/strong");

                //Thumbnail
                var recipeThumbnail = webData.DocumentNode
                    .SelectSingleNode("//div[@class='entry-content']")
                    .SelectSingleNode(".//a/img");

                //Convert to Meal Class
                var nodeParser = new NodeParser(url);
                var mealToAdd = nodeParser.NodeToMeal(recipeName, recipeDescription, recipeInstructions, recipeIngredients, url);

                //Download thumbnail
                var fileName = mealToAdd.Thumbnail;
                string directoryToStoreImgs = @"F:\Pantry\Thumbnails\";
                HtmlAttribute imgSrc = recipeThumbnail.Attributes["src"];
                string imgSrcString = imgSrc.Value;
                string filePath = Path.Combine(directoryToStoreImgs, fileName);

                using (var imgClient = new WebClient())
                {
                    imgClient.DownloadFile(imgSrcString, filePath);
                }
                mealToAdd.Thumbnail = fileName.ToString();

                //Save JSON
                var jsonFilePath = @"F:\Pantry\XXX.txt";

                if (File.Exists(jsonFilePath))
                {
                    var jsonExistingData = File.ReadAllText(jsonFilePath);
                    var existingMeals = JsonSerializer.Deserialize<List<Meal>>(jsonExistingData);
                    existingMeals.Add(mealToAdd);
                    var updatedJsonData = JsonSerializer.Serialize(existingMeals);
                    File.WriteAllText(jsonFilePath, updatedJsonData);
                }
                else
                {
                    var mealList = new List<Meal>();
                    mealList.Add(mealToAdd);
                    var jsonData = JsonSerializer.Serialize(mealList);
                    File.WriteAllText(jsonFilePath, jsonData);
                }
            }
            else
            {
                Console.WriteLine("Not a recipe page, proceeding onwards...");
            }
        }
    }
}

