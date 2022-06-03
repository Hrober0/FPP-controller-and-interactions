using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Audio;

namespace UI.Gameplay.List
{
    public class UITaskList : MonoBehaviour
    {
        [Header("Tools")]
        private Label dustingLabel;
        [SerializeField] private Tool dustingTool;
        private Label moppingLabel;
        [SerializeField] private Tool moppingTool;

        [Header("Sounds")]
        [SerializeField] private AudioClip upPercentSound;
        [SerializeField] private AudioClip downPercentSound;

        private Label movingBoxesLabel;
        private Label killingRatsLabel;

        private int oldDustingPercent = -1;
        private int oldMoppingPercent = -1;
        private int oldMovingBoxesPercent = -1;
        private int oldKilledRats = -1;

        private void Awake()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            dustingLabel = root.Q<Label>("Dusting_Precent");
            moppingLabel = root.Q<Label>("Mopping_Precent");
            movingBoxesLabel = root.Q<Label>("MovingBoxes_Precent");
            killingRatsLabel = root.Q<Label>("KillingRats_Precent");

            InvokeRepeating(nameof(UpdateList), 0.1f, 1f);
        }

        
        private void UpdateList()
        {
            int dustingPercent = Mathf.RoundToInt(MessManager.instance.NumberOfRemovedDirty(dustingTool) / (float)MessManager.instance.NumberOfAllDirty(dustingTool) * 100);
            PlaySoundIfNotEqual(oldDustingPercent, dustingPercent);
            oldDustingPercent = dustingPercent;
            dustingLabel.text = dustingPercent + "%";

            int moppingPercent = Mathf.RoundToInt(MessManager.instance.NumberOfRemovedDirty(moppingTool) / (float)MessManager.instance.NumberOfAllDirty(moppingTool) * 100);
            PlaySoundIfNotEqual(oldMoppingPercent, moppingPercent);
            oldMoppingPercent = moppingPercent;
            moppingLabel.text = moppingPercent + "%";

            int movingBoxesPercent = Mathf.RoundToInt(MessManager.instance.NumberOfObjectToPutBackAtTargetPlaces / (float)MessManager.instance.NumberOfALLObjectToPutBack * 100);
            PlaySoundIfNotEqual(oldMovingBoxesPercent, movingBoxesPercent);
            oldMovingBoxesPercent = movingBoxesPercent;
            movingBoxesLabel.text = movingBoxesPercent + "%";

            int killedRats = MousesManager.instance.NumberOfKilledMouses;
            int allRats = MousesManager.instance.NumberOfAllMouses;
            PlaySoundIfNotEqual(oldKilledRats, killedRats);
            oldKilledRats = killedRats;
            killingRatsLabel.text = killedRats + "/" + allRats;
        }


        private void PlaySoundIfNotEqual(int oldV, int newV)
        {
            if (AudioManager.instance == null)
                return;

            if (oldV != -1 && oldV != newV)
            {
                if (newV > oldV)
                    AudioManager.instance.PlaySound(new Sound().GlobalSound(upPercentSound, 1));
                else
                    AudioManager.instance.PlaySound(new Sound().GlobalSound(downPercentSound, 1));
            }
        }
    }
}