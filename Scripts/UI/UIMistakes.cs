using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Gameplay.Mistakes
{
    public class UIMistakes : MonoBehaviour
    {
        private VisualElement holdBar;

        private void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            holdBar = root.Q<VisualElement>("BarBackground");

            InvokeRepeating(nameof(Set), 0.1f, 1f);
        }

        private void Set()
        {
            if (MistakesController.instance != null)
            {
                float currM = MistakesController.instance.CurrMistakes;
                float maxM = MistakesController.instance.MaxMistakes;
                float percent = Mathf.Clamp(currM / maxM, 0, 1);

                VisualElement progress = holdBar.Q<VisualElement>("BarFill");
                float endWidth = progress.parent.worldBound.width - 10f;
                progress.style.width = endWidth * percent;
            }
        }
    }
}