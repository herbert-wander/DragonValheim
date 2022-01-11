using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using ValheimPictureFrame;

namespace DragonValheim
{
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
    class HarmonyLoad
    {
        static Utils helper = DragonValheim.modInstance.Helper;

        [HarmonyPatch(typeof(CraftingStation), "Start")]
        public class CraftingStation_Start_Patch
        {
            public static void Postfix(ref CraftingStation __instance)
            {
                helper.UpdateCraftingStationsDic(__instance);
                DragonValheim.modInstance.RecipeManager.TryToRegisterRecipes();
            }
        }

        [HarmonyPatch(typeof(Plant), "Awake")]
        public class Plant_Awake_Patch
        {
            public static void Postfix(ref Plant __instance)
            {
                //Debug.LogWarning("Plant_Awake_Patch");
                Plantas planta = new Plantas();
                planta.removePlantBiomePlantingRestriction(__instance);/**/
            }
        }

        [HarmonyPatch(typeof(Plant), "GetHoverText")]
        public class GetHoverTextPlant
        {
            public static string Postfix(string __result, Plant __instance)
            {
                //Debug.LogWarning("GetHoverTextPlant");
                Plantas planta = new Plantas();
                return __result.Replace(" )", $", {planta.InsertTimerInHoverText(__instance, __result)} )")/**/;
            }
        }

        [HarmonyPatch(typeof(Pickable), "Drop")]
        public class ModifyPickableDrop
        {
            public static void Prefix(ref int stack, Pickable __instance)
            {
                if (__instance.name.ToLower().Contains("mushroom") || __instance.name.ToLower().Contains("raspberry") || __instance.name.ToLower().Contains("thistle") || __instance.name.ToLower().Contains("blueberry"))
                {
                    PickableDV pickable = new PickableDV();
                    pickable.SetRespawnTime(__instance, 60);
                    stack = 3;
                }/**/
            }
        }

        [HarmonyPatch(typeof(Pickable), "GetHoverText")]
        public class GetHoverTextPickable
        {
            public static void Postfix(string __result, Pickable __instance)
            {
                if (__instance.name.ToLower().Contains("mushroom") || __instance.name.ToLower().Contains("raspberry") || __instance.name.ToLower().Contains("thistle") || __instance.name.ToLower().Contains("blueberry"))
                {
                    __result += " || Timer => "+__instance.m_respawnTimeMinutes + " --- "+__instance.m_spawnOffset;
                }/**/
            }
        }

        [HarmonyPatch(typeof(PictureFrameBase), "GetHoverText")]
        public class GetHoverTextPictureFrameBase
        {
            public static string Postfix(string __result, PictureFrameBase __instance)
            {
                return __result = __result.Split(':')[2].Split(' ')[0];
            }
        }

        [HarmonyPatch(typeof(Player), "OnSpawned")]
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

                if (helper.InventoryCopy != null && helper.InventoryCopy.Count > 0)
                {
                    foreach (var item in helper.InventoryCopy)
                    {
                        __instance.m_container.m_inventory.AddItem(item.GetComponent<ItemDrop>().m_itemData);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Incinerator), "RPC_AnimateLeverReturn")]
        public class ModifyRPC_AnimateLeverReturn
        {

            public static void Prefix(Incinerator __instance)
            {
                Recipe receipeRecycle = null;
                helper.InventoryCopy.Clear();
                int avaiableSlot = (__instance.m_container.m_inventory.m_width * __instance.m_container.m_inventory.m_height) - __instance.m_container.m_inventory.NrOfItems();
                foreach (var itemFE in __instance.m_container.m_inventory.m_inventory)
                {
                    if (itemFE.m_shared.m_name != null)
                    {
                        receipeRecycle = helper.GetRecipeFromObjectDB(itemFE.m_shared.m_name);
                        if (receipeRecycle != null)
                        {
                            if (avaiableSlot + 1 - receipeRecycle.m_resources.Length >= 0)
                            {
                                foreach (var item in receipeRecycle.m_resources)
                                {
                                    GameObject itemToAdd = UnityEngine.Object.Instantiate(ObjectDB.instance.GetItemPrefab(item.m_resItem.name));
                                    itemToAdd.GetComponent<ItemDrop>().m_itemData.m_stack = item.m_amount * itemFE.m_stack;
                                    helper.InventoryCopy.Add(itemToAdd);

                                }
                            }
                        }
                    }
                }
                __instance.m_container.m_inventory.RemoveAll();
                __instance.m_container.m_inventory.AddItem(ObjectDB.instance.GetItemPrefab("Coal"), 1);
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

        [HarmonyPatch(typeof(Smelter), "OnAddOre")]
        public class GetOnAddOre
        {
            public static bool Postfix(bool __result, Smelter __instance, ref Humanoid user)
            {
                /*
                 * $piece_windmill
                 * $piece_spinningwheel
                 * $piece_blastfurnace
                 * $piece_smelter
                 */
                //MonsterAI;
                //CreatureSpawner;
                ProductionMachines smelter = new ProductionMachines();
                smelter.OnAddOre(__instance, user);
               /**/
                return __result;
            }
        }

        [HarmonyPatch(typeof(Smelter), "Awake")]
        public class ModifySmelterAwake
        {
            public static void Postfix(Smelter __instance)
            {
                ProductionMachines smelter = new ProductionMachines();
                smelter.AwakeChanges(__instance);
            }
        }
    }
}
