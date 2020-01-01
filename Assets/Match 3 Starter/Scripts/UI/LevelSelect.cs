using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public static LevelSelect Instance;
    [SerializeField]
    Image[] stars;
    int starsActive;
    public bool isLocked;
    [SerializeField]
    Text buttonText;

    public int levelNumber;
    Image buttonBackground;
    [SerializeField]
    Sprite buttonLockedBackground;
    [SerializeField]
    GameObject levelConfigureWindow;
    Button thisButton;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = GetComponent<LevelSelect>();
    }

    void Start()
    {
        buttonText.text = levelNumber.ToString();
        thisButton = GetComponent<Button>();
        buttonBackground = GetComponent<Image>();
        LoadData();
        ButtonBacgroundSet();
        ResetStars();
    }

    void LoadData()
    {
        if (GameData.Instance != null)
        {
            if (GameData.Instance.saveData.isActive[levelNumber - 1])
            {
                isLocked = false;
            }
            else
            {
                isLocked = true;
            }
            starsActive = GameData.Instance.saveData.stars[levelNumber - 1];
        }
    }

    void ResetStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    void ButtonBacgroundSet()
    {
        if (isLocked)
        {
            buttonBackground.sprite = buttonLockedBackground;
            thisButton.enabled = false;
        }
    }

    public void OnClick()
    {
        levelConfigureWindow.GetComponent<LevelConfigures>().level = levelNumber;
        levelConfigureWindow.SetActive(true);
    }
}
