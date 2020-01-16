using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    pause,
    lose,
    win
}

public enum TyleKind
{
    Breakable,
    Blank,
    Normal,
    Locktile,
    Iceblock,
    Chocolate
}

[System.Serializable]
public class MatchType
{
    public int type;
    public Sprite sprite;
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
    [Header("Scriptable object variables")]
    public int level;
    public World world;

    [Header("Layout")]
    public TyleType[] boardLayout;
    public bool isShifring = true;
    [SerializeField]
    private GameObject destroyEffect;

    [Header("Score System")]
    [SerializeField]
    public int[] scoreGoals;
    [SerializeField]
    private float refilDelay = 0.5f;
    [SerializeField]
    private int scoreForOneTile;
    private int streakValue = 1;

    public GameState gameState = GameState.move;

    [Header("Board Demensions")]
    public int xSize;
    public int ySize;
    public int offset;

    [Header("Tile type")]
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject breakcableTile;
    [SerializeField]
    private GameObject lockTilePrefab;
    [SerializeField]
    private GameObject iceTilePrefab;
    [SerializeField]
    private GameObject chocolatePrefab;
    private bool makeChocolate = true;
    [SerializeField]
    private List<Sprite> characters = new List<Sprite>();


    public Tiles currentTile;

    [Header("Tyle arrays")]
    private BackgroundTile[,] breakbleTiles;
    public GameObject[,] allTiles;
    public BackgroundTile[,] lockTiles;
    private BackgroundTile[,] iceblockTiles;
    private BackgroundTile[,] chocolateTiles;
    private bool[,] blankSpaces;

    public MatchType matchType;

    private void Awake()
    {
        instance = GetComponent<BoardManager>();
        gameState = GameState.pause;
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    xSize = world.levels[level].xSize;
                    ySize = world.levels[level].ySize;
                    characters = world.levels[level].characterTiles;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }

