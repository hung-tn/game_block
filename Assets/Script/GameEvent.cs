using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvent : MonoBehaviour
{
    public static Action<int, int> UpdateBestScoreBar;
    public static Action<bool> GameOver;
    public static Action<int> AddScores;
    public static Action CheckIfShapeCanBePlaced;
    public static Action MoveShapeToStartPosition;
    public static Action RequestNewShapes;
    public static Action SetShapeInactive;
    public static Action<Config.SquareColor> UpdateSquareColor;
    public static Action ShowWriting;
    public static Action<Config.SquareColor> ShowBonusScreen;
    
}
