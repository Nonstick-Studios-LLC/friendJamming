using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

// thank you sky

public class PeriscopeController : MonoBehaviour, IInteractable
{
    [SerializeField] CinemachineCamera periscopeCam;
    [SerializeField] CinemachineCamera playerCam;

    GameObject periscopeCamRotationParent;
    [SerializeField] float rotationSpeed;

    bool isRotating = false;
    Coroutine rotationCoroutine;

    #region INTERACTABLE

    public void EnterInteract(PlayerController pc)
    {
        ChangeCamera();

        PublicEvents.MoveDirectionStarted += RotatePeriscopeStart;
        PublicEvents.MoveDirectionStopped += RotatePeriscopeStop;
    }

    public void ExitInteract()
    {
        ChangeCamera();

        PublicEvents.MoveDirectionStarted -= RotatePeriscopeStart;
        PublicEvents.MoveDirectionStopped -= RotatePeriscopeStop;

        isRotating = false;
        rotationCoroutine = null;
    }

    public void EnterHover()
    {

    }

    public void ExitHover()
    {

    }

    #endregion INTERACTABLE

    void ChangeCamera()
    {
        if(!periscopeCam.gameObject.activeSelf)
        {
            playerCam.gameObject.SetActive(false);
            periscopeCam.gameObject.SetActive(true);
        }
        else
        {
            periscopeCam.gameObject.SetActive(false);
            playerCam.gameObject.SetActive(true);
        }

    }

    void RotatePeriscopeStart(Vector2 dir)
    {
        if (!gameObject.activeSelf || rotationCoroutine != null)
        {
            return;
        }

        isRotating = true;

        if (dir.x < 0)
        {
            rotationCoroutine = StartCoroutine(RotatePeriscope(-1));
        }
        else if (dir.x > 0)
        {
            rotationCoroutine = StartCoroutine(RotatePeriscope(1));
        }
    }

    IEnumerator RotatePeriscope(int dir)
    {
        while(isRotating)
        {
            periscopeCamRotationParent.transform.Rotate(new Vector3(0, dir * rotationSpeed, 0) * Time.deltaTime);
            yield return null;
        }

        rotationCoroutine = null;
    }

    void RotatePeriscopeStop()
    {
        isRotating = false;
        rotationCoroutine = null;
    }
}
