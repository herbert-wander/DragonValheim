using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DragonValheim
{
    [BepInPlugin("org.bepinex.plugins.dragonvalheim", "Dragon Valheim", "1.0.0.0")]
    class DragonValheim : BaseUnityPlugin
    {
        public static DragonValheim modInstance;
        Harmony harmony = new Harmony("DragonMods");
        Configurations configsManager;
        DragonRecipes recipeManager;
        HarmonyLoad harmonyPatchers;
        Utils helper = new Utils();
        public DragonRecipes RecipeManager
        {
            get => recipeManager;
            set => recipeManager = value;
        }
        public Configurations ConfigsManager
        {
            get => configsManager;
            set => configsManager = value;
        }
        
        public Utils Helper
        {
            get => helper;
            set => helper = value;
        }

        private void Awake()
        {
            modInstance = this;
            configsManager = new Configurations();
            recipeManager = new DragonRecipes();
            harmonyPatchers = new HarmonyLoad();
            Debug.LogWarning("DRAGON VALHEIM GOING FAST");
            configsManager.InitiateAllConfigFiles();
            recipeManager.GenerateRecipesList(configsManager);    
            harmony.PatchAll();
            Debug.LogWarning("DRAGON VALHEIM GOING FAST AS FUCK BOY");
        }
    }
}
