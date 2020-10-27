using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityPhysics 
{
    public static Vector2 GetForce(Vector2 Vf, Vector2 Vi, float mass, float dt)
    {
        //Vf = Vi + a*dt    ->      a = (Vf - Vi)/dt
        //F = mass*a        ->      a = F/mass
        //                  ->      F = mass(Vf - Vi)/dt
        return mass * (Vf - Vi) / dt;
    }

    public static float GetForce(float Vf, float Vi, float mass, float dt)
    {
        //Vf = Vi + a*dt    ->      a = (Vf - Vi)/dt
        //F = mass*a        ->      a = F/mass
        //                  ->      F = mass(Vf - Vi)/dt
        return mass * (Vf - Vi) / dt;
    }
}
