using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousesManager : MonoBehaviour
{
    [SerializeField] private GameObject mousePrefab;

    [SerializeField] [Min(0)] private int maxNumberOfMouses = 10;

    [Header("Start wave")]
    [SerializeField] [Min(0)] private int startNumberOfMousesInWave = 1;
    [SerializeField] [Min(0)] private float delayToFirstWave = 4f;

    [Header("Next waves")]
    [SerializeField] [Min(0)] private float numberOfMousesIncreaseByWave = 0.34f;
    [SerializeField] [Min(0)] private float minNextWaveTime = 2f;
    [SerializeField] [Min(0)] private float maxNextWaveTime = 4f;
    [SerializeField] [Min(0)] private float mousesInWaveExitDelay = 2f;

    private readonly List<MouseBehavior> availableMouses = new List<MouseBehavior>();
    private readonly List<MouseBehavior> activeMouses = new List<MouseBehavior>();

    private readonly List<Mousehole> mouseholes = new List<Mousehole>();

    private int wave = 0;
    private float timeToSendNextWave;
    private float mouseToSpawn = 0f;

    private bool sendingMouses = false;
    private int numberOfMousesInWave = 0;
    private int spawndedMouses = 0;


    private int numberOfKilledMouses = 0;
    private int numberOfAllMouses;

    public int NumberOfKilledMouses => numberOfKilledMouses;
    public int NumberOfAllMouses => numberOfAllMouses;

    public static MousesManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        timeToSendNextWave = delayToFirstWave;
        numberOfMousesInWave = startNumberOfMousesInWave;
        numberOfAllMouses = maxNumberOfMouses;
    }

    private void Update()
    {
        if (!sendingMouses && activeMouses.Count == 0)
        {
            timeToSendNextWave -= Time.deltaTime;
            if (timeToSendNextWave <= 0f)
                StartCoroutine(nameof(StartWave));
        }
    }

    private IEnumerator StartWave()
    {
        timeToSendNextWave = Random.Range(minNextWaveTime, maxNextWaveTime);

        wave++;

        List<MousePath> paths = new List<MousePath>();
        sendingMouses = true;
        int mousesToSend = Mathf.Clamp(numberOfMousesInWave, 0, maxNumberOfMouses);
        Debug.Log("Start wave " + wave + " mouses " + mousesToSend);
        for (int i = 0; i < mousesToSend; i++)
        {
            if (paths.Count == 0)
            {
                foreach (Mousehole mousehole in mouseholes)
                    paths.AddRange(mousehole.Paths);
            }

            MousePath mousePath = paths[Random.Range(0, paths.Count)];
            SendNextMouse(mousePath);

            yield return new WaitForSeconds(mousesInWaveExitDelay);
        }
        sendingMouses = false;


        mouseToSpawn += numberOfMousesIncreaseByWave;
        while (mouseToSpawn > 1f)
        {
            numberOfMousesInWave++;
            mouseToSpawn -= 1f;
        }
    }

    private void CreateNewMouse()
    {
        GameObject mouse = Instantiate(mousePrefab);
        mouse.transform.parent = transform;
        mouse.name = mousePrefab.name + spawndedMouses;
        mouse.SetActive(false);

        spawndedMouses++;

        MouseBehavior mouseBehavior = mouse.GetComponent<MouseBehavior>();
        availableMouses.Add(mouseBehavior);
    }

    private void SendNextMouse(MousePath mousePath)
    {
        if (availableMouses.Count == 0)
            CreateNewMouse();

        MouseBehavior mouse = availableMouses[0];

        availableMouses.RemoveAt(0);
        activeMouses.Add(mouse);

        mouse.SendMouse(mousePath);
    }

    public void SetMouseAsInactive(MouseBehavior mouse)
    {
        availableMouses.Add(mouse);
        activeMouses.Remove(mouse);
    }
    public void RemoveDeadMouse(MouseBehavior mouse)
    {
        availableMouses.Remove(mouse);
        activeMouses.Remove(mouse);
        maxNumberOfMouses--;
        numberOfKilledMouses++;
        if (MistakesController.instance != null)
            MistakesController.instance.RemoveMistake();
    }

    public void AddMousehole(Mousehole mousehole) => mouseholes.Add(mousehole);
}