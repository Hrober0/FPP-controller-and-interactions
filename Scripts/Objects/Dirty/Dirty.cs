using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Gameplay.Tips;
using Tips;

public class Dirty : LookableObjects
{
    [SerializeField] private Tool cleaningTool;
    [SerializeField] [Min(0.1f)] protected float cleaningTime = 1f;

    protected float timeTolCleanUp = 0;

    protected Vector3 targetSize;

    private void Start()
    {
        if (timeTolCleanUp == 0)
            Set(transform.lossyScale);
    }

    public static Dirty CreateLiquid(GameObject prefab, Vector3 position, float size, float deformation, float creatingTime)
    {
        GameObject dirtyObject = Instantiate(prefab);
        dirtyObject.transform.position = position;
        dirtyObject.transform.Rotate(0, Random.Range(0, 360), 0); dirtyObject.transform.localScale = Vector3.up;

        Vector3 targetSize = new Vector3(Random.Range(size - deformation, size + deformation), dirtyObject.transform.localScale.y, Random.Range(size - deformation, size + deformation));

        Dirty dirty = dirtyObject.GetComponent<Dirty>();
        dirty.cleaningTime *= size;
        dirty.Set(targetSize);
        dirty.StartCoroutine(dirty.Extend(creatingTime));

        return dirty;
    }

    protected void Set(Vector3 targetSize)
    {
        timeTolCleanUp = cleaningTime;

        this.targetSize = targetSize;

        MessManager.instance.AddDirty(this);
    }


    protected virtual IEnumerator Extend(float creatingTime)
    {
        float sizePerSekX = targetSize.x / creatingTime;
        float sizePerSekY = targetSize.y / creatingTime;
        float sizePerSekZ = targetSize.z / creatingTime;

        float time = 0;
        while (time < creatingTime)
        {
            time += Time.deltaTime;
            transform.localScale += new Vector3(sizePerSekX * Time.deltaTime, sizePerSekY * Time.deltaTime, sizePerSekZ * Time.deltaTime);
            yield return null;
        }
    }

    public virtual void StartClening() => StartCoroutine(nameof(Clean));
    public virtual void StoptClening() => StopCoroutine(nameof(Clean));

    protected virtual IEnumerator Clean()
    {
        float sizePerSekX = targetSize.x / cleaningTime;
        float sizePerSekY = targetSize.y / cleaningTime;
        float sizePerSekZ = targetSize.z / cleaningTime;

        while (timeTolCleanUp > 0f)
        {
            timeTolCleanUp -= Time.deltaTime;
            transform.localScale = new Vector3(
                Mathf.Max(transform.localScale.x - sizePerSekX * Time.deltaTime, 0.01f),
                Mathf.Max(transform.localScale.y - sizePerSekY * Time.deltaTime, 0.01f),
                Mathf.Max(transform.localScale.z - sizePerSekZ * Time.deltaTime, 0.01f));
            yield return null;
        }

        Remove();
    }

    protected void Remove()
    {
        MessManager.instance.RemoveDirty(this);
        Destroy(gameObject);
    }


    public static bool IsSpaceForDirty(Vector3 position, Vector3 size)
    {
        Collider[] colliders = Physics.OverlapBox(position, size);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out Dirty _))
            {
                return false;
            }
        }
        return true;
    }


    public bool CanCleanBy(LookableObjects toolobject) => toolobject is Tool && cleaningTool.GetType() == toolobject.GetType();

    public float CleanPercent => timeTolCleanUp / cleaningTime;
    public float TimeTolCleanUp => timeTolCleanUp;

    public virtual void ShowToolTip()
    {
        Debug.Log("You need to pick up the " + cleaningTool.name);
        if (UIGameplayTips.instance != null)
        {
            UIGameplayTips.instance.ShowError(cleaningTool.ToolError);
        }
        if (TipArrow.instance != null)
        {
            GameObject obj = GameObject.Find(cleaningTool.GetType().Name);
            if (obj != null)
                TipArrow.instance.ShowArrowAt(obj.transform.position);
        }
    }
}
