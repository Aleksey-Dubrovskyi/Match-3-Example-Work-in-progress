using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    [SerializeField]
    Animator panelAnim;

    public void Ok()
    {
        if (panelAnim != null)
        {
            BoardManager.instance.gameState = GameState.move;
            panelAnim.SetBool("Out", true);
            GUIManager.instance.SetGameType();
        }
    }
}
