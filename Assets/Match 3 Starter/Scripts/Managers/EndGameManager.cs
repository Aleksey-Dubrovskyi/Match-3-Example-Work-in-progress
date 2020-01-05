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
    }

    public void Next()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.saveData.isActive[BoardManager.instance.level + 1] = true;
            GameData.Instance.Save();
        }
        SceneManager.LoadScene(backToMenuScene);
    }
}
