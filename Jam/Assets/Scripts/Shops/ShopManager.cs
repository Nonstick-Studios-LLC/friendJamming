using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : Singleton<ShopManager>
{
    private int playersCurrency;

    [SerializeField, InfoBox("There MUST be two or more items in here. One MUST be the McGuffin", type:EInfoBoxType.Warning)] private List<ItemScriptable> potentialShopItems = new List<ItemScriptable>();
    static private ItemScriptable[] staticShopList;
    private static bool spawnedMcGuffin = false;
    [Header("UI")]
    [SerializeField] private TMP_Text currencyDisplay;
    [SerializeField] private GameObject sellMenu;
    [SerializeField] private TMP_Text sellMenuText;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button nextItemButton;
    [SerializeField] private GameObject buyMenu;

    [SerializeField] private TMP_Text buyMenuText;
    [Header("Testing")]
    [SerializeField] private List<ItemScriptable> ownedTreasure;

    int itemSellIndex = 0;



     protected override  void Awake()
    {
        base.Awake();
        staticShopList = new ItemScriptable[potentialShopItems.Count];
        potentialShopItems.CopyTo(staticShopList);
        Debug.Log("Copied to array");
        PublicEvents.UpdateScrapCount?.Invoke(0);
    }


    public static ItemScriptable GetRandomItemData(int seed = 0)
    {
        if(!spawnedMcGuffin)
        {
            for (int i = 0; i < staticShopList.Length; i++)
            {
                if (staticShopList[i].GetMcGuffin())
                {
                    spawnedMcGuffin = true;
                    return staticShopList[i];
                }
            }
        }
        if(seed != 0)
        {
            UnityEngine.Random.InitState(seed);
        }
        int index = UnityEngine.Random.Range(0, staticShopList.Length);
        while (staticShopList[index].GetMcGuffin())
        {
            index = UnityEngine.Random.Range(0, staticShopList.Length);
        }

        return staticShopList[index];

        

    }

    [Button] public void SellItem()
    {
        if(ownedTreasure.Count > 0)
        {
            if (!ownedTreasure[itemSellIndex].GetMcGuffin())
            {
                playersCurrency += ownedTreasure[itemSellIndex].GetSellPrice();
                ownedTreasure.Remove(ownedTreasure[itemSellIndex]);
                PublicEvents.UpdateScrapCount?.Invoke(playersCurrency);
                currencyDisplay.text = "Scrap: $" + playersCurrency.ToString();

                itemSellIndex--;
                if (itemSellIndex < 0)
                {
                    itemSellIndex = 0;
                }
                if (itemSellIndex >= ownedTreasure.Count)
                {
                    itemSellIndex = 0;
                }
                if (ownedTreasure.Count > 0)
                {
                    sellMenuText.text = $"Sell {ownedTreasure[itemSellIndex].GetName()} for {ownedTreasure[itemSellIndex].GetCost().ToString()} Scrap?";
                }
                else
                {
                    sellMenuText.text = "You have no treasure to sell";
                    nextItemButton.enabled = false;
                    sellButton.enabled = false;
                }
            }
        }
        
    }

    public void ToggleSellMenu()
    {
        if (sellMenu.activeInHierarchy)
        {
            sellMenu.SetActive(false);
            itemSellIndex = 0;
            sellButton.enabled = false;
            nextItemButton.enabled = false;
        }
        else
        {
            sellMenu.SetActive(true);
            if (ownedTreasure.Count > 0)
            {
                sellMenuText.text = $"Sell {ownedTreasure[itemSellIndex].GetName()} for {ownedTreasure[itemSellIndex].GetCost().ToString()} Scrap?";
                if(ownedTreasure.Count > 1)
                {
                    nextItemButton.enabled = true;
                }
                sellButton.enabled = true;
            }
            else
            {
                sellMenuText.text = "You have no treasure to sell";
            }
        }
    }

    public void ViewNextSellableItem()
    {
        itemSellIndex++;
        if(itemSellIndex >= ownedTreasure.Count)
        {
            itemSellIndex = 0;
        }
        sellMenuText.text = $"Sell {ownedTreasure[itemSellIndex].GetName()} for {ownedTreasure[itemSellIndex].GetCost().ToString()} Scrap?";
    }


}
