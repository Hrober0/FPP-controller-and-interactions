using System.Collections;
using System.Collections.Generic;
using UI.Gameplay.Tips;
using UnityEngine;

public class Bucket : Tool
{
    [Header("spill the water")]
    [SerializeField] private Transform Up;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnTime = 0.5f;
    [SerializeField] private float size = 0.5f;
    [SerializeField] private float randomDeformation = 0.2f;
    private float timeTospillWater = 0f;

    public override bool CanUse => IsStandStraight();
    public override UIGameplayTips.Error ToolError => UIGameplayTips.Error.Bucket;

    

    public override void ShowToolTip()
    {
        Debug.Log("Bucket have to stand straight");
        if (UIGameplayTips.instance != null)
        {
            UIGameplayTips.instance.ShowError(UIGameplayTips.Error.Bucket);
        }
    }

    private void Update()
    {
        float angle = GetAngleToStreight();
        if (angle > 80)
        {
            timeTospillWater -= Time.deltaTime;
            if (timeTospillWater < 0f)
            {
                timeTospillWater = 2f;

                Vector3 pos = Up.position;
                pos.y = 0f;
                if (Dirty.IsSpaceForDirty(pos, 0.5f * Vector3.one))
                    Dirty.CreateLiquid(prefab, pos, size, randomDeformation, spawnTime);
            }
        }
        else if (angle < 20)
            timeTospillWater = 0f;
    }

    private bool IsStandStraight()
    {
        return GetAngleToStreight() < 15;
    }

    private float GetAngleToStreight()
    {
        Vector3 myAngles = transform.rotation.eulerAngles;
        Quaternion myDir = Quaternion.Euler(myAngles.x, 0, myAngles.z);
        Quaternion targetDir = Quaternion.Euler(0, 0, 0);
        return Quaternion.Angle(myDir, targetDir);
    }
}
