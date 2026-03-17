using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI continueText;
    public TextMeshProUGUI countdownText;
    //private int enemiesDestroyed;
    private int score;
    private int round;
    private int countdown;
    private bool continueYN;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        round = 1;
        countdown = 5;
        //enemiesDestroyed = 0;
        UpdateScoreText();
        UpdateRoundText();
    }

    public void ScorePoints(int points)
    {
        score += points;
        UpdateScoreText();
    }

    /*public void EnemiesDestroyed(int count)
    {
        enemiesDestroyed += count;

    }*/

    void UpdateScoreText()
    {
        scoreText.text = "Puntuación: " + score;
    }

    void UpdateRoundText()
    {
        roundText.text = "Round\n" + round;
    }

    void UpdateCountdownText()
    {
        for(int i = countdown; i > 0; i--)
        {
            countdownText.text = "" + i;
        }
    }
}
