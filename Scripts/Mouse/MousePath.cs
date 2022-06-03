using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePath : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showPath = true;
    [SerializeField] private Color pathColor = Color.blue;

    private Transform[] pathPoints = new Transform[0];


    private void Start()
    {
        UpdatePath();
    }


    public int Count => pathPoints.Length;
    public Transform Point(int index) => pathPoints[index];


    private void UpdatePath()
    {
        int childs = transform.childCount;
        pathPoints = new Transform[childs + 2];
        pathPoints[0] = transform;
        pathPoints[childs + 1] = transform;
        for (int i = 0; i < childs; i++)
        {
            Transform trans = transform.GetChild(i);
            int index = i + 1;
            trans.name = name + "-" + (index < 10 ? ("0" + index) : index);
            pathPoints[index] = trans;
        }
    }


    private void OnDrawGizmos()
    {
        if (showPath && Application.isEditor)
            UpdatePath();

        // show path
        if (showPath && pathPoints.Length > 0)
        {
            Transform lastPoint = pathPoints[0];
            for (int i = 1; i < pathPoints.Length; i++)
            {
                Gizmos.color = pathColor;
                Gizmos.DrawLine(lastPoint.position, pathPoints[i].position);
                lastPoint = pathPoints[i];
            }
        }
    }
}