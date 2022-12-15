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
                Recipe receipeRecycle;
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
    }
}
