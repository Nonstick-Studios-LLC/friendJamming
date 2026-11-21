/*
 * Does not handle any player input. Other scripts need to call these functions
 */

using UnityEngine;

public class SubmarineInputHandler : MonoBehaviour
{
    [Header("Designer Settings")]
    [SerializeField] private float sidePropellersForwardSpeed = 5;
    [SerializeField] private float backPropellerForwardSpeed = 5;

    [SerializeField] private float horizontalTurnScalar = 10; 
    [SerializeField] private float damagedHorizontalTurnScalar = 2.5f;

    [Header("Ship Parts")]
    [SerializeField] Propeller leftPropeller;
    [SerializeField] Propeller rightPropeller;
    [SerializeField] Propeller backPropeller;

    [Header("Debug Values")]
    [SerializeField] private float currentHorizontalTurnSpeed;

}
