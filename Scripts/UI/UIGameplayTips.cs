using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace UI.Gameplay.Tips
{
    public class UIGameplayTips : MonoBehaviour
    {
        [SerializeField] private float errorShowTime = 4f;

        private VisualElement root;
        private VisualElement holdBar;
        private VisualElement holdBarThrow;

        public enum Error { Mop, Brush, MopClean, Bucket }
        public enum Tip { PickItem, HoldClean, HoldThrow, HoldPutBack }

        public static UIGameplayTips instance;
        private void Awake()
        {
            if (instance == null)
                instance = this;

            root = GetComponent<UIDocument>().rootVisualElement;

            holdBar = root.Q<VisualElement>("HoldBar");
            holdBarThrow = root.Q<VisualElement>("HoldBar_Throw");
        }

        private void Start()
        {
            foreach (Error error in Enum.GetValues(typeof(Error)))
                SetError(error, false);

            SetHoldBar(0, 0);

            foreach (Tip tip in Enum.GetValues(typeof(Tip)))
                SetTip(tip, false);
        }

        private void Update()
        {
            UpdateErrorsTimer();
        }


        #region -errors-

        private readonly Dictionary<Error, float> errorsTimer = new Dictionary<Error, float>();
        public void ShowError(Error error)
        {
            SetError(error, true);

            if (errorsTimer.ContainsKey(error))
                errorsTimer[error] = errorShowTime;
            else
                errorsTimer.Add(error, errorShowTime);
        }
        private void UpdateErrorsTimer()
        {
            List<Error> errors = errorsTimer.Keys.ToList();
            for (int i = 0; i < errors.Count; i++)
            {
                Error error = errors[i];
                errorsTimer[error] -= Time.deltaTime;
                if (errorsTimer[error] <= 0f)
                {
                    errors.RemoveAt(i);
                    i--;
                    SetError(error, false);
                }
            }
        }
        private void SetError(Error error, bool value)
        {
            VisualElement element = root.Q<VisualElement>("ErrorPanel_" + error.ToString());
            UIMethods.SetElementActive(element, value);
        }

        #endregion


        public void SetHoldBar(float percent, float timeToClenUp)
        {
            if (percent <= 0f)
                UIMethods.SetElementActive(holdBar, false);
            else
            {
                UIMethods.SetElementActive(holdBar, true);
                holdBar.Q<Label>("HB_BarText").text = Mathf.RoundToInt(timeToClenUp) + "s";

                VisualElement progress = holdBar.Q<VisualElement>("HB_BarFill");
                float endWidth = progress.parent.worldBound.width -10f;
                progress.style.width = endWidth * percent;
            }
        }

        public void SetHoldBarThrow(float percent)
        {
            if (percent <= 0f)
                UIMethods.SetElementActive(holdBarThrow, false);
            else
            {
                UIMethods.SetElementActive(holdBarThrow, true);

                VisualElement progress = holdBarThrow.Q<VisualElement>("HB_BarFill");
                float endWidth = progress.parent.worldBound.width - 10f;
                progress.style.width = endWidth * percent;
            }
        }


        public void SetPlaceItemInHighlitedPlace(bool value)
        {
            VisualElement element = root.Q<VisualElement>("PutDown");
            UIMethods.SetElementActive(element, value);
        }
        public void SetTip(Tip tip, bool value)
        {
            VisualElement element = root.Q<VisualElement>("TL_" + tip.ToString());
            UIMethods.SetElementActive(element, value);
        }
    }
}