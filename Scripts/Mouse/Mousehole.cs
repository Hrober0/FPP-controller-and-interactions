using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mousehole : MonoBehaviour
{
    [SerializeField] private List<MousePath> paths = new List<MousePath>();

    private void Start()
    {
        MousesManager.instance.AddMousehole(this);
    }

    public List<MousePath> Paths => paths;
}