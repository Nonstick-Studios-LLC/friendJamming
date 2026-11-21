using UnityEngine;

public class SRPAdditionalRenderData : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInputManager.Instance.secrets.performed += Secrets_performed;
    }

    private void Secrets_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.LogFormat("<size=2><color=grey>Wouldn't you like to know, weather boy?</color></size><size=30>(:</size>");
    }

}
