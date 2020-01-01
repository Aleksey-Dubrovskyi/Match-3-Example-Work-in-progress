using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelConfigures : MonoBehaviour
{
    [SerializeField]
    Image[] stars;
    int starsActive;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    string sceneToLoad;

    public int level;

    public int score;
    // Start is called before the first frame update
    void OnEnable()
    {
        LoadData();
        ResetStars();
        scoreText.text = score.ToString();
    }

    void LoadData()
    {
        if (GameData.Instance != null)
        {
            starsActive = GameData.Instance.saveData.stars[level - 1];
            score = GameData.Instance.saveData.highScores[level - 1];
        }
    }

    void ResetStars()
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
