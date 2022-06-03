using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
using Audio;
using Player;

namespace UI.Options
{
    public class UIOptions : MonoBehaviour
    {
        [SerializeField] [Range(0f, 1f)] private float defaultMusicLoudnes = 0.2f;
        [SerializeField] [Range(0f, 1f)] private float defaulSoundsLoudnes = 0.75f;
        [SerializeField] [Range(40f, 90f)] private float defaultFOV = 60f;
        [SerializeField] [Range(0.1f, 2f)] private float defaulMouseSensitivity = 0.55f;


        private readonly List<SliderObject> slidersObjects = new List<SliderObject>();

        private static UIOptions instance;
        private void Start()
        {
            if (instance == null)
                instance = this;

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            slidersObjects.Add(new SliderObject(root.Q<Slider>("O_ML_Slider"), root.Q<Label>("O_ML_Value"), "musicLoudnes", defaultMusicLoudnes, true, (float value) =>
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.SetMiusicVolume(value);
            }));

            slidersObjects.Add(new SliderObject(root.Q<Slider>("O_SL_Slider"), root.Q<Label>("O_SL_Value"), "efectLoudnes", defaulSoundsLoudnes, true, (float value) =>
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.SetSoundsVolume(value);
            }));

            slidersObjects.Add(new SliderObject(root.Q<Slider>("O_FOV_Slider"), root.Q<Label>("O_FOV_Value"), "fieldOfView", defaultFOV, false, (float value) =>
            {
                PlayerInputManager playerInputManager = FindObjectOfType<PlayerInputManager>();
                if (playerInputManager != null)
                    playerInputManager.SetCameraFOV(value);
            }));

            slidersObjects.Add(new SliderObject(root.Q<Slider>("O_MS_Slider"), root.Q<Label>("O_MS_Value"), "mouseSensitivity", defaulMouseSensitivity, true, (float value) =>
            {
                PlayerInputManager playerInputManager = FindObjectOfType<PlayerInputManager>();
                if (playerInputManager != null)
                    playerInputManager.SetMouseSensivity(value);
            }));
            
        }

        private void SetValue(string name, float value) => PlayerPrefs.SetFloat(name, value);

        private float GetValue(string name) => PlayerPrefs.GetFloat(name, -1);

        private class SliderObject
        {
            private readonly Slider slider;
            private readonly Label valueText;
            private readonly string settingsName;
            private readonly bool showValueInPercent;
            private readonly Action<float> settingsMethod;

            public SliderObject(Slider slider, Label valueText, string settingsName, float defaultValue, bool showValueInPercent, Action<float> settingsMethod)
            {
                this.slider = slider;
                this.valueText = valueText;
                this.settingsName = settingsName;
                this.showValueInPercent = showValueInPercent;
                this.settingsMethod = settingsMethod;

                float value = instance.GetValue(settingsName);
                if (value == -1) value = defaultValue;
                slider.value = value;
                slider.RegisterValueChangedCallback(v => SetValue(v.newValue));

                SetValue(value);
            }

            private void SetValue(float value)
            {
                float showValue = value > 10 ? Mathf.RoundToInt(value) : Mathf.RoundToInt(value * 100) / 100f;
                valueText.text = showValueInPercent ? showValue * 100 + "%" : showValue.ToString();
                instance.SetValue(settingsName, value);
                settingsMethod.Invoke(value);
            }
        }
    }
}
