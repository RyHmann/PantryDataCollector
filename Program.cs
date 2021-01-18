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

namespace WebScrapeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var recipeUrl = "https://pinchofyum.com/coconut-green-curry-lentils";
            var recipeUrl = "https://pinchofyum.com/coconut-curry-salmon";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(recipeUrl);

            //Title
            //<h2 class="tasty-recipes-title">
            var recipeTitle = document.DocumentNode.SelectSingleNode("//h1[@class='entry-title']");

            //Description
            var recipeDescription = document.DocumentNode.SelectSingleNode("//div[@class='tasty-recipes-description-body']/p");

            //Instructions
            var recipeInstructions = document.DocumentNode.SelectNodes("//div[@class='tasty-recipes-instructions']/div/ol/li");

            //Ingredients
            var recipeIngredients = document.DocumentNode
                .SelectSingleNode("//div[@class='tasty-recipes-ingredients']")
                .SelectNodes(".//ul/li/strong");

            //Thumbnail
            var recipeThumbnail = document.DocumentNode
                .SelectSingleNode("//div[@class='entry-content']")
                .SelectSingleNode(".//a/img");

            //Convert to Meal Class
            var meal = new Meal();

            //Meal Title
            meal.Title = recipeTitle.InnerText;

            //Meal URL
            meal.URL = recipeUrl;

            //Meal Description
            meal.Description = recipeDescription.InnerText;

            //Meal Instructions
            var instructionSB = new StringBuilder();
            foreach (var node in recipeInstructions)
            {
                var stepUnformatted = node.Attributes["id"].Value;
                var stepDecoded = HttpUtility.HtmlDecode(stepUnformatted);
                var stepSplit = stepDecoded.Split('-')[2];

                var instructionsUnformatted = node.InnerText;
                //Trim Instructions

                instructionSB.Append(stepSplit + ". ");
                instructionSB.Append(instructionsUnformatted);
            }
            var instructionString = instructionSB.ToString();
            var instructionStringFormatted = HttpUtility.HtmlDecode(instructionString);
            meal.Instructions = instructionStringFormatted;

            //Meal Ingredients
            var ingredients = new List<string>();
            Console.WriteLine(recipeIngredients.Count());
            foreach (var node in recipeIngredients)
            {
                var ingredientUnformatted = node.InnerText;

                //Remove extraneous details from ingredients
                if (ingredientUnformatted.Contains(','))
                {
                    var ingredient = ingredientUnformatted.Split(',')[0].Trim();
                    ingredients.Add(ingredient);
                }
                else
                {
                    ingredients.Add(ingredientUnformatted.Trim());
                }
            }
            meal.Ingredients = ingredients;

            //Meal Thumbnail
            var fileNameBuilder = new StringBuilder();
            var recipeWebsite = recipeUrl.Split('/');
            var recipeWebsiteName = recipeWebsite[2].Replace(".com", "");
            var recipeName = meal.Title.Replace(" ", "").ToLower();
            fileNameBuilder.Append(recipeWebsiteName + "_" + recipeName + ".jpeg");
            var fileName = fileNameBuilder.ToString();

            string directoryToStoreImgs = @"F:\Pantry\Thumbnails\";
            HtmlAttribute imgSrc = recipeThumbnail.Attributes["src"];
            string imgSrcString = imgSrc.Value;
            string filePath = Path.Combine(directoryToStoreImgs, fileName);


            //Download thumbnail
            using (var imgClient = new WebClient())
            {
                imgClient.DownloadFile(imgSrcString, filePath);
            }

            meal.Thumbnail = fileName.ToString();

            //Print Instructions to Console
            Console.WriteLine(meal.Instructions);

            //JSON Serializer
            string jsonString = JsonSerializer.Serialize(meal);
            Console.WriteLine(jsonString);
        }
    }
}

