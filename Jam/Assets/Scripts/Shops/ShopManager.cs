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
    /*
    [SerializeField] private TMP_Text currencyDisplay;
    [SerializeField] private GameObject sellMenu;
    [SerializeField] private TMP_Text sellMenuText;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button nextItemButton;
    [SerializeField] private GameObject buyMenu;
    [SerializeField] private Button buyButton;

    [SerializeField] private TMP_Text buyMenuText;*/
    [Header("Testing")]
    //[SerializeField] private List<ItemScriptable> ownedTreasure;
    private ItemScriptable heldData;

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



}
