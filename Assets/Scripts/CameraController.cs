using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Smoothing = 1f;
    public Transform Target;
    public Vector3 Offset;
    // Start is called before the first frame update
    void Start()
    {
        Offset = Target.position - transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 destination = Vector2.MoveTowards(transform.position, Target.position, Smoothing * Time.deltaTime);// - (Vector2)Offset;
        transform.position = new Vector3(destination.x, destination.y, transform.position.z);// - Offset;
    }
}
