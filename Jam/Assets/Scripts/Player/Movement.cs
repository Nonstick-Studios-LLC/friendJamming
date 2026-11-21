/*************************************************
Author Names        :   Cade Naylor
Date Created        : 	11/21/2026
Brief Description   : 	Sets up base functions movement types should inherit

External Resources :    	
***************************************************/

using UnityEngine;

public abstract class Movement: MonoBehaviour 
{
    /// <summary>
    /// Subscribes to all the Public events for controls
    /// </summary>
    protected virtual void OnEnable()
    {
        PublicEvents.MoveDirection += OnMove;
        PublicEvents.MoveDirectionStopped += OnMoveEnd;
        PublicEvents.MousePosition += OnMouseMove;
        PublicEvents.InteractStarted += OnInteractStart;
        PublicEvents.InteractCancelled += OnInteractEnd;
        PublicEvents.SpaceStarted += OnSpaceStarted;
        PublicEvents.SpaceEnded += OnSpaceFinished;
        PublicEvents.ShiftStarted += OnShiftStarted;
        PublicEvents.ShiftEnded += OnShiftFinished;

        PublicEvents.LeftClickStarted += OnLeftClickStarted;
        PublicEvents.LeftClickEnded += OnLeftClickFinished;
    }

    /// <summary>
    /// Unsubscribes from all public events for controls
    /// </summary>
    protected virtual void OnDisable()
    {
        PublicEvents.MoveDirection -= OnMove;
        PublicEvents.MoveDirectionStopped -= OnMoveEnd;
        PublicEvents.MousePosition -= OnMouseMove;
        PublicEvents.InteractStarted -= OnInteractStart;
        PublicEvents.InteractCancelled -= OnInteractEnd;
        PublicEvents.SpaceStarted -= OnSpaceStarted;
        PublicEvents.SpaceEnded -= OnSpaceFinished;
        PublicEvents.ShiftStarted -= OnShiftStarted;
        PublicEvents.ShiftEnded -= OnShiftFinished;

        PublicEvents.LeftClickStarted -= OnLeftClickStarted;
        PublicEvents.LeftClickEnded -= OnLeftClickFinished;
    }


    protected abstract void OnMove(Vector2 moveVector);
    protected abstract void OnMoveEnd();
    protected abstract void OnMouseMove(Vector2 cameraVector);
    protected abstract void OnInteractStart();
    protected abstract void OnInteractEnd();
    protected abstract void OnSpaceStarted();
    protected abstract void OnSpaceFinished();
    protected abstract void OnShiftStarted();
    protected abstract void OnShiftFinished();

    protected abstract void OnLeftClickStarted();
    protected abstract void OnLeftClickFinished();

    protected abstract bool LookingAtObject();



}
