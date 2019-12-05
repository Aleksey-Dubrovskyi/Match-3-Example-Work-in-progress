using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    GameObject otherPrefab;

    Vector2 firstTouchPosition;
    Vector2 secondTouchPosition;
    Vector2 tempPos;

    public bool isMatched = false;

    float angleDir = 0f;
    float swipeResit = 1f;

    public int column, row;
    int previousRow, previousColumn;
    int startX, startY;

    void Start()
    {
        //startX = (int)transform.position.x;
        //startY = (int)transform.position.y;
        ////row = startY;
        ////column = startX;
        ////previousRow = row;
        ////previousColumn = column;
    }

    void Update()
    {
        //FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySPrite = GetComponent<SpriteRenderer>();
            mySPrite.color = new Color(1f, 1f, 1f, .4f);
        }
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
            angleDir = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y, secondTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePosition();
            BoardManager.instance.gameState = GameState.wait;
        }
        else
        {
            BoardManager.instance.gameState = GameState.move;
        }
       
    }

    void MovePosition()
    {
        if (angleDir > -45 && angleDir <= 45 && column < BoardManager.instance.xSize - 1)
        {
            //Right Swipe
            otherPrefab = BoardManager.instance.allTiles[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherPrefab.GetComponent<Tiles>().column -= 1;
            column += 1;
        }
        else if (angleDir > 45 && angleDir <= 135 && row < BoardManager.instance.ySize - 1)
        {
            //Up Swipe
            otherPrefab = BoardManager.instance.allTiles[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherPrefab.GetComponent<Tiles>().row -= 1;
            row += 1;
        }
        else if ((angleDir > 135 || angleDir <= -135) && column > 0)
        {
            //Left Swipe
            otherPrefab = BoardManager.instance.allTiles[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherPrefab.GetComponent<Tiles>().column += 1;
            column -= 1;
        }
        else if (angleDir < -45 && angleDir >= -135 && row > 0)
        {
            //Down Swipe
            otherPrefab = BoardManager.instance.allTiles[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherPrefab.GetComponent<Tiles>().row += 1;
            row -= 1;
        }
        StartCoroutine(PrefabCo());
    }

    IEnumerator PrefabCo()
    {
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
                BoardManager.instance.gameState = GameState.move;
            }
            else
            {
                BoardManager.instance.DestroyMatches();
            }
            otherPrefab = null;
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
}
