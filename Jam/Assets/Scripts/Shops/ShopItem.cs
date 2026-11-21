using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour, IInteractable
{
    [SerializeField] private TMP_Text itemCostDisplay;
    [SerializeField, Tooltip("Can be left null to instantiate a random item")] private ItemScriptable itemData = null;
    int savedScrapCount = 0;
    private Coroutine spawning;
    public void EnterHover()
    {
        return;
    }

    [Button]
    public void EnterInteract()
    {
        EnterInteract(null);
    }
    public void EnterInteract(PlayerController pc)
    {
        PublicEvents.StartBuyInteraction?.Invoke(itemData);
        PublicEvents.ItemBought += OnItemBought;
    }

    public void ExitHover()
    {
        return;
    }

    public void ExitInteract()
    {
        throw new System.NotImplementedException();
    }

    private void OnEnable()
    {
        PublicEvents.UpdateScrapCount += UpdatePriceColor;
    }

    private void OnDisable()
    {
        PublicEvents.UpdateScrapCount -= UpdatePriceColor;
    }

    public void OnItemBought()
    {
        PublicEvents.ItemBought -= OnItemBought;
        if(spawning != null)
        {
            return;
        }
        GameObject childVisual = transform.GetChild(1).gameObject;
        Destroy(childVisual);
        itemCostDisplay.text = "";
        spawning = StartCoroutine(GetNewItem());

    }


    private IEnumerator GetNewItem()
    {
        yield return new WaitForSeconds(3);

        itemData = ShopManager.GetRandomItemData(Random.Range(int.MinValue, int.MaxValue));
        GameObject visual = Instantiate(itemData.GetVisual(), this.gameObject.transform);
        visual.name = itemData.GetName();
        Debug.Log("Saved scrap is " + savedScrapCount);
        UpdatePriceColor(savedScrapCount);
        itemCostDisplay.text = "$" + itemData.GetCost().ToString();
        spawning = null;
    }

    void Start()
    {
        if (itemData == null)
        {
            itemData = ShopManager.GetRandomItemData(Random.Range(int.MinValue, int.MaxValue));
        }
        GameObject visual = Instantiate(itemData.GetVisual(), this.gameObject.transform);
        visual.name = itemData.GetName();
        UpdatePriceColor(savedScrapCount);
        itemCostDisplay.text = "$" + itemData.GetCost().ToString();
    }

    private void UpdatePriceColor(int newAmount)
    {
        savedScrapCount = newAmount;
        if(itemData == null)
        {
            return;
        }
        if(newAmount >= itemData.GetCost())
        {
            itemCostDisplay.color = Color.greenYellow;
        }
        else
        {
            itemCostDisplay.color = Color.indianRed;
        }
    }
}
