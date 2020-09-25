using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowLock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && FindObjectOfType<PlayerHandler>().Inventory.hasYellowKey)
        {
            Destroy(gameObject);
        }
    }


}
