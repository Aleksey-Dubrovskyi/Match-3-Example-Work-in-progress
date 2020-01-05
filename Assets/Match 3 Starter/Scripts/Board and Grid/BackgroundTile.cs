using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (hitPoints <= 0)
        {
            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.CompareGoal(sprite.sprite);
                GoalManager.Instance.UpdateGoals();
            }
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        ChangeOpacity();
    }

    private void ChangeOpacity()
    {
        Color color = sprite.color;
        float newAlpa = color.a * 0.5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpa);
    }
}
