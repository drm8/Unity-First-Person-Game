using Unity.VisualScripting;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField]
    private float launchStrength = 10;

    [SerializeField]
    private float playerVelMultiplier = 2;

	[SerializeField]
	private float maxExtraStrength = 10;

    [SerializeField]
    private float cooldownDuration = 0.1f;
    private float cooldown = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0) cooldown -= Time.deltaTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (cooldown <= 0 && collision.collider.CompareTag("Player"))
		{
            PlayerController playerController = collision.collider.GetComponentInParent<PlayerController>();
            float currentYMagnitude = Mathf.Abs(playerController.GetVelocity().y);
            float fallSpeed = Mathf.Abs(playerController.GetFallSpeed());
			float force = launchStrength + currentYMagnitude + Mathf.Min(fallSpeed * (playerVelMultiplier - 1), maxExtraStrength);
			playerController.AddForce(Vector3.up * force);
            cooldown = cooldownDuration;

		}
	}
}
