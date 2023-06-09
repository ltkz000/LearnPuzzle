using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action<int> AddScores;
    public static Action CheckIfShapeCanBePlaced;
    public static Action MoveShapeToStartPoint;
    public static Action RequestNewShape;
    public static Action SetShapeInactive;
    public static Action<bool> GameOver;
}
