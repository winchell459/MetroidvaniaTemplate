using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObjectTheCube : GravityObject
{
    private Transform[] isGroundedCheckers;
    public int gravityFlipped = 1;
    private void Start()
    {
        isGroundedCheckers = GetComponent<Platformer>().isGroundedChecker;
    }
    public override void FlipGravity()
    {
        gravityFlipped *= -1;
        foreach (Transform checker in isGroundedCheckers)
        {
            checker.localPosition = new Vector3(checker.localPosition.x, -checker.localPosition.y, 0);
        }
    }
}
