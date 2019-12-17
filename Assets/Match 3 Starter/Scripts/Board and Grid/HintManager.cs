using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [SerializeField]
    float hintDelay;
    float hintDelaySeconds;
    [SerializeField]
    GameObject hintParcticle;
    [SerializeField]
    GameObject currentHint;

    // Start is called before the first frame update
    void Start()
    {
        hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if (hintDelaySeconds <= 0 && currentHint == null)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }

    List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int x = 0; x < BoardManager.instance.xSize; x++)
        {
            for (int y = 0; y < BoardManager.instance.ySize; y++)
            {
                if (BoardManager.instance.allTiles[x, y] != null)
                {
                    if (x < BoardManager.instance.xSize - 1)
                    {
                        if (BoardManager.instance.SwitchAndCheck(x, y, Vector2.right))
                        {
                            possibleMoves.Add(BoardManager.instance.allTiles[x, y]);
                        }
                    }
                    if (y < BoardManager.instance.ySize - 1)
                    {
                        if (BoardManager.instance.SwitchAndCheck(x, y, Vector2.up))
                        {
                            possibleMoves.Add(BoardManager.instance.allTiles[x, y]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    GameObject PickRandomTile()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();
        if (possibleMoves.Count > 0)
        {
            int tileToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[tileToUse];
        }
        return null;
    }

    void MarkHint()
    {
        GameObject move = PickRandomTile();
        if (move != null)
        {
            currentHint = Instantiate(hintParcticle, move.transform.position, Quaternion.identity);
        }
    }

    public void DestroyHint()
    {
        if (currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
}
