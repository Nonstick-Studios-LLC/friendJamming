using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class EndStateManager : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [SerializeField] private TMP_Text loseText;

    private void Awake()
    {
        PublicEvents.CallEndState += ShowEndState;
    }
    private void OnDestroy()
    {
        PublicEvents.CallEndState -= ShowEndState;
    }

    public enum EndType
    {
        Next_Area, Ship_Oxygen_Depletion, Personal_Oxygen_Depletion, No_Health
    }


    public void ShowEndState(EndType type)
    {
        FindFirstObjectByType<PlayerController>().CanMove = false;
        if(type == EndType.Next_Area)
        {
            winScreen.SetActive(true);
        }
        else
        {
            switch (type)
            {
                case EndType.Next_Area:
                    break;
                case EndType.Ship_Oxygen_Depletion:
                    loseText.text = "You ran out of ship oxygen. Your entire crew perishes, never to be found again.";
                    break;
                case EndType.Personal_Oxygen_Depletion:
                    loseText.text = "You ran out of oxygen. Technology has not grown enough to support revival and reincarnation.";
                    break;
                case EndType.No_Health:
                    loseText.text = "You ran out of health. Technology has not grown enough to support revival and reincarnation.";
                    break;
                default:
                    break;
            }
            loseScreen.SetActive(true);
        }




    }
}