using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{

    public void PlayButton()
    {
        SceneManager.LoadScene("Menu");
        if (SFXManager.Instance.isActiveAndEnabled)
        {
            SFXManager.Instance.PlaySFX(Clip.Click);
        }
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Start Screen");
        if (SFXManager.Instance.isActiveAndEnabled)
        {
            SFXManager.Instance.PlaySFX(Clip.Click);
        }
    }
}
