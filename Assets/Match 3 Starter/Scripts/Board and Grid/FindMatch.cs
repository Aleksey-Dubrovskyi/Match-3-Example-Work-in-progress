using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatch : MonoBehaviour
{
    public List<GameObject> allMatches = new List<GameObject>();
    public static FindMatch instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<FindMatch>();
    }

    public void FindAllmatches()
    {
        StartCoroutine(FindMatchCo());
    }

    IEnumerator FindMatchCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int x = 0; x < BoardManager.instance.xSize; x++)
        {
            for (int y = 0; y < BoardManager.instance.ySize; y++)
            {
                GameObject currentTile = BoardManager.instance.allTiles[x, y];
                if (currentTile != null)
                {
                    if (x > 0 && x < BoardManager.instance.xSize - 1)
                    {
                        GameObject leftTile = BoardManager.instance.allTiles[x - 1, y];
                        GameObject rightTile = BoardManager.instance.allTiles[x + 1, y];
                        if (leftTile != null && rightTile != null)
                        {
                            if (leftTile.GetComponent<SpriteRenderer>().sprite == currentTile.gameObject.GetComponent<SpriteRenderer>().sprite && rightTile.GetComponent<SpriteRenderer>().sprite == currentTile.gameObject.GetComponent<SpriteRenderer>().sprite)
                            {
                                if (!allMatches.Contains(leftTile))
                                {
                                    allMatches.Add(leftTile);
                                }

                                leftTile.GetComponent<Tiles>().isMatched = true;

                                if (!allMatches.Contains(rightTile))
                                {
                                    allMatches.Add(rightTile);
                                }
                                rightTile.GetComponent<Tiles>().isMatched = true;
                                if (!allMatches.Contains(currentTile))
                                {
                                    allMatches.Add(currentTile);
                                }
                                currentTile.GetComponent<Tiles>().isMatched = true;
                            }
                        }
                    }

                    if (y > 0 && y < BoardManager.instance.ySize - 1)
                    {
                        GameObject upTile = BoardManager.instance.allTiles[x, y + 1];
                        GameObject downTile = BoardManager.instance.allTiles[x, y - 1];

                        if (upTile != null && downTile != null)
                        {
                            if (upTile.GetComponent<SpriteRenderer>().sprite == currentTile.gameObject.GetComponent<SpriteRenderer>().sprite && downTile.GetComponent<SpriteRenderer>().sprite == currentTile.gameObject.GetComponent<SpriteRenderer>().sprite)
                            {
                                if (!allMatches.Contains(upTile))
                                {
                                    allMatches.Add(upTile);
                                }
                                upTile.GetComponent<Tiles>().isMatched = true;
                                if (!allMatches.Contains(downTile))
                                {
                                    allMatches.Add(downTile);
                                }
                                downTile.GetComponent<Tiles>().isMatched = true;
                                if (!allMatches.Contains(currentTile))
                                {
                                    allMatches.Add(currentTile);
                                }
                                currentTile.GetComponent<Tiles>().isMatched = true;
                            }
                        }

                    }
                }
            }
        }
    }
}
