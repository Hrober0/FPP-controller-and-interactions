using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public static class UIMethods
    {
        public static void SetElementActive(VisualElement element, bool value)
        {
            element.visible = value;
            element.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }
        public static bool IsElementActive(VisualElement element)
        {
            return element.style.display == DisplayStyle.Flex && element.visible;
        }
    }
}
    
