/*************************************************
Author Names        : 		Cade Naylor, 
Date Created        : 		11/20/2026
Brief Description   : 		Scriptable Object base for any item the player can pick up
                            Has an assortment of getters
External Resources  : 	    Nonme
***************************************************/

using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Treasure", menuName = "Treasure")]
public class ItemScriptable : ScriptableObject
{
    [Foldout("Item Statistics"), SerializeField] private string itemName;
    [Foldout("Item Statistics"), SerializeField] private int sellPrice;
    [Foldout("Item Statistics"), SerializeField] private int purchasePrice;
    [Foldout("Item Statistics"), SerializeField] private bool isMcGuffin;

   
    [Foldout("Visuals"), SerializeField] private GameObject visual;

    // Getters. No need to modify these at runtime
    public string GetName()
    {
        return name;
    }

    public int GetSellPrice()
    {
        return sellPrice;
    }

    public int GetCost()
    {
        return purchasePrice;
    }

    public bool GetMcGuffin()
    {
        return isMcGuffin;
    }

    /// <summary>
    /// Returns true if it uses a unique model. Returns false for a probuilder torus
    /// </summary>
    /// <returns>returns visual style</returns>
    public GameObject GetVisual()
    {
        return visual;
    }

}
