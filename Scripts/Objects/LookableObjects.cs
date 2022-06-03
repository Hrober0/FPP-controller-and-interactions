using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookableObjects : MonoBehaviour
{
    public const int layer = 8;

    private void Start()
    {
        name = this.GetType().Name;
        gameObject.layer = layer;
    }
}
