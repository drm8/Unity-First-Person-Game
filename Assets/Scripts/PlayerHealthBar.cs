using UnityEngine;

public class PlayerHealthBar : HealthBar
{
    private PlayerCombat player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetCropMask();

        player = FindObjectsByType<PlayerCombat>(FindObjectsSortMode.InstanceID)[0];
    }

    protected override float GetHealth() { return player.GetHealth(); }
}
