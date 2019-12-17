using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    public GameObject gameOverPanel;
    public Text yourScoreTxt;
    public Text highScoreTxt;

    public Text scoreTxt;
    public Text moveCounterTxt;

    private int score;
    [SerializeField]
    private int moveCounter;
    [SerializeField]
    Image scoreBar;

    void Awake()
    {
        moveCounterTxt.text = moveCounter.ToString();
        instance = GetComponent<GUIManager>();
    }

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            if (scoreBar != null)
            {
                int lenght = BoardManager.instance.scoreGoals.Length;
                scoreBar.fillAmount = (float)score / (float)BoardManager.instance.scoreGoals[lenght - 1];
            }
            scoreTxt.text = score.ToString();
        }
    }

    public int MoveCounter
    {
        get
        {
            return moveCounter;
        }
        set
        {
            moveCounter = value;
            moveCounterTxt.text = moveCounter.ToString();
        }
    }

    // Show the game over panel
    public void GameOver()
    {
        GameManager.instance.gameOver = true;

        gameOverPanel.SetActive(true);

        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }

        yourScoreTxt.text = score.ToString();
    }

}
