using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using UnityEngine;

namespace DragonValheim
{
    class Configuration
    {
        readonly Utils helper = DragonValheim.modInstance.Helper;
        readonly string localJsonPath = Directory.GetCurrentDirectory() + "\\BepInEx\\plugins\\DragonValheim\\";
        readonly string webJsonRecipesPath = "https://raw.githubusercontent.com/herbert-wander/modTesting/master/DragonRecipes.json";
        string recipesJson = null;
        readonly string webJsonAltRecipesPath = "https://raw.githubusercontent.com/herbert-wander/modTesting/master/DragonChangeRecipes.json";
        string altsJson = null;
        readonly string webJsonConfigFilePath = "https://raw.githubusercontent.com/herbert-wander/modTesting/master/configFile.json";
        string configJson = null;
        ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "Dragon_Valheim.cfg"), true);
        ConfigData.ConfigJsonData configData;

        public ConfigFile ConfigFile
        {
            get => configFile;
            set => configFile = value;
        }
        public string AltsJson
        {
            get => altsJson;
            set => altsJson = value;
        }
        public string RecipesJson
        {
            get => recipesJson;
            set => recipesJson = value;
        }

        void CreateConfigFile()
        {
            foreach (var item in configData.configDataBase)
            {
                ConfigFile.Bind(item.Section, item.Key, item.Value, item.Description);
            } 
        }
        public void InitiateAllConfigFiles()
        {
            //Mod Configs
            configJson = new WebClient().DownloadString(webJsonConfigFilePath);
            configData = helper.JsonConverter<ConfigData.ConfigJsonData>(configJson);
            System.IO.FileInfo file = new System.IO.FileInfo(localJsonPath);        
            file.Directory.Create();
            if (configData != null)
            {
                CreateConfigFile();
            }
            ConfigEntry<string> configEntry;
            DragonValheim.modInstance.ConfigsManager.ConfigFile.TryGetEntry<string>("Geral", "use_online_recipes", out configEntry);
            bool useOnline = bool.Parse(configEntry.Value);
            //New Recipes
            if (!File.Exists(localJsonPath+ "DragonRecipes.json") || useOnline)
            {
                recipesJson = new WebClient().DownloadString(webJsonRecipesPath); 
                if (!useOnline)
                {
                    File.WriteAllText(localJsonPath + "DragonRecipes.json", recipesJson);
                    Debug.LogError("Dragon Valheim Could not find RECIPES Json files!");
                    Debug.LogWarning("Dragon Valheim has Downloaded a RECIPE Json file from Mod Repository!");
                }  
            }
            else
            {
                recipesJson = helper.LoadJsonFile(localJsonPath + "DragonRecipes.json",false);
            }
            //Changed Recipes
            if (!File.Exists(localJsonPath + "DragonChangeRecipes.json") || useOnline)
            {
                altsJson = new WebClient().DownloadString(webJsonAltRecipesPath);
                if (!useOnline)
                {
                    File.WriteAllText(localJsonPath + "DragonChangeRecipes.json", altsJson);
                    Debug.LogError("Dragon Valheim Could not find ALT RECIPES Json files!");
                    Debug.LogWarning("Dragon Valheim has Downloaded a ALT RECIPE Json file from Mod Repository!");
                } 
            }
            else
            {
                altsJson = helper.LoadJsonFile(localJsonPath + "DragonChangeRecipes.json", false);
            }
            //newDragonRecipesJson = readJsonFile(Directory.GetCurrentDirectory() + "\\BepInEx\\plugins\\DragonValheim\\DragonRecipes.json");
            //changedDragonRecipesJson = readJsonFile(Directory.GetCurrentDirectory() + "\\BepInEx\\plugins\\DragonValheim\\DragonChangeRecipes.json");
        }

        public void GenerateAllRecipesJsonFile()
        {
            string allRecipesJson = "{\n\"recipes\":[\n";
            DragonRecipe converter = new DragonRecipe();
            foreach (var item in ObjectDB.instance.m_recipes)
            {
                DragonRecipe recipe = converter.RecipeToDragonRecipe(item);
                string data = recipe != null ? JsonConvert.SerializeObject(recipe, Formatting.Indented) :null;
                if (data != null)
                { 
                    allRecipesJson += data+",";
                }
            }
            allRecipesJson += "{}]}";
            allRecipesJson = allRecipesJson.Replace(",{}]}", "]\n}");
            allRecipesJson = allRecipesJson.Replace("},{", "},\n{");
            File.WriteAllText(localJsonPath + "AllRecipes.json", allRecipesJson);
        }
    }
}
