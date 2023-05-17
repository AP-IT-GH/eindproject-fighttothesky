using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score instance;
    public int score = 0;
    public Text scoreText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Automatically find and assign the Text component if it's not manually assigned
        if (scoreText == null)
            scoreText = FindObjectOfType<Text>();
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
}
