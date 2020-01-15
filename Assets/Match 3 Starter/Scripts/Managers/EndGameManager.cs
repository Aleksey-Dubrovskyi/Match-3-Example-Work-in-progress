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
        if (!GameData.Instance.saveData.isCompleted[BoardManager.instance.level])
        {
            GameData.Instance.saveData.highScores[BoardManager.instance.level] = 0;
            GameData.Instance.saveData.stars[BoardManager.instance.level] = 0;
        }
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
            GameData.Instance.saveData.isCompleted[BoardManager.instance.level] = true;
            GameData.Instance.Save();
        }
        if (SFXManager.Instance.isActiveAndEnabled)
        {
            SFXManager.Instance.PlaySFX(Clip.Click);
        }
        SceneManager.LoadScene(backToMenuScene);
    }
}
