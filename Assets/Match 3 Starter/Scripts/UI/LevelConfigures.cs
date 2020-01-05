using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelConfigures : MonoBehaviour
{
    [SerializeField]
    private Image[] stars;
    private int starsActive;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private string sceneToLoad;

    public int level;

    public int score;

    // Start is called before the first frame update
    private void OnEnable()
    {
        LoadData();
        ResetStars();
        scoreText.text = score.ToString();
    }

    private void LoadData()
    {
        if (GameData.Instance != null)
        {
            starsActive = GameData.Instance.saveData.stars[level - 1];
            score = GameData.Instance.saveData.highScores[level - 1];
        }
    }

    private void ResetStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    public void MenuButton()
    {
        this.gameObject.SetActive(false);
    }

    public void PlayButton()
    {
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(sceneToLoad);
    }

}
