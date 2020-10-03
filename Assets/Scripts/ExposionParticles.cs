using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExposionParticles : MonoBehaviour
{
    ParticleSystem ps;

    int colliderCount = 0;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        colliderCount = tilemaps.Length;
        for(int i = 0; i < colliderCount; i += 1)
        {
            ps.trigger.SetCollider(i, tilemaps[i].GetComponent<Collider2D>());
        }

    }
}
