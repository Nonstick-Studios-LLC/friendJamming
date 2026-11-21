using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;

public class ShopItem : MonoBehaviour, IInteractable
{
    [SerializeField] private TMP_Text itemCostDisplay;
    [SerializeField, Tooltip("Can be left null to instantiate a random item")] private ItemScriptable itemData = null;
    int savedScrapCount = 0;
    public void EnterHover()
    {
        return;
    }

    public void EnterInteract(PlayerController pc)
    {
        throw new System.NotImplementedException();
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
        if(itemData == null)
        {
            savedScrapCount = newAmount;
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
