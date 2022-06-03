using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlacingPlace : LookableObjects
{
    [SerializeField] private GameObject visualization;
    [SerializeField] private Collider lookableCollider;
    [SerializeField] private GameObject space;

    private void Start()
    {
        SetVisActive(false);
    }

    public void SetVisActive(bool value)
    {
        visualization.SetActive(value);
        lookableCollider.enabled = value;
    }

    public GameObject Space => space;

    public Vector3 GetDropPosition(MoveableObject moveable)
    {
        Collider movableCol = moveable.GetComponent<Collider>();
        Collider placecCollider = space.GetComponent<Collider>();

        Vector3 placingSpace = placecCollider.bounds.size;
        Vector3 mSize = movableCol.bounds.size + Vector3.one * 0.2f;
        mSize.y /= 3f;
        int xSpaces = Mathf.Max(Mathf.FloorToInt(placingSpace.x / mSize.x), 1);
        int ySpaces = Mathf.Max(Mathf.FloorToInt(placingSpace.y / mSize.y), 1);
        int zSpaces = Mathf.Max(Mathf.FloorToInt(placingSpace.z / mSize.z), 1);

        Vector3 centerOffset = new Vector3(
            (placingSpace.x - xSpaces * mSize.x + mSize.x) / 2f,
            mSize.y,
            (placingSpace.z - zSpaces * mSize.z + mSize.z) / 2f
            );

        Vector3 startPos = placecCollider.bounds.min + centerOffset;

        int layer = 1 << LookableObjects.layer;
        List<Vector3> avaPlaces = new List<Vector3>();
        for (int x = 0; x < xSpaces; x++)
        {
            for (int z = 0; z < zSpaces; z++)
            {
                for (int y = 0; y < ySpaces; y++)
                {
                    Vector3 checkedPlace = startPos + new Vector3(mSize.x * x, mSize.y * y, mSize.z * z);
                    if (Physics.CheckBox(checkedPlace, mSize / 3f, moveable.transform.rotation, layer))
                    {
                        Debug.DrawLine(checkedPlace + Vector3.up * mSize.y, checkedPlace + Vector3.down * mSize.y, Color.red, 5f);
                    }
                    else
                    {
                        Debug.DrawLine(checkedPlace + Vector3.up * mSize.y, checkedPlace + Vector3.down * mSize.y, Color.green, 5f);
                        avaPlaces.Add(checkedPlace);
                        break;
                    }
                }
            }
        }

        if (avaPlaces.Count == 0)
        {
            Debug.LogWarning("No space found!");
            return moveable.transform.position;
        }

        Vector3 closestPoint = moveable.transform.position;
        avaPlaces = avaPlaces.OrderByDescending(p => Vector3.SqrMagnitude(closestPoint - p)).OrderBy(p => p.y).ToList();

        Vector3 dropPos = avaPlaces[0];

        float hightesPlace = avaPlaces.Max(p => p.y);
        dropPos.y = hightesPlace + mSize.y + 0.5f;
        //Debug.Log(hightesPlace + " + " + mSize.y + " =" + dropPos.y);
        return dropPos;
    }
}
