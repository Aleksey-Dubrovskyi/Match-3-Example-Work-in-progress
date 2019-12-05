using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    public GameState gameState = GameState.move;

    public int xSize, ySize, offset;

    [SerializeField]
    GameObject tilePrefab;
    [SerializeField]
    List<Sprite> characters = new List<Sprite>();

    public GameObject[,] allTiles;
    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<BoardManager>();
        allTiles = new GameObject[xSize, ySize];
        SetUp();
    }

    void SetUp()
    {
        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
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

    void DestroyMatchesAt(int column, int row)
    {
        if (allTiles[column, row].GetComponent<Tiles>().isMatched)
        {
            FindMatch.instance.allMatches.Remove(allTiles[column, row]);
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
        StartCoroutine(CollapsingCo());
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
                if (allTiles[x, y] == null)
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

        yield return new WaitForSeconds(.1f);
        gameState = GameState.move;
        
    }
}
