using System;
using UnityEngine;

public static class PublicEvents
{
    public static Action<Vector2> MoveDirectionStarted;
    public static Action MoveDirectionStopped;

    public static Action<int> UpdateScrapCount;
}
