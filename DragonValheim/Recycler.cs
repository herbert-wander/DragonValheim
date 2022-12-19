using System;
using UnityEngine;

namespace DragonValheim
{
    class Recycler
    {
        static readonly Utils helper = DragonValheim.modInstance.Helper;
        public void TryToRecyle(Incinerator obliterator)
        {
            Recipe receipeRecycle;
            helper.InventoryCopy.Clear();
            foreach (var itemFE in obliterator.m_container.m_inventory.m_inventory)
            {
                if (itemFE.m_shared.m_name != null)
                {
                    receipeRecycle = helper.GetRecipeFromObjectDB(itemFE.m_shared.m_name);
                    if (receipeRecycle != null)
                    { 
                        foreach (var material in receipeRecycle.m_resources)
                        {
                            GameObject itemToAdd = UnityEngine.Object.Instantiate(ObjectDB.instance.GetItemPrefab(material.m_resItem.name));
                            itemToAdd.GetComponent<ItemDrop>().m_itemData.m_stack = 0;
                            for (int qualityLevel = 1; qualityLevel <= itemFE.m_quality; qualityLevel++)
                            {
                                itemToAdd.GetComponent<ItemDrop>().m_itemData.m_stack += material.GetAmount(qualityLevel) * itemFE.m_stack;
                            }
                            if (itemToAdd.GetComponent<ItemDrop>().m_itemData.m_stack > 0)
                            {
                                helper.InventoryCopy.Add(itemToAdd);
                            }
                        }  
                    }
                }
            }
            obliterator.m_container.m_inventory.RemoveAll();
            obliterator.m_container.m_inventory.AddItem(ObjectDB.instance.GetItemPrefab("Coal"), 1);
        }

        public void TryToInsertRecyledItens(Incinerator obliterator)
        {
            if (helper.InventoryCopy != null && helper.InventoryCopy.Count > 0)
            {
                int avaiableSlot = (obliterator.m_container.m_inventory.m_width * obliterator.m_container.m_inventory.m_height);
                //if (avaiableSlot > helper.InventoryCopy.Count)
                //{
                int allItens = helper.InventoryCopy.Count < avaiableSlot ? helper.InventoryCopy.Count : avaiableSlot;
                for (int i = 0; i < allItens; i++)
                {
                    obliterator.m_container.m_inventory.AddItem(helper.InventoryCopy[i].GetComponent<ItemDrop>().m_itemData);
                }
                Vector3 pos = obliterator.gameObject.transform.position;
                pos.x += 2;
                for (int i = avaiableSlot+1; i < helper.InventoryCopy.Count; i++)
                {    
                    ItemDrop.DropItem((helper.InventoryCopy[i].GetComponent<ItemDrop>().m_itemData), 1, pos, obliterator.gameObject.transform.rotation);
                }
                    
                //}
                /*foreach (var item in helper.InventoryCopy)
                {
                    obliterator.m_container.m_inventory.AddItem(item.GetComponent<ItemDrop>().m_itemData);
                }*/
            }
        }
    }
}