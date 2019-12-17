using System.Collections;
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
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    [SerializeField]
    GameObject goalPrefab;
    [SerializeField]
    GameObject goalIntroParent;
    [SerializeField]
    GameObject goalGameParent;

    // Start is called before the first frame update
    void Start()
    {
        SetUpGoals();
        Instance = this;
    }

    void SetUpGoals()
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
            print("You win!");
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
