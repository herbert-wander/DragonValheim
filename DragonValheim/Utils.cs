using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;


namespace DragonValheim
{
    class Utils
    {
        Dictionary<string, CraftingStation> craftingStations = new Dictionary<string, CraftingStation>();
        List<GameObject> inventoryCopy = new List<GameObject>();
        public List<GameObject> InventoryCopy
        {
            get => inventoryCopy;
            set => inventoryCopy = value;
        }
        public Dictionary<string, CraftingStation> CraftingStations
        {
            get => craftingStations;
            set => craftingStations = value;
        }

        public string GetStageColor(double percentage)
        {
            string color;
            if (percentage >= 15 && percentage <= 29)
            {
                color = "#ff7700";
            }
            else if (percentage >= 30 && percentage <= 44) 
            {
                color = "#fff700";
            }
            else if (percentage >= 45 && percentage <= 59)
            {
                color = "#aaff00";
            }
            else if (percentage >= 60 && percentage <= 74)
            {
                color = "#09ff00";
            }
            else if (percentage >= 75 && percentage <= 89)
            {
                color = "#00fbff";
            }
            else if (percentage >= 90 && percentage <= 100)
            {
                color = "#0066ff";
            }
            else
            {
                color = "#ff0000";
            }

            return color;
        }

        public string FormatSecondsToTime(int seconds)
        {
            string formatedTime = null;
            int hours = seconds / 60 / 60;
            int mins = (seconds / 60) % 60;
            int secs = seconds % 60;
            if (hours >= 1)
            {
                formatedTime += hours + "H "; 
            }
            if (mins >= 1)
            {
                formatedTime += mins + "M ";
            }
            if (secs >= 1)
            {
                formatedTime += secs + "S ";
            }
            return formatedTime;
        }

        public double GetPercentage(double active, double max)
        {
            return Math.Round(active / max * 100, MidpointRounding.AwayFromZero);
        }

        public string LoadJsonFile(string jsonPath,bool isWebPath)
        {
            string JsonText = null;
            if (isWebPath)
            {
                JsonText = new System.Net.WebClient().DownloadString(jsonPath);
            }
            else
            {                
                if (!File.Exists(jsonPath))
                {
                    File.CreateText(jsonPath);
                    Debug.LogError("Dragon Valheim Could not find Json files!");
                    Debug.LogWarning("Dragon Valheim has created new blank Json files!");
                }
                using (StreamReader sr = File.OpenText(jsonPath))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        JsonText += s;
                    }
                }
            }
            return JsonText;
        }

        public T JsonConverter<T>(string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public Recipe GetRecipeFromObjectDB(string searchKey)
        {
            foreach (var itemDB in ObjectDB.instance.m_recipes)
            {
                if (itemDB.m_item != null)
                {
                    if (itemDB.m_item.m_itemData != null)
                    {
                        if (itemDB.m_item.m_itemData.m_shared != null)
                        {
                            if (searchKey.Equals(itemDB.m_item.m_itemData.m_shared.m_name))
                            {
                                return itemDB;
                            }
                        }
                    }
                }
            }
            return null;
        }
        public ItemDrop GetItemDropFromObjectDB(string searchKey)
        {
            foreach (var itemDB in ObjectDB.instance.m_items)
            {
                if (searchKey.Equals(itemDB.name))
                {
                    ItemDrop resultItemData = itemDB.GetComponent<ItemDrop>();
                    return resultItemData;
                }
            }
            return null;
        }
        public ItemDrop.ItemData GetItemDataFromObjectDB(ItemDrop.ItemData searchKey)
        {
            foreach (var itemDB in ObjectDB.instance.m_items)
            {
                if (searchKey.m_shared.m_name.Equals(itemDB.GetComponent<ItemDrop>().m_itemData.m_shared.m_name))
                {
                    ItemDrop.ItemData resultItemData = itemDB.GetComponent<ItemDrop>().m_itemData;
                    return resultItemData;
                }
            }
            return null;
        }

        public void UpdateCraftingStationsDic()
        {
            foreach (var item in ObjectDB.instance.m_recipes)
            {
                if (item.m_craftingStation != null && !craftingStations.ContainsKey(item.m_craftingStation.m_name))
                {
                    try
                    {
                        craftingStations.Add(item.m_craftingStation.m_name, item.m_craftingStation);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Dragon Valheim tried to add a NewStation, but that one was already registered!" + e.Message);
                    }
                }
            }
            
        }
    }
}
