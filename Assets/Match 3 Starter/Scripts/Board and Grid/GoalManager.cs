using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberColected;
    public Sprite goalSprite;
}

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance;
    [SerializeField]
    private BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    private int[] numberOfGoals;
    [SerializeField]
    private GameObject goalPrefab;
    [SerializeField]
    private GameObject goalIntroParent;
    [SerializeField]
    private GameObject goalGameParent;

    // Start is called before the first frame update
    private void Start()
    {
        GetGoals();
        LevelGoalsReset();
        SetUpGoals();
        Instance = this;
    }

    private void LevelGoalsReset()
    {
        foreach (var numberColected in levelGoals)
        {
            numberColected.numberColected = 0;
        }
    }

    private void GetGoals()
    {
        if (BoardManager.instance != null)
        {
            if (BoardManager.instance.world != null)
            {
                if (BoardManager.instance.world.levels[BoardManager.instance.level] != null)
                {
                    levelGoals = BoardManager.instance.world.levels[BoardManager.instance.level].levelGoals;
                }
            }
        }
    }

    private void SetUpGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.Euler(0, 0, 180));
            goal.transform.SetParent(goalIntroParent.transform, false);

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.Euler(0, 0, 180));
            gameGoal.transform.SetParent(goalGameParent.transform, false);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberColected + "/" + levelGoals[i].numberNeeded;
            if (levelGoals[i].numberColected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            GUIManager.instance.WinGameWindow();
        }
    }

    public void CompareGoal(Sprite spriteToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (spriteToCompare == levelGoals[i].goalSprite)
            {
                levelGoals[i].numberColected++;
            }
        }
    }
}