    private void Start()
    {
        breakbleTiles = new BackgroundTile[xSize, ySize];
        lockTiles = new BackgroundTile[xSize, ySize];
        iceblockTiles = new BackgroundTile[xSize, ySize];
        chocolateTiles = new BackgroundTile[xSize, ySize];
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

    private void GenerateLockTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TyleKind.Locktile)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateIceblockTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TyleKind.Iceblock)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(iceTilePrefab, tempPosition, Quaternion.identity);
                iceblockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateChocolateTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TyleKind.Chocolate)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(chocolatePrefab, tempPosition, Quaternion.identity);
                chocolateTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void SetUp()
    {
        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;
        GenerateBlankSpaces();
        GenerateBreakbleTiles();
        GenerateLockTiles();
        GenerateIceblockTiles();
        GenerateChocolateTiles();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (!blankSpaces[x, y] && !iceblockTiles[x, y] && !chocolateTiles[x, y])
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
        StartCoroutine(SetUpDeadLock());
    }

    private IEnumerator SetUpDeadLock()
    {
        yield return new WaitForSeconds(refilDelay);
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
    }

    private MatchType ColumnOrRow()
    {
        List<GameObject> matchCopy = FindMatch.instance.allMatches as List<GameObject>;

        matchType.type = 0;
        matchType.sprite = null;

        for (int i = 0; i < matchCopy.Count; i++)
        {
            Tiles thisTile = matchCopy[i].GetComponent<Tiles>();
            Sprite sprite = matchCopy[i].GetComponent<SpriteRenderer>().sprite;
            int column = thisTile.column;
            int row = thisTile.row;
            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchCopy.Count; j++)
            {
                Tiles nextTile = matchCopy[j].GetComponent<Tiles>();

                if (nextTile == thisTile)
                {
                    continue;
                }

                //Sprite nextTileSprite = nextTile.GetComponent<SpriteRenderer>().sprite;

                if (nextTile.column == thisTile.column && nextTile.GetComponent<SpriteRenderer>().sprite == sprite)
                {
                    columnMatch++;
                }

                if (nextTile.row == thisTile.row && nextTile.GetComponent<SpriteRenderer>().sprite == sprite)
                {
                    rowMatch++;
                }
            }

            // 1 == Color Bomb
            // 2 == Adjacent Bomb
            // 3 = Column or row Bomb

            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.sprite = sprite;
                return matchType;
            }
            else if (columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.sprite = sprite;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.sprite = sprite;
                return matchType;
            }


        }
        matchType.type = 0;
        matchType.sprite = null;
        return matchType;
    }

    private void CheckToMakeBombs()
    {
        if (FindMatch.instance.allMatches.Count > 3)
        {
            MatchType typeOfMatch = ColumnOrRow();
            switch (typeOfMatch.type)
            {
                case 1:
                    //Column or Row Bomb
                    if (currentTile != null && currentTile.isMatched && currentTile.GetComponent<SpriteRenderer>().sprite == typeOfMatch.sprite)
                    {
                        currentTile.isMatched = false;
                        currentTile.MakeColorBomb();
                    }
                    else
                    {
                        if (currentTile.otherPrefab != null)
                        {
                            Tiles otherTile = currentTile.otherPrefab.GetComponent<Tiles>();
                            if (otherTile.isMatched && otherTile.GetComponent<SpriteRenderer>().sprite == typeOfMatch.sprite)
                            {
                                otherTile.isMatched = false;
                                otherTile.MakeColorBomb();
                            }
                        }
                    }

                    break;

                case 2:
                    //Adjacent Bomb
                    if (currentTile != null && currentTile.isMatched && currentTile.GetComponent<SpriteRenderer>().sprite == typeOfMatch.sprite)
                    {
                        currentTile.isMatched = false;
                        currentTile.MakeAdjacentBomb();
                    }

                    else if (currentTile.otherPrefab != null)
                    {
                        Tiles otherTile = currentTile.otherPrefab.GetComponent<Tiles>();
                        if (otherTile.isMatched && currentTile.GetComponent<SpriteRenderer>().sprite == typeOfMatch.sprite)
                        {
                            otherTile.isMatched = false;
                            otherTile.MakeAdjacentBomb();
                        }
                    }

                    break;
                case 3:
                    //Color Bomb
                    FindMatch.instance.CheckBombs(matchType);
                    break;
            }
        }
    }

    public void BombRow(int row)
    {
        for (int x = 0; x < xSize; x++)
        {
            if (iceblockTiles[x, row])
            {
                iceblockTiles[x, row].TakeDamage(1);
                if (iceblockTiles[x, row].hitPoints <= 0)
                {
                    iceblockTiles[x, row] = null;
                }
            }
        }
    }

    public void BombColumn(int column)
    {
        for (int x = 0; x < xSize; x++)
        {
            if (iceblockTiles[column, x])
            {
                iceblockTiles[column, x].TakeDamage(1);
                if (iceblockTiles[column, x].hitPoints <= 0)
                {
                    iceblockTiles[column, x] = null;
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allTiles[column, row].GetComponent<Tiles>().isMatched)
        {
            if (breakbleTiles[column, row] != null)
            {
                //There is a damage to brakable tile. Caution its a magic number
                breakbleTiles[column, row].TakeDamage(1);
                if (breakbleTiles[column, row].hitPoints <= 0)
                {
                    breakbleTiles[column, row] = null;
                }
            }
            if (lockTiles[column, row] != null)
            {
                //There is a damage to brakable tile. Caution its a magic number
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }
            }

            DamageIceblock(column, row);
            DamageChocolate(column, row);

            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.CompareGoal(allTiles[column, row].GetComponent<SpriteRenderer>().sprite);
                GoalManager.Instance.UpdateGoals();
            }
            GameObject particlePrefab = Instantiate(destroyEffect, allTiles[column, row].transform.position, Quaternion.identity);
            Destroy(particlePrefab, .5f);
            Destroy(allTiles[column, row]);
            GUIManager.instance.Score += (scoreForOneTile * streakValue);
            if (SFXManager.Instance.isActiveAndEnabled)
            {
                SFXManager.Instance.PlaySFX(Clip.Clear);
            }
            allTiles[column, row] = null;
        }
    }

    private void DamageIceblock(int column, int row)
    {
        if (column > 0)
        {
            if (iceblockTiles[column - 1, row])
            {
                iceblockTiles[column - 1, row].TakeDamage(1);
                if (iceblockTiles[column - 1, row].hitPoints <= 0)
                {
                    iceblockTiles[column - 1, row] = null;
                }
            }
        }
        if (column < xSize - 1)
        {
            if (iceblockTiles[column + 1, row])
            {
                iceblockTiles[column + 1, row].TakeDamage(1);
                if (iceblockTiles[column + 1, row].hitPoints <= 0)
                {
                    iceblockTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (iceblockTiles[column, row - 1])
            {
                iceblockTiles[column, row - 1].TakeDamage(1);
                if (iceblockTiles[column, row - 1].hitPoints <= 0)
                {
                    iceblockTiles[column, row - 1] = null;
                }
            }
        }
        if (row < ySize - 1)
        {
            if (iceblockTiles[column, row + 1])
            {
                iceblockTiles[column, row + 1].TakeDamage(1);
                if (iceblockTiles[column, row + 1].hitPoints <= 0)
                {
                    iceblockTiles[column, row + 1] = null;
                }
            }
        }
    }

    private void DamageChocolate(int column, int row)
    {
        if (column > 0)
        {
            if (chocolateTiles[column - 1, row])
            {
                chocolateTiles[column - 1, row].TakeDamage(1);
                if (chocolateTiles[column - 1, row].hitPoints <= 0)
                {
                    chocolateTiles[column - 1, row] = null;
                }
                makeChocolate = false;
            }
        }
        if (column < xSize - 1)
        {
            if (chocolateTiles[column + 1, row])
            {
                chocolateTiles[column + 1, row].TakeDamage(1);
                if (chocolateTiles[column + 1, row].hitPoints <= 0)
                {
                    chocolateTiles[column + 1, row] = null;
                }
                makeChocolate = false;
            }
        }
        if (row > 0)
        {
            if (chocolateTiles[column, row - 1])
            {
                chocolateTiles[column, row - 1].TakeDamage(1);
                if (chocolateTiles[column, row - 1].hitPoints <= 0)
                {
                    chocolateTiles[column, row - 1] = null;
                }
                makeChocolate = false;
            }
        }
        if (row < ySize - 1)
        {
            if (chocolateTiles[column, row + 1])
            {
                chocolateTiles[column, row + 1].TakeDamage(1);
                if (chocolateTiles[column, row + 1].hitPoints <= 0)
                {
                    chocolateTiles[column, row + 1] = null;
                }
                makeChocolate = false;
            }
        }
    }

    public void DestroyMatches()
    {
        if (FindMatch.instance.allMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
        FindMatch.instance.allMatches.Clear();
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
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (!blankSpaces[x, y] && allTiles[x, y] == null && !iceblockTiles[x, y] && !chocolateTiles[x, y])
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
        yield return new WaitForSeconds(refilDelay * 0.5f);
        StartCoroutine(FillBoard());
    }

    private void RefillBoard()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] == null && !blankSpaces[x, y] && !iceblockTiles[x, y] && !chocolateTiles[x, y])
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

    public bool MatchesOnBoard()
    {
        FindMatch.instance.FindAllmatches();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
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

    private IEnumerator FillBoard()
    {
        isShifring = true;
        yield return new WaitForSeconds(refilDelay);
        RefillBoard();
        yield return new WaitForSeconds(refilDelay);

        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
        }

        currentTile = null;
        CheckToMakeChocolate();

        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }

        yield return new WaitForSeconds(refilDelay);
        if (gameState != GameState.lose)
        {
            gameState = GameState.move;
        }
        makeChocolate = true;
        streakValue = 1;
        isShifring = false;
    }

    private void CheckToMakeChocolate()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (chocolateTiles[x, y] != null && makeChocolate)
                {
                    MakeNewChocolate();
                    return;
                }
            }
        }
    }

    private Vector2 CheckForAdjacent(int column, int row)
    {
        if (allTiles[column + 1, row] && column < xSize - 1)
        {
            return Vector2.right;
        }
        if (allTiles[column - 1, row] && column > 0)
        {
            return Vector2.left;
        }
        if (allTiles[column, row + 1] && row < ySize - 1)
        {
            return Vector2.up;
        }
        if (allTiles[column, row - 1] && row > 0)
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }

    private void MakeNewChocolate()
    {
        bool chocolate = false;
        int loops = 0;

        while (!chocolate && loops < 200)
        {
            int newX = Random.Range(0, xSize);
            int newY = Random.Range(0, ySize);

            if (chocolateTiles[newX, newY])
            {
                Vector2 adjacent = CheckForAdjacent(newX, newY);
                if (adjacent != Vector2.zero)
                {
                    Destroy(allTiles[newX + (int)adjacent.x, newY + (int)adjacent.y]);
                    Vector2 tempPos = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y);
                    GameObject tile = Instantiate(chocolatePrefab, tempPos, Quaternion.identity);
                    chocolateTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTile>();
                    chocolate = true;
                }
            }
            loops++;
        }
    }

    //Deadlock section
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        if (allTiles[column + (int)direction.x, row + (int)direction.y] != null)
        {
            GameObject holder = allTiles[column + (int)direction.x, row + (int)direction.y] as GameObject;

            allTiles[column + (int)direction.x, row + (int)direction.y] = allTiles[column, row];
            allTiles[column, row] = holder;
        }
    }

    private bool CheckForMathces()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] != null && allTiles[x, y] != lockTiles[x, y])
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

    private bool IsDeadlocked()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] != null)
                {
                    if (x < xSize - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.right))
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

    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(0.5f);
        List<GameObject> newBoard = new List<GameObject>();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (allTiles[x, y] != null)
                {
                    newBoard.Add(allTiles[x, y]);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (!blankSpaces[x, y] && !iceblockTiles[x, y] && !chocolateTiles[x, y])
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

        StartCoroutine(DestroyingMatchesForDeadLock());

        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
    }

    private IEnumerator DestroyingMatchesForDeadLock()
    {
        yield return new WaitForSeconds(refilDelay);
        FindMatch.instance.FindAllmatches();
        yield return new WaitForSeconds(refilDelay);

        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
        }
    }
}
