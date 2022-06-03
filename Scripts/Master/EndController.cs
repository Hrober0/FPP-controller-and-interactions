using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;


public class EndController : MonoBehaviour
{
    [SerializeField] private GameObject allUI;
    [SerializeField] private GameObject chaosScreen;
    [SerializeField] private AudioClip chaosSound;
    [SerializeField] private GameObject nonChaosScreen;
    [SerializeField] private AudioClip nonChaosSound;
    [SerializeField] private float volume = 1f;

    private void Start()
    {
        chaosScreen.SetActive(false);
        nonChaosScreen.SetActive(false);

        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        while (true)
        {
            if (!chaosScreen.activeSelf && !nonChaosScreen.activeSelf)
            {
                CheckIfChaos();
                CheckIfNoChaos();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void CheckIfChaos()
    {
        if (MessManager.instance != null)
        {
            foreach (Tool tool in MessManager.instance.CleaningTools)
            {
                if (MessManager.instance.NumberOfRemovedDirty(tool) < MessManager.instance.NumberOfAllDirty(tool))
                    return;
            }

            if (MessManager.instance.NumberOfObjectToPutBackAtTargetPlaces < MessManager.instance.NumberOfALLObjectToPutBack)
                return;
        }
        
        if (MousesManager.instance != null)
        {
            if (MousesManager.instance.NumberOfKilledMouses < MousesManager.instance.NumberOfAllMouses)
                return;
        }

        End(true);   
    }

    private void CheckIfNoChaos()
    {
        if (MistakesController.instance != null)
        {
            if (MistakesController.instance.CurrMistakes < MistakesController.instance.MaxMistakes)
                return;
        }

        End(false);
    }

    private void End(bool Chaos)
    {
        if (Chaos)
        {
            Debug.Log("Win!");
            nonChaosScreen.SetActive(true);
            AudioManager.instance.PlaySound(new Sound().GlobalSound(nonChaosSound, volume));
        }
        else
        {
            Debug.Log("Lose!");
            chaosScreen.SetActive(true);
            AudioManager.instance.PlaySound(new Sound().GlobalSound(chaosSound, volume));
        }

        Cursor.lockState = CursorLockMode.None;
        allUI.SetActive(false);
    }
}
