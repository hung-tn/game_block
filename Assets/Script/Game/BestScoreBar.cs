using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScoreBar : MonoBehaviour
{
    public Image fillInImage;
    public Text bestScoreText;

    private void OnEnable()
    {
        GameEvent.UpdateBestScoreBar += UpdateBestScoreBar;
    }

    private void OnDisable()
    {
        GameEvent.UpdateBestScoreBar -= UpdateBestScoreBar;
    }

    private void UpdateBestScoreBar(int currentScore, int BestScore)
    {
        float currentPercentage = (float)currentScore / (float)BestScore;
        fillInImage.fillAmount = currentPercentage;
        bestScoreText.text = BestScore.ToString();
    }

}
