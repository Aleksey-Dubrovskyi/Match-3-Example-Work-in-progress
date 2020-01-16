using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class FindMatch : MonoBehaviour
{
    public List<GameObject> allMatches = new List<GameObject>();
    public static FindMatch instance;

    private void Start()
    {
        instance = GetComponent<FindMatch>();
    }

    public void FindAllmatches()
    {
        StartCoroutine(FindMatchCo());
    }

    private List<GameObject> IsAdjacentBomb(Tiles tile1, Tiles tile2, Tiles tile3)
    {
        List<GameObject> currenTiles = new List<GameObject>();
        if (tile1.isAdjacentBomb)
        {
            allMatches.Union(GetAdjacentTiles(tile1.column, tile1.row));
        }
        if (tile2.isAdjacentBomb)
        {
            allMatches.Union(GetAdjacentTiles(tile2.column, tile2.row));
        }
        if (tile3.isAdjacentBomb)
        {
            allMatches.Union(GetAdjacentTiles(tile3.column, tile3.row));
        }
        return currenTiles;
    }

    private void AddToListAndMatch(GameObject tile)
    {
        if (!allMatches.Contains(tile))
        {
            allMatches.Add(tile);
        }        
        tile.GetComponent<Tiles>().isMatched = true;

    }

    private void GetNearbyTile(GameObject tile1, GameObject tile2, GameObject tile3)
    {
        AddToListAndMatch(tile1);
        AddToListAndMatch(tile2);
        AddToListAndMatch(tile3);
    }

    private List<GameObject> IsRowBomb(Tiles tile1, Tiles tile2, Tiles tile3)
    {
        List<GameObject> currenTiles = new List<GameObject>();
        if (tile1.isRowBomb)
        {
            allMatches.Union(GetRowTiles(tile1.row));
            BoardManager.instance.BombRow(tile1.row);
        }
        if (tile2.isRowBomb)
        {
            allMatches.Union(GetRowTiles(tile2.row));
            BoardManager.instance.BombRow(tile2.row);
        }
        if (tile3.isRowBomb)
        {
            allMatches.Union(GetRowTiles(tile3.row));
            BoardManager.instance.BombRow(tile3.row);
        }
        return currenTiles;
    }

    private List<GameObject> IsColumnBomb(Tiles tile1, Tiles tile2, Tiles tile3)
    {
        List<GameObject> currenTiles = new List<GameObject>();
        if (tile1.isColumnBomb)
        {
            allMatches.Union(GetColumnTiles(tile1.column));
            BoardManager.instance.BombRow(tile1.column);
        }
        if (tile2.isColumnBomb)
        {
            allMatches.Union(GetColumnTiles(tile2.column));
            BoardManager.instance.BombRow(tile2.column);
        }
        if (tile3.isColumnBomb)
        {
            allMatches.Union(GetColumnTiles(tile3.column));
            BoardManager.instance.BombRow(tile3.column);
        }
        return currenTiles;
    }

    private IEnumerator FindMatchCo()
    {
        for (int x = 0; x < BoardManager.instance.xSize; x++)
        {
            for (int y = 0; y < BoardManager.instance.ySize; y++)
            {
                GameObject currentTile = BoardManager.instance.allTiles[x, y];
                if (currentTile != null && currentTile != BoardManager.instance.lockTiles[x, y])
                {
                    if (x > 0 && x < BoardManager.instance.xSize - 1)
                    {
                        GameObject leftTile = BoardManager.instance.allTiles[x - 1, y];
                        GameObject rightTile = BoardManager.instance.allTiles[x + 1, y];
                        if (leftTile != null && rightTile != null)
                        {
                            if (leftTile.GetComponent<SpriteRenderer>().sprite == currentTile.GetComponent<SpriteRenderer>().sprite && rightTile.GetComponent<SpriteRenderer>().sprite == currentTile.GetComponent<SpriteRenderer>().sprite)
                            {
                                allMatches.Union(IsRowBomb(currentTile.GetComponent<Tiles>(), leftTile.GetComponent<Tiles>(), rightTile.GetComponent<Tiles>()));
                                allMatches.Union(IsColumnBomb(currentTile.GetComponent<Tiles>(), leftTile.GetComponent<Tiles>(), rightTile.GetComponent<Tiles>()));
                                allMatches.Union(IsAdjacentBomb(currentTile.GetComponent<Tiles>(), leftTile.GetComponent<Tiles>(), rightTile.GetComponent<Tiles>()));
                                GetNearbyTile(leftTile, currentTile, rightTile);

                            }
                        }
                    }

                    if (y > 0 && y < BoardManager.instance.ySize - 1)
                    {
                        GameObject upTile = BoardManager.instance.allTiles[x, y + 1];
                        GameObject downTile = BoardManager.instance.allTiles[x, y - 1];

                        if (upTile != null && downTile != null)
                        {
                            if (upTile.GetComponent<SpriteRenderer>().sprite == currentTile.GetComponent<SpriteRenderer>().sprite && downTile.GetComponent<SpriteRenderer>().sprite == currentTile.GetComponent<SpriteRenderer>().sprite)
                            {

                                allMatches.Union(IsColumnBomb(upTile.GetComponent<Tiles>(), currentTile.GetComponent<Tiles>(), downTile.GetComponent<Tiles>()));
                                allMatches.Union(IsRowBomb(upTile.GetComponent<Tiles>(), currentTile.GetComponent<Tiles>(), downTile.GetComponent<Tiles>()));
                                allMatches.Union(IsAdjacentBomb(upTile.GetComponent<Tiles>(), currentTile.GetComponent<Tiles>(), downTile.GetComponent<Tiles>()));
                                GetNearbyTile(currentTile, upTile, downTile);
                            }
                        }

                    }
                }
            }
        }
        yield return null;
    }

    public void MatchPicesOfTile(Sprite sprite)
    {
        for (int x = 0; x < BoardManager.instance.xSize; x++)
        {
            for (int y = 0; y < BoardManager.instance.ySize; y++)
            {
                if (BoardManager.instance.allTiles[x, y] != null)
                {
                    if (BoardManager.instance.allTiles[x, y].GetComponent<SpriteRenderer>().sprite == sprite)
                    {
                        BoardManager.instance.allTiles[x, y].GetComponent<Tiles>().isMatched = true;
                    }
                }
            }
        }
    }

    private List<GameObject> GetAdjacentTiles(int column, int row)
    {
        List<GameObject> tiles = new List<GameObject>();
        for (int x = column - 1; x <= column + 1; x++)
        {
            for (int y = row - 1; y <= row + 1; y++)
            {
                if (x >= 0 && x < BoardManager.instance.xSize && y >= 0 && y < BoardManager.instance.ySize)
                {
                    Tiles tile = BoardManager.instance.allTiles[x, y].GetComponent<Tiles>();
                    if (BoardManager.instance.allTiles[x, y] != null)
                    {
                        if (tile.isColumnBomb)
                        {
                            tiles.Union(GetColumnTiles(x).ToList());
                        }
                        else if (tile.isRowBomb)
                        {
                            tiles.Union(GetRowTiles(y).ToList());
                        }
                        else if (tile.isColorBomb)
                        {
                            MatchPicesOfTile(tile.GetComponent<SpriteRenderer>().sprite);
                        }
                        else
                        {
                            tiles.Add(BoardManager.instance.allTiles[x, y]);
                            tile.isMatched = true;
                        }
                    }

                }
            }
        }
        return tiles;
    }

    private List<GameObject> GetColumnTiles(int column)
    {
        List<GameObject> tiles = new List<GameObject>();
        for (int y = 0; y < BoardManager.instance.ySize; y++)
        {
            if (BoardManager.instance.allTiles[column, y] != null)
            {
                Tiles tile = BoardManager.instance.allTiles[column, y].GetComponent<Tiles>();
                if (tile.isRowBomb)
                {
                    tiles.Union(GetRowTiles(y).ToList());
                }
                else if (tile.isAdjacentBomb)
                {
                    tiles.Union(GetAdjacentTiles(column, y).ToList());
                }
                else if (tile.isColorBomb)
                {
                    MatchPicesOfTile(tile.GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    tiles.Add(BoardManager.instance.allTiles[column, y]);
                    tile.isMatched = true;
                }
            }
        }
        return tiles;
    }

    private List<GameObject> GetRowTiles(int row)
    {
        List<GameObject> tiles = new List<GameObject>();
        for (int x = 0; x < BoardManager.instance.xSize; x++)
        {
            if (BoardManager.instance.allTiles[x, row] != null)
            {
                Tiles tile = BoardManager.instance.allTiles[x, row].GetComponent<Tiles>();
                if (tile.isColumnBomb)
                {
                    tiles.Union(GetColumnTiles(x).ToList());
                }
                else if (tile.isAdjacentBomb)
                {
                    tiles.Union(GetAdjacentTiles(x, row).ToList());
                }
                else if (tile.isColorBomb)
                {
                    MatchPicesOfTile(tile.GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    tiles.Add(BoardManager.instance.allTiles[x, row]);
                    tile.isMatched = true;
                }

            }
        }
        return tiles;
    }

    public void CheckBombs(MatchType matchType)
    {
        if (BoardManager.instance.currentTile != null)
        {
            if (BoardManager.instance.currentTile.isMatched && BoardManager.instance.currentTile.GetComponent<SpriteRenderer>().sprite == matchType.sprite)
            {
                BoardManager.instance.currentTile.isMatched = false;
                if ((BoardManager.instance.currentTile.angleDir > -45 && BoardManager.instance.currentTile.angleDir <= 45)
                    || (BoardManager.instance.currentTile.angleDir < -135 || BoardManager.instance.currentTile.angleDir >= 135))
                {
                    BoardManager.instance.currentTile.MakeRowBomb();
                }
                else
                {
                    BoardManager.instance.currentTile.MakeColumnBomb();
                }

            }
        }
        else if (BoardManager.instance.currentTile.otherPrefab != null)
        {
            Tiles otherTile = BoardManager.instance.currentTile.otherPrefab.GetComponent<Tiles>();
            if (otherTile.isMatched && otherTile.GetComponent<SpriteRenderer>().sprite == matchType.sprite)
            {
                otherTile.isMatched = false;

                if ((BoardManager.instance.currentTile.angleDir > -45 && BoardManager.instance.currentTile.angleDir <= 45)
                    || (BoardManager.instance.currentTile.angleDir < -135 || BoardManager.instance.currentTile.angleDir >= 135))
                {
                    otherTile.MakeRowBomb();
                }
                else
                {
                    otherTile.MakeColumnBomb();
                }
            }
        }
    }
}
