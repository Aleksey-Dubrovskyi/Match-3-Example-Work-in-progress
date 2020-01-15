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
    [Header("Game type")]
    public Requiarament requiarament;
    [SerializeField]
    private Text gameTypeText;
    [Header("Game panel  UI")]
    public GameObject gameOverPanel;
    [SerializeField]
    private GameObject winGameWindow;
    public GameObject bottomUI;
    [SerializeField]
    private Image[] stars;

    [Header("Lose panel text")]
    public Text yourScoreTxtL;
    public Text highScoreTxtL;

    [Header("Win panel Text")]
    public Text yourScoreTxtW;
    public Text highScoreTxtW;

    [Header("Comon variables")]
    public Text scoreTxt;
    public Text moveCounterTxt;
    private int numberStars;

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

            for (int i = 0; i < BoardManager.instance.scoreGoals.Length; i++)
            {
                if (score > BoardManager.instance.scoreGoals[i] && numberStars < i + 1)
                {
                    stars[numberStars].enabled = true;
                    numberStars++;
                }
            }

            if (GameData.Instance != null)
            {
                int hightScore = GameData.Instance.saveData.highScores[BoardManager.instance.level];
                if (score > hightScore)
                {
                    GameData.Instance.saveData.highScores[BoardManager.instance.level] = score;
                }
                int currentStars = GameData.Instance.saveData.stars[BoardManager.instance.level];
                if (numberStars > currentStars)
                {
                    GameData.Instance.saveData.stars[BoardManager.instance.level] = numberStars;
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
            if (BoardManager.instance != null &&
                (BoardManager.instance.gameState != GameState.pause
                || BoardManager.instance.gameState != GameState.lose
                || BoardManager.instance.gameState != GameState.win))
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
                StartCoroutine(WaitForShiftingLose());
                ammountOFMoves = 0;
                BoardManager.instance.gameState = GameState.lose;
            }
            gameTypeText.text = "Time";
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
                StartCoroutine(WaitForShiftingLose());
                ammountOFMoves = 0;
            }
            gameTypeText.text = "Moves";
            moveCounterTxt.text = ammountOFMoves.ToString();
        }
    }

    // Show the game over panel
    public void GameOver()
    {
        BoardManager.instance.gameState = GameState.lose;
        bottomUI.SetActive(false);
        gameOverPanel.SetActive(true);

        yourScoreTxtL.text = score.ToString();
        if (score > GameData.Instance.saveData.highScores[BoardManager.instance.level])
        {
            //PlayerPrefs.SetInt("HighScore", score);
            highScoreTxtL.text = "New Best: " + GameData.Instance.saveData.highScores[BoardManager.instance.level];
        }
        else
        {
            highScoreTxtL.text = /*"Best: " + */ GameData.Instance.saveData.highScores[BoardManager.instance.level].ToString();
        }
    }

    public void WinGameWindow()
    {
        StartCoroutine(WaitForShiftingWin());
    }

    private IEnumerator WaitForShiftingLose()
    {
        yield return new WaitUntil(() => BoardManager.instance.isShifring == false);
        yield return new WaitForSeconds(.5f);
        GameOver();
    }

    private IEnumerator WaitForShiftingWin()
    {
        yield return new WaitUntil(() => BoardManager.instance.isShifring == false);
        yield return new WaitForSeconds(.5f);
        BoardManager.instance.gameState = GameState.win;
        winGameWindow.SetActive(true);

        yourScoreTxtW.text = score.ToString();
        if (score > GameData.Instance.saveData.highScores[BoardManager.instance.level])
        {
            //PlayerPrefs.SetInt("HighScore", score);
            highScoreTxtW.text = "New Best: " + GameData.Instance.saveData.highScores[BoardManager.instance.level];
        }
        else
        {
            highScoreTxtW.text = /*"Best: " + */ GameData.Instance.saveData.highScores[BoardManager.instance.level].ToString();
        }
    }

}
