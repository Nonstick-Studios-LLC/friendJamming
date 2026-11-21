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

    public void EnterHover()
    {
        throw new System.NotImplementedException();
    }

    public void EnterInteract(PlayerController pc)
    {
        throw new System.NotImplementedException();
    }

    public void ExitHover()
    {
        throw new System.NotImplementedException();
    }

    public void ExitInteract()
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        if (itemData == null)
        {
            itemData = ShopManager.GetRandomItemData(Random.Range(int.MinValue, int.MaxValue));
        }
        GameObject visual = Instantiate(itemData.GetVisual(), this.gameObject.transform);
        visual.name = itemData.GetName();
        itemCostDisplay.text = "$" + itemData.GetCost().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
