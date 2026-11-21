using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyDisplay;
    [SerializeField] private GameObject sellMenu;
    [SerializeField] private TMP_Text sellMenuText;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button nextItemButton;
    [SerializeField] private GameObject buyMenu;
    [SerializeField] private Button buyButton;

    [SerializeField] private TMP_Text buyMenuText;

    [SerializeField] private List<ItemScriptable> personalTreasure;
    private ItemScriptable heldData;
    private int localScrapReference = 0;
    private PlayerController pc;

    int itemSellIndex = 0;


    private void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        PublicEvents.UpdateScrapCount += UpdateScrapUI;
        PublicEvents.StartBuyInteraction += ToggleBuyMenu;
        PublicEvents.StartSellInteraction += ToggleSellMenu;
    }

    private void OnDisable()
    {
        PublicEvents.UpdateScrapCount -= UpdateScrapUI;
    }


    private void UpdateScrapUI(int scrapCount)
    {
        localScrapReference = scrapCount;
        currencyDisplay.text = $"Scrap: {scrapCount.ToString()}";
    }


    #region Selling

    [Button]
    public void SellItem()
    {
        if (personalTreasure.Count > 0)
        {
            if (!personalTreasure[itemSellIndex].GetMcGuffin())
            {
                localScrapReference += personalTreasure[itemSellIndex].GetSellPrice();
                personalTreasure.Remove(personalTreasure[itemSellIndex]);
                PublicEvents.UpdateScrapCount?.Invoke(localScrapReference);

                itemSellIndex--;
                if (itemSellIndex < 0)
                {
                    itemSellIndex = 0;
                }
                if (itemSellIndex >= personalTreasure.Count)
                {
                    itemSellIndex = 0;
                }
                if (personalTreasure.Count > 0)
                {
                    sellMenuText.text = $"Sell {personalTreasure[itemSellIndex].GetName()} for {personalTreasure[itemSellIndex].GetCost().ToString()} Scrap?";
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
            Cursor.visible = false;
            pc.CanMove = true;
            sellMenu.SetActive(false);
            itemSellIndex = 0;
            sellButton.enabled = false;
            nextItemButton.enabled = false;
        }
        else
        {
            Cursor.visible = true;
            pc.CanMove = false;
            sellMenu.SetActive(true);
            if (personalTreasure.Count > 0)
            {
                sellMenuText.text = $"Sell {personalTreasure[itemSellIndex].GetName()} for {personalTreasure[itemSellIndex].GetSellPrice().ToString()} Scrap?";
                if (personalTreasure.Count > 1)
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
        if (itemSellIndex >= personalTreasure.Count)
        {
            itemSellIndex = 0;
        }
        sellMenuText.text = $"Sell {personalTreasure[itemSellIndex].GetName()} for {personalTreasure[itemSellIndex].GetSellPrice().ToString()} Scrap?";
    }

    #endregion

    #region Buying

    public void ToggleBuyMenu(ItemScriptable data = null)
    {
        if (buyMenu.activeInHierarchy)
        {
            Cursor.visible = false;
            pc.CanMove = true;
            buyMenu.SetActive(false);
            buyButton.enabled = false;
        }
        else
        {
            Cursor.visible = true;
            pc.CanMove = false;
            heldData = data;
            buyMenu.SetActive(true);
            if (data.GetCost() <= localScrapReference)
            {
                buyButton.enabled = true;
                buyMenuText.text = $"Buy {data.GetName()} for {data.GetCost().ToString()} Scrap?";
            }
            else
            {
                buyMenuText.text = $"You are missing {((int)Mathf.Abs(data.GetCost() - localScrapReference)).ToString()} Scrap to buy {data.GetName()}";
            }
        }
    }

    public void BuyItem()
    {
        if (heldData.GetCost() <= localScrapReference)
        {
            localScrapReference -= heldData.GetCost();
            currencyDisplay.text = "Scrap: $" + localScrapReference.ToString();
            PublicEvents.UpdateScrapCount?.Invoke(localScrapReference);
            personalTreasure.Add(heldData);

            if (heldData.GetMcGuffin())
            {
                Debug.Log("McGuffin Obtained. Add logic to end game");
                heldData = null;
                return;
            }


            heldData = null;
            PublicEvents.ItemBought?.Invoke();

            PublicEvents.ItemBought = null;

            ToggleBuyMenu();




        }
    }

    #endregion
}
