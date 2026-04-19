using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Dummy : Hitable
{
	[SerializeField]
	private float hitFlashDuration = 1.0f;
	private float hitFlashDelta = 0;

    private Material material;

    [SerializeField]
    ParticleSystem hitParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
			material.color = Color.Lerp(Color.white, Color.red, flashStrength * flashStrength);
		}
    }

	public override void Hit(float damage = 1, string type = "shot")
	{
        hitFlashDelta = hitFlashDuration;
        Instantiate(hitParticles, transform);
        CreateDamageNumber(damage);
    }
}
