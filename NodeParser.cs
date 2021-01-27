using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PantryDataCollector
{
    public class NodeParser
    {
        public Meal NewMeal { get; set; }
        public string URL { get; set; }

        public NodeParser(string recipeURL)
        {
            NewMeal = new Meal();
            URL = recipeURL;
        }

        // Nodes to Meal
        public Meal NodeToMeal(
            HtmlNode nameNode, 
            HtmlNode descriptionNode, 
            HtmlNodeCollection instructionNode, 
            HtmlNodeCollection ingredientNode,
            string URL)
        {
            NewMeal.Name = NameParser(nameNode);
            NewMeal.Description = DescriptionParser(descriptionNode);
            NewMeal.Instructions = InstructionParser(instructionNode);
            NewMeal.MealIngredients = MealIngredientParser(ingredientNode);
            NewMeal.URL = URL;
            NewMeal.Thumbnail = ThumbnailParser(NewMeal.Name);
            return NewMeal;
        }

        // Node to Meal.Name
        public string NameParser(HtmlNode node)
        {
            string dataString = node.InnerText;
            string mealName = dataString.Trim();
            return mealName;
        }

        // Node to Meal.Description
        public string DescriptionParser(HtmlNode node)
        {
            string dataString = node.InnerText;
            string mealDesc = dataString.Trim();
            return mealDesc;
        }

        // Node to Meal.Instructions
        public string InstructionParser(HtmlNodeCollection nodes)
        {
            var instructionSB = new StringBuilder();
            foreach (var node in nodes)
            {
                var instructionUnformatted = node.Attributes["id"].Value;
                var stepDecoded = HttpUtility.HtmlDecode(instructionUnformatted);
                var stepSplit = stepDecoded.Split('-')[2];
                var instructionString = node.InnerText.Trim();
                instructionSB.Append(stepSplit + ". ");
                instructionSB.Append(instructionString);
            }
            var rawInstructions = instructionSB.ToString();
            var instructionStringFormatted = HttpUtility.HtmlDecode(rawInstructions);
            return instructionStringFormatted;
        }

        // Node to Meal.Ingredients
        public List<MealIngredient> MealIngredientParser(HtmlNodeCollection nodes)
        {
            var mealIngredients = new List<MealIngredient>();
            foreach (var node in nodes)
            {
                var ingredientUnformatted = node.InnerText;

                // Remove extraneous descriptors
                if (ingredientUnformatted.Contains(','))
                {
                    var ingredientName = ingredientUnformatted.Split(',')[0].Trim();
                    var ingredientToAdd = new Ingredient { Name = ingredientName };
                    var newMealIngredient = new MealIngredient { Ingredient = ingredientToAdd };
                    mealIngredients.Add(newMealIngredient);
                }
                else
                {
                    var ingredientName = ingredientUnformatted.Trim();
                    var ingredientToAdd = new Ingredient { Name = ingredientName };
                    var newMealIngredient = new MealIngredient { Ingredient = ingredientToAdd };
                    mealIngredients.Add(newMealIngredient);
                }
            }
            return mealIngredients;
        }

        // Node to Meal.Thumbnail
        public string ThumbnailParser(string RecipeName)
        {
            var fileNameBuilder = new StringBuilder();
            var recipeURL = URL.Split('/');
            var recipeWebsiteName = recipeURL[2].Replace(".com", "");
            var recipeName = RecipeName.Replace(" ", "").ToLower();
            fileNameBuilder.Append(recipeWebsiteName + "_" + recipeName + ".jpeg");
            var fileName = fileNameBuilder.ToString();
            return fileName;
        }
    }
}
