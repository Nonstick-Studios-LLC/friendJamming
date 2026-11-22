using TMPro;
using UnityEngine;

public class BatterySlot : MonoBehaviour, IInteractable
{
    [SerializeField] private int batterySlotID;
    public OxygenBattery CurrentBattery;

    [SerializeField] private TextMeshProUGUI uiPopUp;
    [SerializeField] private ShipOxygenStorage storage;
    [SerializeField] private ItemScriptable allowedItems;
    [SerializeField] private Transform snapPosition;

    private void Awake()
    {
        CurrentBattery.GetComponent<Collider>().enabled = false;
        CurrentBattery.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void EnterHover()
    {
        Debug.Log("Hif thkl");
        if(CurrentBattery != null)
        {
            Debug.Log($"Eject battery " + batterySlotID);
        }
        else if(CurrentBattery == null)
        {
            Debug.Log($"Insert battery " + batterySlotID);
        }
    }

    public void EnterInteract(PlayerController pc)
    {
        if (CurrentBattery != null)
        {
            CurrentBattery.EnterInteract(pc);
            CurrentBattery = null;
        }

        //If slot is empty and player is not holding battery do nothing

        //If slot is empty and player is holding battery "INSERT BATTERY"

        UpdateUIPopUp();
        UpdateOxygenStorage();
    }

    public void ExitHover()
    {
        //Turn off current UI pop-up
    }

    public void ExitInteract()
    {
        
    }

    /// <summary>
    /// Updates the UI pop up manually to handle required changes due to interaction.
    /// </summary>
    private void UpdateUIPopUp()
    {
        EnterHover();
    }

    private void UpdateOxygenStorage()
    {
        storage.batteries[batterySlotID - 1] = CurrentBattery;
    }
}
