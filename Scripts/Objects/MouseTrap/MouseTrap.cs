using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrap : MoveableObject
{
    [SerializeField] private float minForce = 3f;
    [SerializeField] private float maxForce = 5f;
    [SerializeField] private float verticalShift = 0.2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out MouseBehavior mouseBehavior))
        {
            float pushForce;
            Vector3 dir;
            
            pushForce = Random.Range(minForce, maxForce);
            dir = new Vector3(Random.Range(-verticalShift, verticalShift), 1, Random.Range(-verticalShift, verticalShift));
            Rigidbody trapRB = mouseBehavior.GetComponent<Rigidbody>();
            trapRB.isKinematic = false;
            trapRB.useGravity = true;
            trapRB.AddForce(dir * pushForce, ForceMode.Impulse);

            pushForce = Random.Range(minForce, maxForce);
            dir = new Vector3(Random.Range(-verticalShift, verticalShift), 1, Random.Range(-verticalShift, verticalShift));
            mouseBehavior.Kill(10f);
            Rigidbody mouseRB = mouseBehavior.GetComponent<Rigidbody>();
            mouseRB.isKinematic = false;
            mouseRB.useGravity = true;
            mouseRB.AddForce(dir * pushForce, ForceMode.Impulse); 
        }
    }
}
