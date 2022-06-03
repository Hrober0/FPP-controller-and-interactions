using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistakesController : MonoBehaviour
{
    [SerializeField] private int maxMistakes = 10;

    private int currMistakes = 0;

    public int MaxMistakes => maxMistakes;
    public int CurrMistakes => currMistakes;

    public static MistakesController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddMistake()
    {
        currMistakes++;
    }

    public void RemoveMistake()
    {
        currMistakes--;
        if (currMistakes <= 0)
            currMistakes = 0;
    }
}
