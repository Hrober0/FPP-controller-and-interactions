using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Sounds
{
    public class UIAddSoundsToButtons : MonoBehaviour
    {
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip hoverSound;

        private AudioSource audioSource;

        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            List<VisualElement> allElements = GetAllChilds(root);
            foreach (VisualElement element in allElements)
            {
                if (element is Button)
                {
                    element.RegisterCallback<MouseUpEvent>(e => PlayClickSound());
                    element.RegisterCallback<MouseEnterEvent>( e => PlayHoverSound());
                }
            }
        }

        private List<VisualElement> GetAllChilds(VisualElement element)
        {
            List<VisualElement> childs = new List<VisualElement>();
            if (null == element)
            {
                return childs;
            }

            for (int i = 0; i < element.childCount; i++)
            {
                VisualElement child = element.ElementAt(i);
                childs.Add(child);
                childs.AddRange(GetAllChilds(child));
            }
            return childs;
        }

        private void PlayClickSound()
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }

        private void PlayHoverSound()
        {
            audioSource.clip = hoverSound;
            audioSource.Play();
        }
    }
}