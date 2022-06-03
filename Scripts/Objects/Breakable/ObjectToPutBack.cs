using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToPutBack : BreakableObject
{
    [SerializeField] private GameObject placeToPut;
    public GameObject PlaceToPut => placeToPut;

    private bool wasOnTargetPlace = false;
    private bool isOnTargetPlace = false;
    public bool IsOnTargetPlace => isOnTargetPlace;

    private PlacingPlace placingPlace;

    private void Start()
    {
        if (placeToPut == null)
            Debug.LogError(name + ": placeToPut is null!");

        placingPlace = placeToPut.GetComponent<PlacingPlace>();

        MessManager.instance.AddObjectToPutBack(this);
    }

    private void OnDestroy()
    {
        if (MessManager.instance != null)
            MessManager.instance.RemoveObjectToPutBack(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == placingPlace.Space)
        {
            wasOnTargetPlace = true;
            isOnTargetPlace = true;

            if (MistakesController.instance != null)
                MistakesController.instance.RemoveMistake();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == placingPlace.Space)
        {
            isOnTargetPlace = false;
            if (wasOnTargetPlace && MistakesController.instance != null)
                MistakesController.instance.AddMistake();
        } 
    }
}
