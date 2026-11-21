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

    [Foldout("Visuals"), SerializeField, InfoBox("True if this item has a unique model. False if it is a probuilder torus")] private bool hasUniqueModel;
    private bool usesProbuilder => !hasUniqueModel;
    [Foldout("Visuals"), SerializeField, ShowIf(nameof(hasUniqueModel))] private Mesh[] modelMeshes;
    [Foldout("Visuals"), SerializeField, ShowIf(nameof(hasUniqueModel))] private Material[] modelMaterials;
    [Foldout("Visuals"), SerializeField, ShowIf(nameof(usesProbuilder))] private int numberOfSides;
    [Foldout("Visuals"), SerializeField, ShowIf(nameof(usesProbuilder))] private Material appliedMaterial;

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

    public bool GetVisualType()
    {
        return hasUniqueModel;
    }

    public Mesh[] GetAllMeshes()
    {
        return modelMeshes;
    }

    public Material[] GetAllMaterials()
    {
        return modelMaterials;
    }

    public int GetSideCount()
    {
        return numberOfSides;
    }

    public Material GetSingleMaterial()
    {
        return appliedMaterial;
    }







}
