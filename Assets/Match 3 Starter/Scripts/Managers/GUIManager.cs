using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class Requiarament
{
    public GameType gameType;
    public int ammount;
}


public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;
    public Requiarament requiarament;
    public GameObject gameOverPanel;
    [SerializeField]
    private GameObject winGameWindow;
    public GameObject bottomUI;
    public Text yourScoreTxt;
    public Text highScoreTxt;

    public Text scoreTxt;
    public Text moveCounterTxt;

    private int score;
    private int ammountOFMoves;
    //[SerializeField]
    //private int moveCounter;
    [SerializeField]
    private Image scoreBar;

    private void Awake()
    {
        instance = GetComponent<GUIManager>();
    }

    private void Start()
    {
        SetGameType();
        moveCounterTxt.text = requiarament.ammount.ToString();
        if (requiarament.gameType == GameType.Time)
        {
            StartCoroutine(TimeCounter());
        }
    }

    public void SetGameType()
    {
        if (BoardManager.instance != null)
        {
            if (BoardManager.instance.world != null)
            {
                if (BoardManager.instance.world.levels[BoardManager.instance.level] != null)
                {
                    requiarament = BoardManager.instance.world.levels[BoardManager.instance.level].requiaraments;
                    ammountOFMoves = requiarament.ammount;
                }
            }
        }
    }

    //void Update()
    //{
    //    if (BoardManager.instance.gameState == GameState.lose)
    //    {
    //        BoardManager.instance.gameState = GameState.lose;
    //        StartCoroutine(WaitForShifting());
    //    }
    //}

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
                scoreBar.fillAmount = score / (float)BoardManager.instance.scoreGoals[lenght - 1];
            }
            if (GameData.Instance != null)
            {
                int hightScore = GameData.Instance.saveData.highScores[BoardManager.instance.level];
                if (score > hightScore)
                {
                    GameData.Instance.saveData.highScores[BoardManager.instance.level] = score;
                }
                GameData.Instance.Save();
            }
            scoreTxt.text = "Score: " + score.ToString();
        }
    }

    private IEnumerator TimeCounter()
    {
        while (ammountOFMoves > 0)
        {
            if (BoardManager.instance != null && BoardManager.instance.gameState != GameState.pause)
            {
                MoveTimer--;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public int MoveTimer
    {
        get
        {
            return ammountOFMoves;
        }
        set
        {
            ammountOFMoves = value;
            if (ammountOFMoves <= 0)
            {
                ammountOFMoves = 0;
                BoardManager.instance.gameState = GameState.lose;
                gameOverPanel.SetActive(true);
            }
            moveCounterTxt.text = ammountOFMoves.ToString();
        }
    }

    public int MoveCounter
    {
        get
        {
            return ammountOFMoves;
        }
        set
        {
            ammountOFMoves = value;
            if (ammountOFMoves <= 0 && BoardManager.instance.gameState != GameState.win)
            {
                StartCoroutine(WaitForShifting());
                ammountOFMoves = 0;
            }
            moveCounterTxt.text = ammountOFMoves.ToString();
        }
    }

    // Show the game over panel
    public void GameOver()
    {
        BoardManager.instance.gameState = GameState.lose;
        GameManager.instance.gameOver = true;
        bottomUI.SetActive(false);
        gameOverPanel.SetActive(true);

        yourScoreTxt.text = score.ToString();
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreTxt.text = /*"Best: " + */ PlayerPrefs.GetInt("HighScore").ToString();
        }
    }

    public void WinGameWindow()
    {
        BoardManager.instance.gameState = GameState.win;
        winGameWindow.SetActive(true);

        yourScoreTxt.text = score.ToString();
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreTxt.text = /*"Best: " + */ PlayerPrefs.GetInt("HighScore").ToString();
        }
    }

    private IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => BoardManager.instance.isShifring == false);
        yield return new WaitForSeconds(.5f);
        GameOver();
    }

}
