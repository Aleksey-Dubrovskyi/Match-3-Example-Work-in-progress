using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public GameObject otherPrefab;
    public GameObject arrowPrefab;
    public GameObject colorBomb;
    public GameObject adjacentBomb;

    Vector2 firstTouchPosition;
    Vector2 secondTouchPosition;
    Vector2 tempPos;

    public bool isMatched;
    public bool isColorBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    public bool isColumnBomb;

    public float angleDir = 0f;
    float swipeResit = 1f;

    public int column, row;
    int previousRow, previousColumn;
    int startX, startY;

    void Start()
    {
        isMatched = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        isRowBomb = false;
        isColumnBomb = false;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MakeColorBomb();
        }
    }

    void Update()
    {
        

        startX = column;
        startY = row;
        if (Mathf.Abs(startX - transform.position.x) > 0.1f)
        {
            //Move towards
            tempPos = new Vector2(startX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.6f);
            if (BoardManager.instance.allTiles[column, row] != this.gameObject)
            {
                BoardManager.instance.allTiles[column, row] = this.gameObject;
            }
            FindMatch.instance.FindAllmatches();
        }
        else
        {
            tempPos = new Vector2(startX, transform.position.y);
            transform.position = tempPos;
            if (BoardManager.instance.allTiles[column, row] != this.gameObject)
            {
                BoardManager.instance.allTiles[column, row] = this.gameObject;
            }
            BoardManager.instance.allTiles[column, row] = this.gameObject;
        }
        if (Mathf.Abs(startY - transform.position.y) > 0.1f)
        {
            //Move towards
            tempPos = new Vector2(transform.position.x, startY);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.6f);
            FindMatch.instance.FindAllmatches();
        }
        else
        {
            tempPos = new Vector2(transform.position.x, startY);
            transform.position = tempPos;
            BoardManager.instance.allTiles[column, row] = this.gameObject;
        }
    }

    void OnMouseDown()
    {
        if (BoardManager.instance.gameState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void OnMouseUp()
    {
        if (BoardManager.instance.gameState == GameState.move)
        {
            secondTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(secondTouchPosition.y - firstTouchPosition.y) > swipeResit || Mathf.Abs(secondTouchPosition.x - firstTouchPosition.x) > swipeResit)
        {
            BoardManager.instance.gameState = GameState.wait;
            angleDir = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y, secondTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePosition();
            BoardManager.instance.currentTile = this;
        }
        else
        {
            BoardManager.instance.gameState = GameState.move;
        }
       
    }

    void CalculationOfPosition(Vector2 direction)
    {
        otherPrefab = BoardManager.instance.allTiles[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        otherPrefab.GetComponent<Tiles>().column += -1 * (int)direction.x;
        otherPrefab.GetComponent<Tiles>().row += -1 * (int)direction.y;
        column += (int)direction.x;
        row += (int)direction.y;
        StartCoroutine(PrefabCo());
    }

    void MovePosition()
    {
        if (angleDir > -45 && angleDir <= 45 && column < BoardManager.instance.xSize - 1)
        {
            //Right Swipe
            CalculationOfPosition(Vector2.right);
        }
        else if (angleDir > 45 && angleDir <= 135 && row < BoardManager.instance.ySize - 1)
        {
            //Up Swipe
            CalculationOfPosition(Vector2.up);
        }
        else if ((angleDir > 135 || angleDir <= -135) && column > 0)
        {
            //Left Swipe
            CalculationOfPosition(Vector2.left);
        }
        else if (angleDir < -45 && angleDir >= -135 && row > 0)
        {
            //Down Swipe
            CalculationOfPosition(Vector2.down);
        }
        BoardManager.instance.gameState = GameState.move;
    }

    IEnumerator PrefabCo()
    {
        if (isColorBomb)
        {
            FindMatch.instance.MatchPicesOfTile(otherPrefab.GetComponent<SpriteRenderer>().sprite);
            isMatched = true;
        }
        else if (otherPrefab.GetComponent<Tiles>().isColorBomb)
        {
            FindMatch.instance.MatchPicesOfTile(this.gameObject.GetComponent<SpriteRenderer>().sprite);
            otherPrefab.GetComponent<Tiles>().isMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if (otherPrefab != null)
        {
            if (!isMatched && !otherPrefab.GetComponent<Tiles>().isMatched)
            {
                otherPrefab.GetComponent<Tiles>().row = row;
                otherPrefab.GetComponent<Tiles>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                BoardManager.instance.currentTile = null;
                BoardManager.instance.gameState = GameState.move;
            }
            else
            {
                BoardManager.instance.DestroyMatches();
            }
        }
    }

    void FindMatches()
    {
        if (column > 0 && column < BoardManager.instance.xSize - 1)
        {
            GameObject leftPrefab = BoardManager.instance.allTiles[column - 1, row];
            GameObject rightPrefab = BoardManager.instance.allTiles[column + 1, row];
            if (leftPrefab != null & rightPrefab != null)
            {
                if (leftPrefab.GetComponent<SpriteRenderer>().sprite == this.gameObject.GetComponent<SpriteRenderer>().sprite && rightPrefab.GetComponent<SpriteRenderer>().sprite == this.gameObject.GetComponent<SpriteRenderer>().sprite)
                {
                    leftPrefab.GetComponent<Tiles>().isMatched = true;
                    rightPrefab.GetComponent<Tiles>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < BoardManager.instance.ySize - 1)
        {
            GameObject upPrefab = BoardManager.instance.allTiles[column, row + 1];
            GameObject downPrefab  = BoardManager.instance.allTiles[column, row - 1];
            if (upPrefab != null && downPrefab != null)
            {
                if (upPrefab.GetComponent<SpriteRenderer>().sprite == this.gameObject.GetComponent<SpriteRenderer>().sprite && downPrefab.GetComponent<SpriteRenderer>().sprite == this.gameObject.GetComponent<SpriteRenderer>().sprite)
                {
                    upPrefab.GetComponent<Tiles>().isMatched = true;
                    downPrefab.GetComponent<Tiles>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
    public void MakeRowBomb()
    {
        isRowBomb = true;
        //this.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        //this.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, 90));
        arrow.transform.parent = this.transform;
    }

    public void  MakeColorBomb()
    {
        isColorBomb = true;
        GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
        color.transform.parent = this.transform;
    }

    public void MakeAdjacentBomb()
    {
        isAdjacentBomb = true;
        GameObject adjacent = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
        adjacent.transform.parent = this.transform;
    }
}
