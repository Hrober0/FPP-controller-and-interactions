using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI.Menu
{
    public class UIMainMenu : MonoBehaviour
    {
        private VisualElement _optionsMenu;

        private void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            Button startButton = root.Q<Button>("ESC_Play");
            startButton.clicked += StartButtonPressed;

            Button optionsButton = root.Q<Button>("ESC_Option");
            optionsButton.clicked += OptionButtonPressed;

            Button exitButton = root.Q<Button>("ESC_Exit");
            exitButton.clicked += ExitButtonPressed;

            _optionsMenu = root.Q<VisualElement>("UI_MainMenu_Options");
            UIMethods.SetElementActive(_optionsMenu, false);
        }


        private void StartButtonPressed()
        {
            Debug.Log("Loading scene: " + "Game");
            SceneManager.LoadScene("Game");
        }

        private void OptionButtonPressed()
        {
            UIMethods.SetElementActive(_optionsMenu, !UIMethods.IsElementActive(_optionsMenu));
        }

        private void ExitButtonPressed()
        {
            Debug.Log("Exit game");
            Application.Quit();
        }
    }
}