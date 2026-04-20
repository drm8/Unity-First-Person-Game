using UnityEngine;

public class Enemy : Hitable
{
    [SerializeField]
    private float maxHealth = 4.9f;
	private float health;

	[SerializeField]
	private float hitFlashDuration = 1.0f;
	private float hitFlashDelta = 0;

    private Material material;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        health = maxHealth;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(meshRenderer.material);
		material = meshRenderer.material;
	}

    // Update is called once per frame
    void Update()
    {
        if (hitFlashDelta > 0)
        {
			hitFlashDelta -= Time.deltaTime;
			if (hitFlashDelta < 0) hitFlashDelta = 0;
			float flashStrength = hitFlashDelta / hitFlashDuration;
			material.color = Color.Lerp(Color.red, Color.white, flashStrength * flashStrength);
		}
    }

	public override void Hit(float damage = 1, string type = "shot")
	{
        hitFlashDelta = hitFlashDuration;
        CreateDamageNumber(damage);

        health -= damage;
        if (health <= 0)
        {
			Object.Destroy(gameObject);
		}
	}

    public override float GetHealth() { return health / maxHealth; }
}
