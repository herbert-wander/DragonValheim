using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace DragonValheim
{
    [BepInPlugin("org.bepinex.plugins.dragonvalheim", "Dragon Valheim", "1.0.0.0")]
    class DragonValheim : BaseUnityPlugin
    {
        public static DragonValheim modInstance;
        readonly Harmony harmony = new Harmony("DragonMods");
        Configuration configsManager;
        DragonRecipe recipeManager;
        Utils helper = new Utils();
        public DragonRecipe RecipeManager
        {
            get => recipeManager;
            set => recipeManager = value;
        }
        public Configuration ConfigsManager
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
            configsManager = new Configuration();
            recipeManager = new DragonRecipe();
            Debug.LogWarning("DRAGON VALHEIM GOING FAST");
            configsManager.InitiateAllConfigFiles();
            recipeManager.GenerateRecipesList(configsManager);    
            harmony.PatchAll();
            Debug.LogWarning("DRAGON VALHEIM GOING FAST AS FUCK BOY");
        }
    }
}
