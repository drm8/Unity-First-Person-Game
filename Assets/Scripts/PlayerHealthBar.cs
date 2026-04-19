using UnityEngine;

public class PlayerHealthBar : HealthBar
{
    private PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetCropMask();

        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0];
    }

    protected override float GetHealth() { return player.GetHealth(); }
}
