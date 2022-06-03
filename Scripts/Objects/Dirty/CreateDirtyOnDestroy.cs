using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateDirtyOnDestroy : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] private int chance = 100;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnTime = 0.5f;
    [SerializeField] private float size = 0.5f;
    [SerializeField] private float randomDeformation = 0.2f;

    private void OnDestroy()
    {
        if (prefab == null || gameObject == null || MessManager.instance == null)
            return;

#if UNITY_EDITOR
        if (EditorApplication.isPlaying == true && EditorApplication.isPlayingOrWillChangePlaymode == false)
            return;
#endif

        Collider collider = GetComponent<Collider>();
        if (collider.bounds.min.y > 0.5f)
            return;

        if (Random.Range(0, 100) <= chance)
        {
            Vector3 pos = transform.position;
            pos.y = 0;
            if (!Dirty.IsSpaceForDirty(pos, 0.5f * Vector3.one))
                return;

            Dirty.CreateLiquid(prefab, pos, size, randomDeformation, spawnTime);
        }
    }
}
