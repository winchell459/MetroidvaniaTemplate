using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObject/Inventory")]
public class PlayerInventory : ScriptableObject
{
    public int YellowKeyCount = 0;

    public bool hasYellowKey {
        get
        {
            return useYellowKey();
        }
        set
        {
            if (value) YellowKeyCount += 1;
            else YellowKeyCount = 0;
        }
    }
    public bool hasDoubleJump;

    private void Awake()
    {
        Debug.Log("Invetory Added");

    }

    private bool useYellowKey()
    {
        if(YellowKeyCount > 0)
        {
            YellowKeyCount -= 1;
            return true;
        }
        else
        {
            return false;
        }
    }
}
