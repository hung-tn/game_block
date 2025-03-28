using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}
public class Score : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public Text scoreText;
    private bool newBestScore_ = false;
    private BestScoreData bestScores_ = new BestScoreData();
    private int currentScores_;
    private string bestScoreKey_ = "bsdat";

    private void Awake()
    {
        if(BinaryDataStream.Exist(bestScoreKey_))
        {
            StartCoroutine(ReadDataFile());
        }
    }
    private IEnumerator ReadDataFile()
    {
        bestScores_ = BinaryDataStream.Read<BestScoreData>(bestScoreKey_);
        yield return new WaitForEndOfFrame();
        GameEvent.UpdateBestScoreBar(currentScores_, bestScores_.score);
    }
    private void Start()
    {
        currentScores_ = 0;
        newBestScore_ = false;
        squareTextureData.SetStartColor();
        UpdateScoreText();
    }

    

    private void OnEnable()
    {
        GameEvent.AddScores += AddScore;
        GameEvent.GameOver += SaveBestScores;
    }

    private void OnDisable()
    {
        GameEvent.AddScores -= AddScore;
        GameEvent.GameOver -= SaveBestScores;
    }

    private void SaveBestScores(bool newBestScores)
    {
        BinaryDataStream.Save<BestScoreData>(bestScores_, bestScoreKey_);
    }
    private void AddScore(int socres)
    {
        currentScores_ += socres;
        if(currentScores_ > bestScores_.score)
        {
            newBestScore_ = true;
            bestScores_.score = currentScores_;
            SaveBestScores(true);
        }
        UpdateSquareColors();
        GameEvent.UpdateBestScoreBar(currentScores_, bestScores_.score);
        UpdateScoreText();  
    }

    private void UpdateSquareColors()
    {
        if(GameEvent.UpdateSquareColor != null && currentScores_ >= squareTextureData.tresholdVal)
        {
            squareTextureData.UpdateColors(currentScores_);
            GameEvent.UpdateSquareColor(squareTextureData.currentColor);
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScores_.ToString();
    }
    
}
