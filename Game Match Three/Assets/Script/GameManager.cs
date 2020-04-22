using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int playerScore;
    public Text scoreText;
    // Tugas 2 scoreMultiple digunakan sebagai pengali untuk playerScore
    public int scoreMultiple = 1;

    void Start()
    {   
        if (!instance)
        {
            instance = this;
        }else if (instance)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Get_Score(int point){
        // Tugas 2 ditambahkan perkalian dengan scoreMultiple
        print("Score : " + scoreMultiple + " x (" + point + " + " + playerScore);
        playerScore = (playerScore + point) * scoreMultiple;
        scoreText.text = playerScore.ToString();
    }
}
