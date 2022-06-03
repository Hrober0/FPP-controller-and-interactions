using System.Collections;
using System.Collections.Generic;
using UI.Gameplay.Tips;
using UnityEngine;

public abstract class Tool : MoveableObject
{
    [SerializeField] private AudioClip usingSound;
    public AudioClip UsingSound => usingSound;

    public abstract bool CanUse { get; }

    public abstract UIGameplayTips.Error ToolError { get; }

    public virtual void ShowToolTip()
    {
        Debug.Log("Cant use");
    }
}
