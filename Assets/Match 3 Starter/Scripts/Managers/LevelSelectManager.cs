using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] panels;
    [SerializeField]
    private GameObject currentPanel;
    [SerializeField]
    private int page;
    [SerializeField]
    private int currentLevel;
    [SerializeField]
    private GameObject leftArow;
    [SerializeField]
    private Sprite leftArrowUnlocked;
    [SerializeField]
    private Sprite leftArrowLocked;
    [SerializeField]
    private GameObject rightArow;
    [SerializeField]
    private Sprite rightArrowUnlocked;
    [SerializeField]
    private Sprite rightArrowLocked;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        if (GameData.Instance != null)
        {
            for (int i = 0; i < GameData.Instance.saveData.isActive.Length; i++)
            {
                if (GameData.Instance.saveData.isActive[i])
                {
                    currentLevel = i;
                }
            }
        }
        page = (int)Mathf.Floor(currentLevel / 16);
        currentPanel = panels[page];
        panels[page].SetActive(true);
    }

    private void OnEnable()
    {
        if (page < panels.Length - 1)
        {
            leftArow.GetComponent<Image>().sprite = leftArrowLocked;
            rightArow.GetComponent<Image>().sprite = rightArrowUnlocked;
        }
        if (page > 0)
        {
            leftArow.GetComponent<Image>().sprite = leftArrowUnlocked;
            rightArow.GetComponent<Image>().sprite = rightArrowLocked;
        }
    }

    public void PageRight()
    {
        if (page < panels.Length - 1)
        {
            currentPanel.SetActive(false);
            page++;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
            leftArow.GetComponent<Image>().sprite = leftArrowUnlocked;
            rightArow.GetComponent<Image>().sprite = rightArrowLocked;
        }
    }
    public void PageLeft()
    {
        if (page > 0)
        {
            currentPanel.SetActive(false);
            page--;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
            leftArow.GetComponent<Image>().sprite = leftArrowLocked;
            rightArow.GetComponent<Image>().sprite = rightArrowUnlocked;
        }
    }
}
