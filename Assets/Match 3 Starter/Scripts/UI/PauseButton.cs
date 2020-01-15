using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [Header("Setings Section")]
    [SerializeField]
    private bool isClicked;
    [SerializeField]
    private GameObject setingsWindow;
    private Animator animator;
    [Header("Sound Button Section")]
    [SerializeField]
    private GameObject sFXManager;
    [SerializeField]
    private GameObject soundButton;
    [SerializeField]
    private Sprite soundOn;
    [SerializeField]
    private Sprite soundOf;

    private void Start()
    {
        animator = setingsWindow.GetComponent<Animator>();
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.GetComponent<Image>().sprite = soundOf;
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

    public void OnButtonClick()
    {
        if (!isClicked)
        {
            BoardManager.instance.gameState = GameState.pause;
            setingsWindow.SetActive(true);
            animator.SetBool("isOpen", true);
            isClicked = true;
            if (SFXManager.Instance.isActiveAndEnabled)
            {
                SFXManager.Instance.PlaySFX(Clip.Click);
            }
        }
        else if (isClicked)
        {
            BoardManager.instance.gameState = GameState.move;
            animator.SetBool("isOpen", false);
            isClicked = false;
            if (SFXManager.Instance.isActiveAndEnabled)
            {
                SFXManager.Instance.PlaySFX(Clip.Click);
            }
        }
    }

    public void ExitButton()
    {
        if (SFXManager.Instance.isActiveAndEnabled)
        {
            SFXManager.Instance.PlaySFX(Clip.Click);
        }
        SceneManager.LoadScene("Menu");
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
                soundButton.GetComponent<Image>().sprite = soundOf;
                sFXManager.SetActive(false);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
            soundButton.GetComponent<Image>().sprite = soundOf;
            sFXManager.SetActive(false);
        }
    }
}
