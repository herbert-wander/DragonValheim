using BepInEx.Configuration;
using System;

namespace DragonValheim
{
    class ProductionMachines
    {
        Utils helper = DragonValheim.modInstance.Helper;
        private class ItemConversionData
        {
            string rawMaterial = null;
            string refinedMaterial = null;

            public string RawMaterial
            {
                get => rawMaterial;
                set => rawMaterial = value;
            }
            public string RefinedMaterial
            {
                get => refinedMaterial;
                set => refinedMaterial = value;
            }
        }
        public void OnAddFuel(Smelter instance, Humanoid user)
        {
            int coalAmount = 0;
            foreach (var itemInventory in user.m_inventory.m_inventory)
            {
                if (itemInventory.m_shared.m_name.ToLower() == "$item_coal")
                {
                    coalAmount += itemInventory.m_stack;
                }
            }
            if (coalAmount >= 1)
            {
                int spaceAvaiable = instance.m_maxFuel - (int)instance.GetFuel();
                ConfigEntry<string> configEntry;
                DragonValheim.modInstance.ConfigsManager.ConfigFile.TryGetEntry<string>("ProductionStations", "fuel_per_interaction", out configEntry);
                if (coalAmount >= Int32.Parse(configEntry.Value))
                {
                    coalAmount = Int32.Parse(configEntry.Value);
                }

                instance.SetFuel((float)(instance.GetFuel() + coalAmount));

                user.m_inventory.RemoveItem("$item_coal", coalAmount);
            }
        }

        public void OnAddOre(Smelter instance, Humanoid user)
        {
            int materialAmount = 0;
            string materialName = null;
            for (int i = 0; i < instance.m_conversion.Count; i++)
            {
                if (user.m_inventory.HaveItem("$item_" + instance.m_conversion[i].m_from.name.ToLower()))
                {
                    materialName = instance.m_conversion[i].m_from.name;
                    i = instance.m_conversion.Count;
                }
            }
            if (materialName != null)
            {
                foreach (var itemInventory in user.m_inventory.m_inventory)
                {
                    if (itemInventory.m_shared.m_name == "$item_" + materialName.ToLower())
                    {
                        materialAmount += itemInventory.m_stack + 1;
                    }
                }
            }
            if (materialAmount >= 1)
            {
                int spaceAvaiable = instance.m_maxOre - instance.GetQueueSize() + 1;
                ConfigEntry<string> configEntry;
                DragonValheim.modInstance.ConfigsManager.ConfigFile.TryGetEntry<string>("ProductionStations", "fuel_per_interaction", out configEntry);
                if (materialAmount >= Int32.Parse(configEntry.Value))
                {
                    materialAmount = Int32.Parse(configEntry.Value);
                }
                for (int i = 0; i < materialAmount - 1; i++)
                {
                    instance.QueueOre(materialName);
                }
                user.m_inventory.RemoveItem("$item_" + materialName.ToLower(), materialAmount - 1);
            }
        }

        public void AwakeChanges(Smelter instance)
        {
            if (instance.m_name.Equals("$piece_blastfurnace"))
            {
                string[] newRawMaterials = { "CopperOre", "IronOre" , "IronScrap", "TinOre" , "SilverOre" };
                string[] newRefinedMaterials = { "Copper", "Iron" , "Iron" , "Tin" , "Silver" };
                for (int i = 0; i < newRefinedMaterials.Length; i++)
                {
                    Smelter.ItemConversion newConversionData = new Smelter.ItemConversion();
                    newConversionData.m_from = helper.GetItemDropFromObjectDB(newRawMaterials[i]);
                    newConversionData.m_to = helper.GetItemDropFromObjectDB(newRefinedMaterials[i]);
                    instance.m_conversion.Add(newConversionData);
                }
            }
        }
    }
}
