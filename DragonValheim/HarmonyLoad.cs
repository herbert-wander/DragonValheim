using HarmonyLib;
using UnityEngine;
//using ValheimPictureFrame;

namespace DragonValheim
{
    
    class HarmonyLoad
    {
        static readonly Utils helper = DragonValheim.modInstance.Helper;

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        public static class ObjectDB_CopyOtherDB_Patch
        {
            public static void Postfix()
            {
                helper.UpdateCraftingStationsDic();
                DragonValheim.modInstance.RecipeManager.TryToRegisterRecipes();
                DragonValheim.modInstance.ConfigsManager.GenerateAllRecipesJsonFile();
            }
        }
        [HarmonyPatch(typeof(ObjectDB), "Awake")]
         public static class ObjectDB_Awake_Patch
         {
            public static void Postfix()
             {
                helper.UpdateCraftingStationsDic();
                DragonValheim.modInstance.RecipeManager.TryToRegisterRecipes();
                DragonValheim.modInstance.ConfigsManager.GenerateAllRecipesJsonFile();
            }
         }
        [HarmonyPatch(typeof(Player), "OnSpawned")]
        public class GetOnSpawned
        {
            public static void Postfix(Player __instance)
            {
                Tutorial.TutorialText newTutorial = new Tutorial.TutorialText()
                {
                    m_label = "DragonValheim Intro",
                    m_name = "dvintro",
                    m_text = "Bem vindo ao DragonValheim, um mod com vários QoL!\nEspero que se divirta!",
                    m_topic = "Bem Vindo ao DragonValheim"
                };

                if (!Tutorial.instance.m_texts.Contains(newTutorial))
                {
                    Tutorial.instance.m_texts.Add(newTutorial);
                }

                __instance.ShowTutorial("dvintro");
                //__instance.UpdateKnownRecipesList();
                //DragonRecipes recipeHelper = new DragonRecipes();
                //recipeHelper.TryToRegisterRecipesPlayer(__instance);
            }
        }
        [HarmonyPatch(typeof(Incinerator), "RPC_IncinerateRespons")]
        public class ModifyRPC_IncinerateRespons
        {
            /*
         * [Warning: Unity Log] OnIncinerate
         * [Warning: Unity Log] RPC_RequestIncinerate
         * [Warning: Unity Log] Incinerate
         * [Warning: Unity Log] RPC_AnimateLever
         * Warning: Unity Log] RPC_AnimateLeverReturn
         * [Warning: Unity Log] RPC_IncinerateRespons
         * [Warning: Unity Log] StopAOE
        */
            public static void Postfix(Incinerator __instance)
            {
                Recycler recycler = new Recycler();
                recycler.TryToInsertRecyledItens(__instance);
            }
        }

        [HarmonyPatch(typeof(Incinerator), "RPC_AnimateLeverReturn")]
        public class ModifyRPC_AnimateLeverReturn
        {
            public static void Prefix(Incinerator __instance)
            {
                Recycler recycler = new Recycler();
                recycler.TryToRecyle(__instance);
            }
        }

        [HarmonyPatch(typeof(Smelter), "OnAddFuel")]
        public class GetOnAddFuel
        {
            public static bool Prefix(bool __result, Smelter __instance, ref Humanoid user)
            {
                ProductionMachines smelter = new ProductionMachines();
                smelter.OnAddFuel(__instance, user);
                return __result;
                //Debug.Log("Opa Bom");
            }
        }
    }
}
