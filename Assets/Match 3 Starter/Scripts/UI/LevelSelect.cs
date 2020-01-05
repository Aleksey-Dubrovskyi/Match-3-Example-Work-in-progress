using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public static LevelSelect Instance;
    [SerializeField]
    private Image[] stars;
    private int starsActive;
    public bool isLocked;
    [SerializeField]
    private Text buttonText;

    public int levelNumber;
    private Image buttonBackground;
    [SerializeField]
    private Sprite buttonLockedBackground;
    [SerializeField]
    private GameObject levelConfigureWindow;
    private Button thisButton;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = GetComponent<LevelSelect>();
    }

    private void Start()
    {
        buttonText.text = levelNumber.ToString();
        thisButton = GetComponent<Button>();
        buttonBackground = GetComponent<Image>();
        LoadData();
        ButtonBacgroundSet();
        ResetStars();
    }

    private void LoadData()
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

    private void ResetStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    private void ButtonBacgroundSet()
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
