using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("Board Demensions")]
    public int xSize;
    public int ySize;

    [Header("Tiles Layout")]
    public TyleType[] boardLayout;

    [Header("Avilable Tiles")]
    public List<Sprite> characterTiles = new List<Sprite>();

    [Header("Score goals")]
    public int[] scoreGoals;

    [Header("End game requirements")]
    public Requiarament requiaraments;
    public BlankGoal[] levelGoals;
}
