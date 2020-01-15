using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    [SerializeField]
    private string sceneToReload;
    [SerializeField]
    private string backToMenuScene;
    public void Restart()
    {
        SceneManager.LoadScene(sceneToReload);
        if (SFXManager.Instance.isActiveAndEnabled)
        {
            SFXManager.Instance.PlaySFX(Clip.Click);
        }
    }

    public void Next()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.saveData.isActive[BoardManager.instance.level + 1] = true;
            GameData.Instance.Save();
        }
        if (SFXManager.Instance.isActiveAndEnabled)
        {
            SFXManager.Instance.PlaySFX(Clip.Click);
        }
        SceneManager.LoadScene(backToMenuScene);
    }
}
