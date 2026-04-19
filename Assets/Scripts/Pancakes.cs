using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Pancakes : Hitable
{
    private PlayerController player;
    protected new float headOffset = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0];
        jumpable = false;
    }

    // Update is called once per frame
    void Update()
    {
        player.SetAmmo(3);
    }

    public override void Hit(float damage = 1, string type = "shot")
	{
        CreateDamageNumber(damage);
        if (type == "shot") Object.Destroy(gameObject);
    }
}
