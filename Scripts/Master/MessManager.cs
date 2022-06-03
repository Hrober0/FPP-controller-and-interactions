using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessManager : MonoBehaviour
{
    public static MessManager instance;

    [Header("Tools")]
    [SerializeField] private List<Tool> cleaningTools = new List<Tool>();

    public List<Tool> CleaningTools => cleaningTools;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #region -Dirty-
    
    public int NumberOfAllDirty(Tool tool)
    {
        DirtiesInfo info = GetDirtiesInfo(tool);
        if (info == null)
        {
            Debug.LogWarning(tool.name + " in not supported in " + this.name);
            return 0;
        }
        else
            return info.allDirty;
    }
    public int NumberOfRemovedDirty(Tool tool)
    {
        DirtiesInfo info = GetDirtiesInfo(tool);
        if (info == null)
        {
            Debug.LogWarning(tool.name + " in not supported in " + this.name);
            return 0;
        }
        else
            return info.removedDirty;
    }

    private readonly Dictionary<Tool, DirtiesInfo> dirties = new Dictionary<Tool, DirtiesInfo>();
    private DirtiesInfo GetDirtiesInfo(Tool tool)
    {
        if (!cleaningTools.Contains(tool))
            return null;

        if (dirties.TryGetValue(tool, out DirtiesInfo dirtiesInfo))
            return dirtiesInfo;

        dirtiesInfo = new DirtiesInfo();
        dirties.Add(tool, dirtiesInfo);
        return dirtiesInfo;
    }
    public void AddDirty(Dirty dirty)
    {
        foreach (Tool tool in cleaningTools)
        {
            if (dirty.CanCleanBy(tool))
            {
                DirtiesInfo info = GetDirtiesInfo(tool);
                info.allDirty++;
                info.list.Add(dirty);
                return;
            }
        }
        Debug.LogWarning(dirty.gameObject.name + " has no supported tool in " + this.name);
    }
    public void RemoveDirty(Dirty dirty)
    {
        foreach (Tool tool in cleaningTools)
        {
            if (dirty.CanCleanBy(tool))
            {
                DirtiesInfo info = GetDirtiesInfo(tool);
                info.removedDirty++;
                info.list.Remove(dirty);
                if (MistakesController.instance != null)
                    MistakesController.instance.RemoveMistake();
                return;
            }
        }

        Debug.LogWarning(dirty.gameObject.name + " has no supported tool in " + this.name);
    }

    private class DirtiesInfo
    {
        public List<Dirty> list = new List<Dirty>();
        public int allDirty = 0;
        public int removedDirty = 0;
    }

    #endregion

    #region -ObjectToPutBack-

    public int NumberOfALLObjectToPutBack => objectsToPutBack.Count;
    public int NumberOfObjectToPutBackAtTargetPlaces
    {
        get
        {
            int n = 0;
            foreach (ObjectToPutBack obj in objectsToPutBack)
            {
                if (obj.IsOnTargetPlace)
                    n++;
            }
            return n;
        }
    }

    private readonly List<ObjectToPutBack> objectsToPutBack = new List<ObjectToPutBack>();
    public void AddObjectToPutBack(ObjectToPutBack objectToPutBack)
    {
        objectsToPutBack.Add(objectToPutBack);
    }

    public void RemoveObjectToPutBack(ObjectToPutBack objectToPutBack)
    {
        objectsToPutBack.Remove(objectToPutBack);
        if (MistakesController.instance != null)
            MistakesController.instance.AddMistake();
    }

    #endregion
}
