using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Player;

public class ObjectKeeper : MonoBehaviour
{
    [SerializeField] private float backPosY = 1f;
    [SerializeField] private float offsetToPlayer = 0.5f;

    private Transform playerT;

    private void Start()
    {
        PlayerInteractions playerInteractions = GameObject.FindObjectOfType<PlayerInteractions>();
        playerT = playerInteractions.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + ": droped from map");
        Transform t = other.gameObject.transform;
        Vector3 dir = playerT.position - t.position;
        dir.y = 0;
        dir.Normalize();
        t.position = new Vector3(t.position.x + dir.x * offsetToPlayer, backPosY, t.position.z + dir.z * offsetToPlayer);
    }
}
