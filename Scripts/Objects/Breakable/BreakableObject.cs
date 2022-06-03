using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MoveableObject
{
    [SerializeField] private float forceNeedToDestroy = 4f;

    private void OnCollisionEnter(Collision collision)
    {
        float force = collision.relativeVelocity.magnitude;

        //Debug.Log("Droped from: " + force);

        if (force >= forceNeedToDestroy)
            Destroy(gameObject);
    }
}
