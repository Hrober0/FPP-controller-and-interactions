using UnityEngine;

public class MAChangeSpeed : MonoBehaviour, IMouseAction
{
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float time = 1f;

    public void TriggerAction(MouseBehavior mouse)
    {
        mouse.ChangeSpeed(mouse.DefaultSpeed * speedMultiplier, time);
    }
}
