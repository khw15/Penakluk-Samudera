using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public Text ScoreText;
    public Text HighestScoreText;

    public void Setup(int FinalScore, int HighestCount)
    {
        gameObject.SetActive(true);
        ScoreText.text = "Skor Akhir: " + FinalScore.ToString();
        HighestScoreText.text = "Skor Tertinggi: " + HighestCount.ToString();
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("SplashScene");
    }
}
