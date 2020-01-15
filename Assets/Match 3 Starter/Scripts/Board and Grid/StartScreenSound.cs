using UnityEngine;
using UnityEngine.UI;

public class StartScreenSound : MonoBehaviour
{
    [SerializeField]
    private Sprite soundOn;
    [SerializeField]
    private Sprite soundOff;
    [SerializeField]
    private GameObject soundButton;
    [SerializeField]
    private GameObject sFXManager;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.GetComponent<Image>().sprite = soundOff;
                sFXManager.SetActive(false);
            }
            else
            {
                soundButton.GetComponent<Image>().sprite = soundOn;
                sFXManager.SetActive(true);
            }
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundOn;
            sFXManager.SetActive(false);
        }
    }

    public void AudioButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                PlayerPrefs.SetInt("Sound", 1);
                soundButton.GetComponent<Image>().sprite = soundOn;
                sFXManager.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetInt("Sound", 0);
                soundButton.GetComponent<Image>().sprite = soundOff;
                sFXManager.SetActive(false);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
            soundButton.GetComponent<Image>().sprite = soundOff;
            sFXManager.SetActive(false);
        }
    }
}
