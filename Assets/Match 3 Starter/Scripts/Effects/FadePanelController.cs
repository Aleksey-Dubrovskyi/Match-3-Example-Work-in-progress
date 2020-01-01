using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{

    [SerializeField]
    Animator panelAnim;
    [SerializeField]
    Animator gameInfoAnim;

    public void Ok()
    {
        if (panelAnim != null && gameInfoAnim != null)
        {
            BoardManager.instance.gameState = GameState.move;
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            GUIManager.instance.SetGameType();
        }
    }
}
