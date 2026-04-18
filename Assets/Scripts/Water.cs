using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField]
    private float cooldownDuration = 0.1f;
    private float cooldown = 0;

    [SerializeField]
    private float yVelMin = 6;
    [SerializeField]
    private float yVelMax = 10;
    [SerializeField]
    private float horizVel = 5;
    [SerializeField]
    private float speedBoost = 1;
    [SerializeField]
    private float damage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0) cooldown -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (cooldown <= 0 && collider.CompareTag("Player"))
        {
            cooldown = cooldownDuration;

            PlayerController playerController = collider.GetComponentInParent<PlayerController>();
            playerController.Hurt(damage);
            playerController.AddForce(Vector3.up * (Random.Range(yVelMin, yVelMax) - playerController.GetVelocity().y));
            playerController.MoveBySpeed(horizVel);
            playerController.AddSpeedBoost(speedBoost);
        }
    }
}
