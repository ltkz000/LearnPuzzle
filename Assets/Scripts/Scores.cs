using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    public Text scoreText;

    private int currentScore;

    private void Start()
    {
        currentScore = 0;
    }

    private void OnEnable()
    {
        GameEvents.AddScores += GameEvents_AddScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= GameEvents_AddScores;
    }

    private void GameEvents_AddScores(int scores)
    {
        currentScore += scores;
        scoreText.text = currentScore.ToString();
    }
}
