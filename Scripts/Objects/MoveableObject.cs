using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveableObject : LookableObjects
{
    [SerializeField] private bool canPitch = true;
    public bool CanPitch => canPitch;
}