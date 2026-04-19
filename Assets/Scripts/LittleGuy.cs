using UnityEngine;

public class LittleGuy : MonoBehaviour
{
    private float bounceDelta = 0.0f;
    public float bounceSpeed = 4.0f;
    public float bounceHeight = 0.5f;
    public float rotationStrength = 30.0f;
    public float[] activeRange = {5.5f, 7.0f};
    private float activeDistance;
    private float distanceMultiplier;
	private float bounceFade = 0.0f;
    private float initialY;

    private Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialY = transform.position.y;
        playerTransform = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0].transform;
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 origPos = transform.position;

		// Rotate towards player
		Vector3 playerPosition = playerTransform.position;
		transform.LookAt(new Vector3(playerPosition.x, origPos.y, playerPosition.z));

		// Bounce around
		bounceDelta += bounceSpeed*Time.deltaTime;

        origPos.y = initialY;
        // These calculations are made in update in case activeRange is changed at runtime
		distanceMultiplier = 1 / (activeRange[1] - activeRange[0]);
		activeDistance = activeRange[1] * distanceMultiplier;
		bounceFade = Mathf.Clamp01(activeDistance - (playerPosition - origPos).magnitude * distanceMultiplier);

        float newY = initialY + bounceFade * bounceHeight * Mathf.Abs(Mathf.Sin(bounceDelta));
		transform.position = new Vector3(origPos.x, newY, origPos.z);

        float rotation = bounceFade * rotationStrength * Mathf.Sin(bounceDelta + Mathf.PI / 2);
		transform.Rotate(new Vector3(0, 0, rotation));
    }
}
