using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI.Menu
{
    public class UIEscapeMenu : MonoBehaviour
    {
        [SerializeField] private bool active = false;

        private VisualElement escapeMenuElement;
        private CursorLockMode? offCursorLockMode = null;

        private VisualElement optionsMenu;

        private UIControls controls;

        private void Awake()
        {
            controls = new UIControls();

            UIControls.EscapeMenuActions actions = controls.EscapeMenu;
            actions.ChangeActive.performed += _ => { SetEnable(!active); };

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            escapeMenuElement = root.Q<VisualElement>("EscapeMenu");

            Button resumeButton = escapeMenuElement.Q<Button>("ESC_Resume");
            resumeButton.clicked += ReasumeButtonPressed;

            Button restartButton = escapeMenuElement.Q<Button>("ESC_Restart");
            restartButton.clicked += RestartButtonPressed;

            Button optionsButton = root.Q<Button>("ESC_Option");
            optionsButton.clicked += OptionButtonPressed;

            Button exitButton = escapeMenuElement.Q<Button>("ESC_Exit");
            exitButton.clicked += ExitButtonPressed;

            optionsMenu = root.Q<VisualElement>("UI_MainMenu_Options");
        }

        private void Start()
        {
            SetEnable(active);
        }

        private void OnEnable() => controls.Enable();

        private void OnDisable() => controls.Disable();

        private void SetEnable(bool value)
        {
            active = value;
            escapeMenuElement.visible = value;
            if (value)
            {
                offCursorLockMode = UnityEngine.Cursor.lockState;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
            }
            else
            {
                if (offCursorLockMode.HasValue)
                    UnityEngine.Cursor.lockState = offCursorLockMode.Value;
                Time.timeScale = 1f;

                UIMethods.SetElementActive(optionsMenu, false);
            }    
        }

        private void ReasumeButtonPressed()
        {
            SetEnable(false);
        }

        private void RestartButtonPressed()
        {
            Debug.Log("Loading scene: " + "Game");
            SceneManager.LoadScene("Game");
        }

        private void OptionButtonPressed()
        {
            UIMethods.SetElementActive(optionsMenu, !UIMethods.IsElementActive(optionsMenu));
        }

        private void ExitButtonPressed()
        {
            Debug.Log("Loading scene: " + "Main Menu");
            SceneManager.LoadScene("Main Menu");
        } 
    }
}