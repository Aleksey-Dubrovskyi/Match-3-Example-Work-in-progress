using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile firstSelect;
    private static Tile secondSelect;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() {
		render = GetComponent<SpriteRenderer>();
    }

    private void CurrentSelect()
    {
//        render.color = selectedColor;
        secondSelect = gameObject.GetComponent<Tile>();
        SFXManager.instance.PlaySFX(Clip.Select);
    }

    private void DeselectSecond()
    {
        secondSelect = null;
        SFXManager.instance.PlaySFX(Clip.Select);
    }

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		firstSelect = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		firstSelect = null;
	}

    void OnMouseDown()
    {
        if (render.sprite == null || BoardManager.instance.IsShifting)
        {
            return;
        }

        if (isSelected)
        {
            Deselect();
        }
        else
        {
            if (firstSelect == null)
            {
                Select();
            }
            else if (firstSelect)
            {
                CurrentSelect();

                if (SuitableTile(secondSelect.gameObject)) //Этот код должен не давать одному тайлу перемащатся через всё поле, 
                                                                                 //а только в указанном в масиве adjacentDirections направлении.
                                                                                 //Но почему-то, оно просто не видит firstSelect в методе GetAllAdjacentTiles()
                {
                    SwapSprite(firstSelect.render);
                    firstSelect.Deselect();
                    secondSelect.DeselectSecond();
                }
                else
                {
                    firstSelect.GetComponent<Tile>().Deselect();
                    Select();
                }
                //SwapSprite(firstSelect.render); //имея этот код, можно переместить тайл с одного угла в другой. Это неправильно.
                //firstSelect.Deselect();
            }
        }
    }

    public void SwapSprite(SpriteRenderer render2)
    {
        if (render.sprite == render2.sprite)
        {
            return;
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
        SFXManager.instance.PlaySFX(Clip.Swap);
    }

    // Єдиний метод яки й перевіряє чи підходить нам Tile
    private bool SuitableTile(GameObject tile)
    {
        // точка звідки кидаємо RayCast
        var originPos = (Vector2)firstSelect.transform.position;
        // дистанція на яку кидаємо RayCast
        var dist = firstSelect.GetComponent<BoxCollider2D>().bounds.size.x;

        foreach (var dir in adjacentDirections)
        {
            // отримуємо масив Raycast, це всі колайдери які потрапили в рейкаст
            var raycastHit = Physics2D.RaycastAll(originPos, dir, dist);
            // перевіряємо чи хоча б один із рейкастів відповідає імені шукаємого
            // raycastHit.FirstOrDefault(i => i.transform.name == tile.name) це LINQ конструкція
            // , якщо привести до циклу то буде виглядати так:
            //            for (int i = 0; i < raycastHit.Length; i++)
            //            {
            //                if (raycastHit[i].transform.name == tile.name)
            //                    return true;
            //            }

            if (raycastHit.FirstOrDefault(i => i.transform.name == tile.name))
                return true;
        }

        return false;
    }

    // Це по суті не робоче
    //    private GameObject GetAdjacent(Vector2 castDir)
    //    {
    //        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
    //        if (hit.collider.Equals(firstSelect))
    //        {
    //            print("Collider was hitet previous selected object");
    //        }
    //
    //        if (hit.collider != null)
    //        {
    //            return hit.collider.gameObject;
    //        }
    //        return null;
    //    }
    //
    //    private List<GameObject> GetAllAdjacentTiles()
    //    {
    //        List<GameObject> adjacentTiles = new List<GameObject>();
    //        for (int i = 0; i < adjacentDirections.Length; i++)
    //        {
    //            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
    //        }
    //        return adjacentTiles;
    //    }


}