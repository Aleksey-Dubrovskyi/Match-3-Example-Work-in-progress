using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    public Image thisImage;
    public Sprite thisSprite;
    public Text thisText;
    public string thisString;

    private void Start()
    {
        SetUp();
    }

    private void SetUp()
    {
        thisImage.sprite = thisSprite;
        thisText.text = thisString;
    }
}
