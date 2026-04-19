using UnityEngine;

public class EnemyHealthBar : HealthBar
{
    private Hitable enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetCropMask();

        enemy = GetComponentInParent<Hitable>();
    }

    protected override float GetHealth() { return enemy.GetHealth(); }
}
