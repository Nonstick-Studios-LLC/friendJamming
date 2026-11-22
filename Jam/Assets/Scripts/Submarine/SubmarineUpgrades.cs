using UnityEngine;
using NaughtyAttributes;

public class SubmarineUpgrades : Singleton<SubmarineUpgrades>
{
    public bool McGuffinObtained = false;

    [Button]
    public void EnableMcGuffin()
    {
        McGuffinObtained = true;
    }
}
