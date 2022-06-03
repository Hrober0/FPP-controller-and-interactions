using UnityEngine;

public class MAWait : MonoBehaviour, IMouseAction
{
    [SerializeField] private float time = 1f;

    public void TriggerAction(MouseBehavior mouse)
    {
        mouse.Wait(time);
    }
}
