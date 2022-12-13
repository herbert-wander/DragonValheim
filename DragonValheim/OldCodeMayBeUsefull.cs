using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonValheim
{
    class OldCodeMayBeUsefull
    {
        /*[HarmonyPatch(typeof(ObjectDB), "Awake")]
        public static class ObjectDB_Awake_Patch
        {
            public static void Postfix()
            {
                //helper.UpdateCraftingStationsDic(__instance);
                DragonValheim.modInstance.RecipeManager.TryToRegisterRecipes();
            }
        }*/

        /*[HarmonyPatch(typeof(CraftingStation), "Start")]
        public class CraftingStation_Start_Patch
        {
            public static void Postfix(ref CraftingStation __instance)
            {
                helper.UpdateCraftingStationsDic(__instance);
                DragonValheim.modInstance.RecipeManager.TryToRegisterRecipes();
            }
        }*/

        /*[HarmonyPatch(typeof(Plant), "Awake")]
        public class Plant_Awake_Patch
        {
            public static void Postfix(ref Plant __instance)
            {
                //Debug.LogWarning("Plant_Awake_Patch");
                Plantas planta = new Plantas();
                planta.removePlantBiomePlantingRestriction(__instance);
            }
        }*/

        /*[HarmonyPatch(typeof(Plant), "GetHoverText")]
        public class GetHoverTextPlant
        {
            public static string Postfix(string __result, Plant __instance)
            {
                //Debug.LogWarning("GetHoverTextPlant");
                Plantas planta = new Plantas();
                return __result.Replace(" )", $", {planta.InsertTimerInHoverText(__instance, __result)} )");
            }
        }*/

        /*[HarmonyPatch(typeof(Pickable), "Drop")]
        public class ModifyPickableDrop
        {
            public static void Prefix(ref int stack, Pickable __instance)
            {
                if (__instance.name.ToLower().Contains("mushroom") || __instance.name.ToLower().Contains("raspberry") || __instance.name.ToLower().Contains("thistle") || __instance.name.ToLower().Contains("blueberry"))
                {
                    PickableDV pickable = new PickableDV();
                    pickable.SetRespawnTime(__instance, 60);
                    stack = 3;
                }
            }
        }*/

        /*[HarmonyPatch(typeof(Pickable), "GetHoverText")]
        public class GetHoverTextPickable
        {
            public static void Postfix(string __result, Pickable __instance)
            {
                if (__instance.name.ToLower().Contains("mushroom") || __instance.name.ToLower().Contains("raspberry") || __instance.name.ToLower().Contains("thistle") || __instance.name.ToLower().Contains("blueberry"))
                {
                    __result += " || Timer => "+__instance.m_respawnTimeMinutes + " --- "+__instance.m_spawnOffset;
                }
            }
        }*/

        /*[HarmonyPatch(typeof(PictureFrameBase), "GetHoverText")]
        public class GetHoverTextPictureFrameBase
        {
            public static string Postfix(string __result, PictureFrameBase __instance)
            {
                return __result = __result.Split(':')[2].Split(' ')[0];
            }
        }*/
        /*[HarmonyPatch(typeof(Player), "OnSpawned")]
        public class GetOnSpawned
        {
            public static void Postfix(Player __instance)
            {
                DragonRecipes recipesHelper = new DragonRecipes();
                foreach (var item in ObjectDB.m_instance.m_recipes)
                {
                    //solve some Player x Server sync problems
                    recipesHelper.TryToRegisterRecipesPlayer(item, __instance);
                }
            }
        }*/
        /*[HarmonyPatch(typeof(Smelter), "OnAddOre")]
            public class GetOnAddOre
            {
                public static bool Postfix(bool __result, Smelter __instance, ref Humanoid user)
                {
                    /*
                     * $piece_windmill
                     * $piece_spinningwheel
                     * $piece_blastfurnace
                     * $piece_smelter
                     *//*
                    //MonsterAI;
                    //CreatureSpawner;
                    ProductionMachines smelter = new ProductionMachines();
                    smelter.OnAddOre(__instance, user);

                    return __result;
                }
            }*/

        /*[HarmonyPatch(typeof(Smelter), "Awake")]
        public class ModifySmelterAwake
        {
            public static void Postfix(Smelter __instance)
            {
                ProductionMachines smelter = new ProductionMachines();
                smelter.AwakeChanges(__instance);
            }
        }*/
        /*[HarmonyPatch(typeof(Piece), "Awake")]
        public static class get_all_item
        {
            public static void Postfix(ref Piece __instance)
            {
                //Debug.LogWarning("Plant_Awake_Patch");
                string path = Directory.GetCurrentDirectory() + "\\BepInEx\\plugins\\DragonValheim";
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "ItemList.txt"), true))
                {
                    foreach (var item in __instance.m_items)
                    {
                        outputFile.WriteLine(Localization.instance.Localize(item.GetComponentInChildren<ItemDrop>().m_itemData.m_shared.m_name) + " = " + item.name);
                    }
                }
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "RecipeList.txt"), true))
                {
                    foreach (var item in __instance.m_recipes)
                    {
                        outputFile.WriteLine(item.name);
                    }
                }
            }
        }*/
    }
}
