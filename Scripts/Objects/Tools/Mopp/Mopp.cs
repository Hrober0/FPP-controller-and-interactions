using System.Collections;
using System.Collections.Generic;
using UI.Gameplay.Tips;
using Tips;
using UnityEngine;

public class Mopp : Tool
{
    [SerializeField] private float usingTime = 5f;
    private float currUse = 0;

    [SerializeField] private GameObject mopDownGO;
    private Renderer mopDownR;
    private MaterialPropertyBlock props;

    public override bool CanUse => currUse > 0;
    public override UIGameplayTips.Error ToolError => UIGameplayTips.Error.Mop;


    private void Awake()
    {
        props = new MaterialPropertyBlock();
        mopDownR = mopDownGO.GetComponent<Renderer>();
        mopDownR.GetPropertyBlock(props);

        currUse = usingTime;
    }

    public void Use(float value)
    {
        currUse -= value;
        UpdateMopShader();
    }

    public void CleanUp()
    {
        currUse = usingTime;
        UpdateMopShader();
    }

    public override void ShowToolTip()
    {
        Debug.Log("You need to clean mopp");
        if (UIGameplayTips.instance != null)
        {
            UIGameplayTips.instance.ShowError(UIGameplayTips.Error.MopClean);
        }
        if (TipArrow.instance != null)
        {
            Bucket bucket = GameObject.FindObjectOfType<Bucket>();
            if (bucket != null)
                TipArrow.instance.ShowArrowAt(bucket.transform.position);
        }
    }

    private void UpdateMopShader()
    {
        props.SetFloat("_Blend_Textures_Lerp", Mathf.Clamp((1 - currUse / usingTime), 0, 1));
        mopDownR.SetPropertyBlock(props);
    }
}
