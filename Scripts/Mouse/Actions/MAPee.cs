using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAPee : MonoBehaviour, IMouseAction
{
    [SerializeField] [Range(0, 100)] private int chance = 50;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float time = 0.5f;
    [SerializeField] private float size = 0.5f;
    [SerializeField] private float randomDeformation = 0.2f;

    public void TriggerAction(MouseBehavior mouse)
    {
        if (Random.Range(0, 100) <= chance)
        {
            if (!Dirty.IsSpaceForDirty(transform.position, 0.5f * Vector3.one))
                return;

            if (MistakesController.instance != null)
                MistakesController.instance.AddMistake();

            Dirty.CreateLiquid(prefab, transform.position, size, randomDeformation, time);
            mouse.Wait(time);
        }
    }
}
