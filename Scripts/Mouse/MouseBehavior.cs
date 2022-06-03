using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Audio;

public class MouseBehavior : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] [Min(0)] private float defaultSpeed = 2f;

    [Header("Interactions")]
    [SerializeField] [Min(0)] private float pushForce = 4f;
    [SerializeField] [Min(0)] private float forceNeedToKill = 4f;
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] [Range(0f, 1f)] float hitVolume = 0.7f;
    [SerializeField] private float hitdDistanceScale = 3f;

    [Header("Nibbing sounds")]
    [SerializeField] private float minTimeToMakeSound = 4f;
    [SerializeField] private float maxTimeToMakeSound = 4f;
    [SerializeField] private AudioClip[] nibbingSounds;
    [SerializeField] [Range(0f, 1f)] float nibbingVolume = 0.7f;
    [SerializeField] private float nibbingDistanceScale = 3f;

    public float DefaultSpeed => defaultSpeed;

    private MousePath path = null;

    private int curentPathIndex;
    private Transform target;
    private Transform mousehool;

    private Animator animator;

    private float currentSpeed;
    private bool isAlive = false;
    private bool walk = false;

    public bool Walk
    {
        get => walk;
        set
        {
            walk = value;
            animator.SetBool("Walk", value);
        }
    }

    private void Awake()
    {
        isAlive = true;
        currentSpeed = defaultSpeed;
        animator = gameObject.GetComponent<Animator>();
    }

    #region -movement-

    private void Update()
    {
        if (isAlive && Walk && path != null)
        {
            float distanceThisFrame = currentSpeed * Time.deltaTime;
            transform.Translate(Vector3.forward * distanceThisFrame * 0.8f, Space.Self);

            if (target.position - transform.position != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, currentSpeed * 10 * Time.deltaTime);

                // small move to avoid stack
                transform.Translate(targetRotation * Vector3.forward * distanceThisFrame * 0.2f, Space.World);
            }

            if (Vector3.Distance(target.position, transform.position) < distanceThisFrame)
                GetNextPathPoint();
        }
    }

    public void SendMouse(MousePath path)
    {
        CancelAllActions();

        this.path = path;

        curentPathIndex = 0;
        mousehool = path.Point(curentPathIndex);
        transform.position = path.Point(curentPathIndex).position;

        GetNextPathPoint();

        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

        gameObject.SetActive(true);
        Walk = true;

        StopCoroutine(nameof(MakeNibbingSounds));
        StartCoroutine(nameof(MakeNibbingSounds));
    }

    private void GetNextPathPoint()
    {
        foreach (IMouseAction mouseAction in path.Point(curentPathIndex).GetComponents<IMouseAction>())
            mouseAction.TriggerAction(this);

        curentPathIndex++;

        if (curentPathIndex == path.Count)
        {
            path = null;
            Walk = false;
            gameObject.SetActive(false);
            MousesManager.instance.SetMouseAsInactive(this);
            return;
        }

        target = path.Point(curentPathIndex);
    }

    #endregion

    #region -interaction-

    private void OnCollisionEnter(Collision collision)
    {
        if (Vector3.Distance(transform.position, mousehool.position) < 0.2f)
            return;

        if (collision.gameObject.TryGetComponent(out Rigidbody rigitbody))
        {
            float force = rigitbody.velocity.magnitude;
            //Debug.Log(name + ": hit by " + collision.gameObject.name + " at force " + force);
            if (force >= forceNeedToKill)
            {
                Kill(1f);
                return;
            }

            if (force >= forceNeedToKill / 2)
            {
                CancelAllActions();
                ChangeSpeed(defaultSpeed, 2f);
                PlayHitSound();
                return;
            }

            Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(1f, 3f), UnityEngine.Random.Range(-1f, 1f));
            rigitbody.AddForce(dir * pushForce, ForceMode.Impulse);
        }
    }

    public void Kill(float delay)
    {
        if (isAlive)
        {
            isAlive = false;
            StartCoroutine(Die(delay));
        }
    }

    private IEnumerator Die(float delay)
    {
        MousesManager.instance.RemoveDeadMouse(this);

        CancelAllActions();

        PlayHitSound();

        Walk = false;
        animator.SetTrigger("Jump");
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;

        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    #endregion

    #region -actions-

    private readonly List<IEnumerator> actions = new List<IEnumerator>();

    private void CancelAllActions()
    {
        foreach (IEnumerator action in actions)
            StopCoroutine(action);
        actions.Clear();

        Walk = true;
        currentSpeed = defaultSpeed;
    }

    // wait
    public void Wait(float time)
    {
        ChangeRunning(BoolToFloat(false));
        WaitAndCallMethod(ChangeRunning, BoolToFloat(true), time);

        PlayNibbingSound();
    }
    private void ChangeRunning(float state) => Walk = FloatToBool(state);

    // ChangeSpeed
    public void ChangeSpeed(float modifier, float time)
    {
        ModifiSpeed(modifier);
        WaitAndCallMethod(ModifiSpeed, -modifier, time);
    }
    private void ModifiSpeed(float modifier) => currentSpeed += modifier;


    // methods
    private void WaitAndCallMethod(Action<float> method, float parametr, float time)
    {
        IEnumerator enumerator = WaitAndCallMethodC(method, parametr, time);
        actions.Add(enumerator);
        StartCoroutine(enumerator);
    }
    private IEnumerator WaitAndCallMethodC(Action<float> method, float parametr, float time)
    {
        yield return new WaitForSeconds(time);

        method(parametr);
    }

    private bool FloatToBool(float v) => v < 0.5f ? false : true;
    private float BoolToFloat(bool v) => v ? 1 : 0;

    #endregion

    #region -sounds-

    private IEnumerator MakeNibbingSounds()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeToMakeSound, maxTimeToMakeSound));
            PlayNibbingSound();
        }
    }
    private void PlayNibbingSound()
    {
        if (AudioManager.instance != null)
        {
            AudioClip clip = nibbingSounds[UnityEngine.Random.Range(0, nibbingSounds.Length)];
            AudioManager.instance.PlaySound(new Sound().ObjectSound(clip, nibbingVolume, nibbingDistanceScale, gameObject));
        }
            
    }
    private void PlayHitSound()
    {
        if (AudioManager.instance != null)
        {
            AudioClip clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)];
            AudioManager.instance.PlaySound(new Sound().ObjectSound(clip, hitVolume, hitdDistanceScale, gameObject));
        }
    }

    #endregion
}
