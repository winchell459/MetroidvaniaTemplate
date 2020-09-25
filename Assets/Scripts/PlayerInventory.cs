using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObject/Inventory")]
public class PlayerInventory : ScriptableObject
{
    public bool hasYellowKey;
    public bool hasDoubleJump;

    private void Awake()
    {
        Debug.Log("Invetory Added");

    }
}
