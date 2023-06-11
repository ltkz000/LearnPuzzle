using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    [SerializeField] private GameObject darkBg;
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private GameObject overText;
    [SerializeField] private GameObject newBestText;

    private void Start()
    {
        gameOverPopup.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += GameEvents_GameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= GameEvents_GameOver;
    }

    private void GameEvents_GameOver(bool newBestScore)
    {
        darkBg.SetActive(true);
        gameOverPopup.SetActive(true);
        overText.SetActive(true);
        newBestText.SetActive(false);
    }
}
