using UnityEngine;

public class FrameLimiter : MonoBehaviour
{
    [SerializeField] int frameLimit = 300;
    void Start()
    {
        // Make the game run as fast as possible
        Application.targetFrameRate = frameLimit;
    }
}
