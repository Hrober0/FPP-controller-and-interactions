using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI.Gameplay.End
{
    public class UIEndScrean : MonoBehaviour
    {
        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            Button restartButton = root.Q<Button>("RestartButton");
            restartButton.clicked += RestartButtonPressed;
        }

        void RestartButtonPressed()
        {
            Debug.Log("Loading scene: " + "Game");
            SceneManager.LoadScene("Game");
        }
    }
}