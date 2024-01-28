using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public Text ScoreText;
    public Text HighestScoreText;
    private PlayerStats Stats;

    void Start()
    {
        Stats = GameObject.Find("Player").GetComponent<PlayerStats>();
        ScoreText.text = "Skor Akhir: " + Stats.FoodCount.ToString();
        HighestScoreText.text = "Skor Tertinggi: " + Stats.HighestCount.ToString();
        GameObject.Find("Player").SetActive(false);
        
        transform.GetChild(3).gameObject.SetActive(true); // Activates Child 3, the victory stinger
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
