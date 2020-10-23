using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObject/Inventory")]
public class PlayerInventory : ScriptableObject
{
    public List<AbilityItem> Accessories = new List<AbilityItem>();
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
    //public bool hasDoubleJump;


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

    public void AddAccessory(AbilityItem item)
    {
        //changed day 5 adding condition
        if(item.isUnique && !Accessories.Contains(item) || !item.isUnique) Accessories.Add(item);
    }

    public bool UseAccessory(AbilityItem.AbilityTypes type)
    {
        foreach( AbilityItem ability in Accessories)
        {
            if(type == ability.AbilityType)
            {
                return ability.canUse;
            }
        }
        return false;
    }

    public void ResetAccessory(AbilityItem.AbilityTypes type)
    {
        foreach (AbilityItem ability in Accessories)
        {
            if (type == ability.AbilityType)
            {
                //Debug.Log("Reset " + type);
                ability.ResetUse();
            }
        }
    }
}
