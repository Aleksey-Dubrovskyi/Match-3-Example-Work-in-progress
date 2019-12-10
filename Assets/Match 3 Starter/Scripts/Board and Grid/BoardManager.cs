using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public enum TyleKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TyleType
{
    public int x;
    public int y;
    public TyleKind tileKind;
}

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    public TyleType[] boardLayout;

    [SerializeField]
    GameObject destroyEffect;

    public GameState gameState = GameState.move;

    public int xSize, ySize, offset;

    [SerializeField]
    GameObject tilePrefab;
    [SerializeField]
    GameObject breakcableTile;
    [SerializeField]
    List<Sprite> characters = new List<Sprite>();

    public Tiles currentTile;
    BackgroundTile[,] breakbleTiles;
    public GameObject[,] allTiles;
    private bool[,] blankSpaces;
    // Start is called before the first frame update
    void Start()
    {
        breakbleTiles = new BackgroundTile[xSize, ySize];
        instance = GetComponent<BoardManager>();
        blankSpaces = new bool[xSize, ySize];
        allTiles = new GameObject[xSize, ySize];
        SetUp();
    }

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TyleKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakbleTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TyleKind.Breakable)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakcableTile, tempPosition, Quaternion.identity);
                breakbleTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    void SetUp()
    {
        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;
        GenerateBlankSpaces();
        GenerateBreakbleTiles();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (!blankSpaces[x, y])
                {
                    Vector2 tempPos = new Vector2(x, y + offset);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity);
                    backgroundTile.GetComponent<Tiles>().row = y;
                    backgroundTile.GetComponent<Tiles>().column = x;
                    backgroundTile.name = $"{tilePrefab.name}_{x}_{y}";
                    backgroundTile.transform.parent = transform;
                    List<Sprite> possibleCharacters = new List<Sprite>();
                    possibleCharacters.AddRange(characters);

                    possibleCharacters.Remove(previousLeft[y]);
                    possibleCharacters.Remove(previousBelow);
                    Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
                    backgroundTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                    previousLeft[y] = newSprite;
                    previousBelow = newSprite;
                }
            }
        }
    }

    bool ColumnOrRow()
    {
        int numberHorizontal = 0, numberVertical = 0;
        Tiles firstTile = FindMatch.instance.allMatches[0].GetComponent<Tiles>();
        if (firstTile != null)
        {
            foreach (var currentTile in FindMatch.instance.allMatches)
            {
                Tiles tile = currentTile.GetComponent<Tiles>();
                if (tile.row == firstTile.row)
                {
                    numberHorizontal++;
                }
                if (tile.column == firstTile.column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
    }

    void CheckToMakeBombs()
    {
        if (FindMatch.instance.allMatches.Count == 4 || FindMatch.instance.allMatches.Count == 7)
        {
            FindMatch.instance.CheckBombs();
        }
        if (FindMatch.instance.allMatches.Count == 5 || FindMatch.instance.allMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                if (currentTile != null)
                {
                    if (currentTile.isMatched)
                    {
                        if (!currentTile.isColorBomb)
                        {
                            currentTile.isMatched = false;
                            currentTile.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentTile.otherPrefab != null)
                        {
                            Tiles otherTile = currentTile.otherPrefab.GetComponent<Tiles>();
                            if (otherTile.isMatched)
                            {
                                if (!otherTile.isColorBomb)
                                {
                                    otherTile.isMatched = false;
                                    otherTile.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentTile != null)
                {
                    if (currentTile.isMatched)
                    {
                        if (!currentTile.isAdjacentBomb)
                        {
                            currentTile.isMatched = false;
                            currentTile.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentTile.otherPrefab != null)
                        {
                            Tiles otherTile = currentTile.otherPrefab.GetComponent<Tiles>();
                            if (otherTile.isMatched)
                            {
                                if (!otherTile.isAdjacentBomb)
                                {
                                    otherTile.isMatched = false;
                                    otherTile.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void DestroyMatchesAt(int column, int row)
    {
        if (allTiles[column, row].GetComponent<Tiles>().isMatched)
        {
            if (FindMatch.instance.allMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }
            if (breakbleTiles[column, row])
            {
                //There is a damage to brakable tile. Caution its a magic number
                breakbleTiles[column, row].TakeDamage(1);
                if (breakbleTiles[column, row].hitPoints <= 0)
                {
                    breakbleTiles[column, row] = null;
                }
            }
            GameObject particlePrefab = Instantiate(destroyEffect, allTiles[column, row].transform.position, Quaternion.identity);
            Destroy(particlePrefab, .5f);
            Destroy(allTiles[column, row]);
            allTiles[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] != null)
                {
                    DestroyMatchesAt(x, y);
                }
            }
        }
        FindMatch.instance.allMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    IEnumerator DecreaseRowCo()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < xSize; y++)
            {
                if (!blankSpaces[x,y] && allTiles[x,y] == null)
                {
                    for (int i = y + 1; i < ySize; i++)
                    {
                        if (allTiles[x, i] != null)
                        {
                            allTiles[x, i].GetComponent<Tiles>().row = y;
                            allTiles[x, i] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    IEnumerator CollapsingCo()
    {
        int nullCounter = 0;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allTiles[x, y].GetComponent<Tiles>().row -= nullCounter;
                    allTiles[x, y] = null;
                }
            }
            nullCounter = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }
    
    void RefillBoard()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] == null && !blankSpaces[x, y])
                {
                    Vector2 tempPos = new Vector2(x, y + offset);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity);
                    backgroundTile.GetComponent<Tiles>().row = y;
                    backgroundTile.GetComponent<Tiles>().column = x;
                    allTiles[x, y] = backgroundTile;
                    backgroundTile.name = $"{tilePrefab.name}_{x}_{y}";
                    backgroundTile.transform.parent = transform;
                    List<Sprite> possibleCharacters = new List<Sprite>();
                    possibleCharacters.AddRange(characters);
                    Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
                    backgroundTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                }
            }
        }
    }

    bool MatchesOnBoard()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < xSize; y++)
            {
                if (allTiles[x, y] != null)
                {
                    if (allTiles[x, y].GetComponent<Tiles>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        FindMatch.instance.allMatches.Clear();
        yield return new WaitForSeconds(.1f);
        if (IsDeadlocked())
        {

        }
        gameState = GameState.move;
        
    }

    //Deadlock section
    void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject holder = allTiles[column + (int)direction.x, row + (int)direction.y];

        allTiles[column + (int)direction.x, row + (int)direction.y] = allTiles[column, row];
        allTiles[column, row] = holder;
    }

    bool CheckForMathces()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] != null)
                {
                    if (x < xSize - 2)
                    {
                        if (allTiles[x + 1, y] != null && allTiles[x + 2, y] != null)
                        {
                            if (allTiles[x + 1, y].GetComponent<SpriteRenderer>().sprite ==
                                allTiles[x, y].GetComponent<SpriteRenderer>().sprite &&
                                allTiles[x + 2, y].GetComponent<SpriteRenderer>().sprite ==
                                allTiles[x, y].GetComponent<SpriteRenderer>().sprite)
                            {
                                return true;
                            }
                        }
                    }
                    if (y < ySize - 2)
                    {
                        if (allTiles[x, y + 1] != null && allTiles[x, y + 2] != null)
                        {
                            if (allTiles[x, y + 1].GetComponent<SpriteRenderer>().sprite ==
                                allTiles[x, y].GetComponent<SpriteRenderer>().sprite &&
                                allTiles[x, y + 2].GetComponent<SpriteRenderer>().sprite ==
                                allTiles[x, y].GetComponent<SpriteRenderer>().sprite)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMathces())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    bool IsDeadlocked()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] != null)
                {
                    if (x < xSize - 1)
                    {
                        if(SwitchAndCheck(x, y, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (y < ySize - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < xSize; y++)
            {
                if (allTiles[x, y] != null)
                {
                    newBoard.Add(allTiles[x, y]);
                }
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < xSize; y++)
            {
                if (!blankSpaces[x, y])
                {
                    int tileToUse = Random.Range(0, newBoard.Count);
                    Tiles tile = newBoard[tileToUse].GetComponent<Tiles>();
                    tile.column = x;
                    tile.row = y;
                    allTiles[x, y] = newBoard[tileToUse];
                    newBoard.Remove(newBoard[tileToUse]);
                }
            }
        }
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }
}
