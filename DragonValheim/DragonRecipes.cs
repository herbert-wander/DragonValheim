using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace DragonValheim
{
    
    class DragonRecipes
    {
        [JsonIgnore, JsonProperty(Required = Required.Default)]
        Utils helper = DragonValheim.modInstance.Helper;
        [JsonIgnore,JsonProperty(Required = Required .Default)]
        DragonRecipesList newRecipesList = new DragonRecipesList();
        [JsonIgnore, JsonProperty(Required = Required.Default)]
        DragonRecipesList changedRecipesList = new DragonRecipesList();
        public static List<Recipe> playerNewRecipesList = new List<Recipe>();
        public class RecipeMaterials
        {
            private string item = null;
            private int? amount = null;

            public string Item
            {
                get => item;
                set => item = value;
            }
            public int? Amount
            {
                get => amount;
                set => amount = value;
            }
        }
        
        public class DragonRecipesList
        {
            List<DragonRecipes> recipes = new List<DragonRecipes>();

            public List<DragonRecipes> Recipes
            {
                get => recipes;
                set => recipes = value;
            }
        }
        
        bool isRecipeRegistred = false;       
        string name = null;  
        string item = null;
        int? amount = null;
        string craftingStation = null;
        int? minStationLevel = null;
        bool enabled = true;
        string repairStation = null;
        List<RecipeMaterials> resources = new List<RecipeMaterials>();
        //Gets and Sets

        public bool IsRecipeRegistred
        {
            get => isRecipeRegistred;
            set => isRecipeRegistred = value;
        }
        public string Name
        {
            get => name;
            set => name = value;
        }
        public string Item
        {
            get => item;
            set => item = value;
        }
        public int? Amount
        {
            get => amount;
            set => amount = value;
        }
        public string CraftingStation
        {
            get => craftingStation;
            set => craftingStation = value;
        }
        public int? MinStationLevel
        {
            get => minStationLevel;
            set => minStationLevel = value;
        }
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }
        public string RepairStation
        {
            get => repairStation;
            set => repairStation = value;
        }
        public List<RecipeMaterials> Resources
        {
            get => resources;
            set => resources = value;
        }

        public void GenerateRecipesList(Configurations configsManager)
        {
            newRecipesList = helper.JsonConverter<DragonRecipesList>(configsManager.RecipesJson);
            changedRecipesList = helper.JsonConverter<DragonRecipesList>(configsManager.AltsJson);
        }

        public void TryToRegisterRecipes()
        {
            foreach (var item in newRecipesList.Recipes)
            {
                LoadRecipe(item);
            }
            foreach (var item in changedRecipesList.Recipes)
            {
                LoadRecipe(item);
            }
        }

        void LoadRecipe(DragonRecipes recipe)
        {
            if (!recipe.isRecipeRegistred && recipe.enabled && helper.CraftingStations.ContainsKey(recipe.craftingStation))
            {
                bool isNewRecipe = false;
                Recipe newRecipe = null;
                newRecipe = ObjectDB.instance.m_recipes.Find(x => x.name == recipe.name);
                if (newRecipe == null)
                {
                    isNewRecipe = true;
                    newRecipe = ScriptableObject.CreateInstance<Recipe>();
                }
                newRecipe.name = recipe.name;
                newRecipe.m_repairStation = helper.CraftingStations[recipe.repairStation];
                newRecipe.m_amount = (int)recipe.amount;
                newRecipe.m_craftingStation = helper.CraftingStations[recipe.craftingStation];
                newRecipe.m_minStationLevel = (int)recipe.minStationLevel;
                newRecipe.m_item = ObjectDB.instance.GetItemPrefab(recipe.item).GetComponent<ItemDrop>();
                List<Piece.Requirement> craftMaterials = new List<Piece.Requirement>();
                foreach (var material in recipe.resources)
                {
                    craftMaterials.Add(new Piece.Requirement() { m_amount = (int)material.Amount, m_resItem = ObjectDB.instance.GetItemPrefab(material.Item).GetComponent<ItemDrop>() });
                }
                newRecipe.m_resources = craftMaterials.ToArray();
                if (isNewRecipe)
                {
                    playerNewRecipesList.Add(newRecipe);
                    ObjectDB.instance.m_recipes.Add(newRecipe);
                    ObjectDB.instance.m_recipes.Sort((x, y) => x.name.CompareTo(y.name));
                }
                recipe.isRecipeRegistred = true;
            }
        }

        public  void TryToRegisterRecipesPlayer(Player player)
        {
            if (playerNewRecipesList.Count > 0)
            {
                foreach (var recipe in playerNewRecipesList)
                {
                    if (!player.IsRecipeKnown(recipe.name))
                    {
                        if (recipe.m_craftingStation != null)
                        {
                            if (player.KnowStationLevel(recipe.m_craftingStation.m_name, recipe.m_minStationLevel))
                            {
                                bool areAllMaterialsKnow = true;
                                foreach (var material in recipe.m_resources)
                                {
                                    if (!player.IsKnownMaterial(material.m_resItem.m_itemData.m_shared.m_name))
                                    {
                                        areAllMaterialsKnow = false;
                                    }
                                }
                                if (areAllMaterialsKnow)
                                {
                                    Debug.LogWarning("NEW Recipe {"+recipe.name+"} Added");
                                    player.AddKnownRecipe(recipe);
                                }
                            }                   
                        }
                    }
                }
            }                       
        }
    }
}
/**
 {
  "recipes": [
    {
      "isRecipeRegistred": false,
      "name": "teste",
      "item": "teste",
      "amount": 5,
      "craftingStation": "",
      "minStationLevel": "",
      "enabled": true,
      "repairStation": "",
      "resources": [
        {
          "item": "teste",
          "amount": 5
        },
        {
          "item": "teste",
          "amount": 5
        }
      ]
    }
  ]
}
$piece_forge
$piece_artisanstation
$piece_stonecutter
$piece_workbench
$piece_cauldron
 **/
