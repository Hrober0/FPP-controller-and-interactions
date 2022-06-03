using System.Collections;
using System.Collections.Generic;
using UI.Gameplay.Tips;
using UnityEngine;

public class Broom : Tool
{
    public override bool CanUse => true;
    public override UIGameplayTips.Error ToolError => UIGameplayTips.Error.Brush;
}
