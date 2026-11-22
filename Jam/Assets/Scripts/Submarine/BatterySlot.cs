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
    bool holdingBattery = false;

    private void Awake()
    {
        CurrentBattery.GetComponent<Collider>().enabled = false;
        CurrentBattery.GetComponent<Rigidbody>().isKinematic = true;
        CurrentBattery.transform.parent = snapPosition;
        CurrentBattery.transform.localPosition = Vector3.zero;

        PublicEvents.HoldingBattery += SetBatteryStatus;
    }

    private void SetBatteryStatus(bool b)
    {
        holdingBattery = b;
    }

    public void EnterHover()
    {
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

        if(CurrentBattery == null && holdingBattery)
        {

        }

        UpdateUIPopUp();
        UpdateOxygenStorage();
    }

    public void Placed(OxygenBattery ob)
    {
        CurrentBattery = ob;
        CurrentBattery.GetComponent<Collider>().enabled = false;
        CurrentBattery.GetComponent<Rigidbody>().isKinematic = true;
        CurrentBattery.transform.parent = snapPosition;
        CurrentBattery.transform.localPosition = Vector3.zero;

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
