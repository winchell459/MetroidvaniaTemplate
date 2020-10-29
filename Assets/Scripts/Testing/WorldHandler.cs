using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldHandler : MonoBehaviour
{
    Vector2 gravityDirection;
    Vector2 gravity;
    private void Awake()
    {
        gravity = Physics2D.gravity;
        gravityDirection = gravity.normalized;
    }
    public void FlipGravity(Vector2 flipDirection)
    {

    }
}
