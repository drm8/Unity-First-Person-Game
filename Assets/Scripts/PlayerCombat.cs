using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 10f;
    private float health;

    private string respawnScene = "Menu";

    private InputAction attackAction;

    [SerializeField]
    private float shootDistance = 500;

    [SerializeField]
    private float aimAssistBase = 0.9f;
    [SerializeField]
    private float aimAssistPerMeter = 0.05f;

    [SerializeField]
    private float recoilDistanceThreshold = 5;
    [SerializeField]
    private float recoilMaxVelocity = 10;
    [SerializeField]
    private float recoilMaxEnemyVelocity = 5;
    [SerializeField]
    private float recoilSpeedBoost = 0.5f;

    [SerializeField]
    private float shotDamageBase = 1;
    [SerializeField]
    private float shotDamageVelocityMultiplier = 0.025f;

    [SerializeField]
    private int maxAmmo = 3;
    private int ammo;

    [SerializeField]
    ParticleSystem enemyHitParticles;
    [SerializeField]
    ParticleSystem floorHitParticles;

    EnemyManager enemies;

    private PlayerController controller;
    private Camera cam;

    private Crosshair crosshairUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        ammo = maxAmmo;

        attackAction = InputSystem.actions.FindAction("Attack");

        enemies = FindObjectsByType<EnemyManager>(FindObjectsSortMode.InstanceID)[0];

        controller = GetComponent<PlayerController>();
        cam = GetComponentInChildren<Camera>();
        crosshairUI = FindObjectsByType<Crosshair>(FindObjectsSortMode.InstanceID)[0];
        crosshairUI.SetActive(true);
    }

    public void OnPause() { attackAction.Disable(); }
    public void OnUnpause() { attackAction.Enable(); }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }

    private void Shoot()
    {
        if (ammo > 0 && attackAction.WasPressedThisFrame()) // Has the shoot button been pressed?
        {
            SetAmmo(ammo - 1);

            RaycastHit hit;
            bool hasHit;

            // Aim assisted hit check
            Vector3 direction = cam.transform.forward;
            foreach (Transform enemy in enemies.GetEnemyList())
            {
                Vector3 positionDifference = enemy.position - cam.transform.position;
                float distance = positionDifference.magnitude;

                // Is the enemy close enough?
                if (distance > shootDistance) continue;

                // Is the angle close enough?
                Vector3 flattenedDirection = new Vector3(direction.x, direction.y / 2, direction.z);
                Vector3 flattenedPositionDifference = new Vector3(positionDifference.x, positionDifference.y / 2, positionDifference.z);
                float angleDisparity = Vector3.Distance(flattenedPositionDifference, Vector3.Project(flattenedPositionDifference, flattenedDirection));
                if (angleDisparity > aimAssistBase + distance * aimAssistPerMeter) continue;
                if (Vector3.Angle(positionDifference, direction) > 90) continue;

                // Is there line of sight?
                hasHit = Physics.Raycast(cam.transform.position, positionDifference, out hit, distance);
                if (hasHit && hit.collider.transform == enemy)
                {
                    // Hurt enemy
                    float speedMultiplier = 1 + Mathf.Pow(controller.GetSpeed(), 2) * shotDamageVelocityMultiplier;
                    enemy.GetComponentInParent<Hitable>().Hit(shotDamageBase * speedMultiplier, "shot");

                    // Spawn particles
                    Quaternion particleRotation = Quaternion.FromToRotation(Vector3.up, Vector3.Reflect(cam.transform.forward, hit.normal));
                    Instantiate(enemyHitParticles, hit.point, particleRotation);

                    // Apply recoil
                    RaycastRecoil(hit, recoilMaxEnemyVelocity);

                    return;
                }
            }

            // Wall hit check
            hasHit = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, shootDistance);
            if (hasHit)
            {
                RaycastRecoil(hit, recoilMaxVelocity);
                Quaternion particleRotation = Quaternion.FromToRotation(Vector3.up, Vector3.Reflect(cam.transform.forward, hit.normal));
                Instantiate(floorHitParticles, hit.point, particleRotation);
            }
        }
    }

    private void RaycastRecoil(RaycastHit from, float maxVelocity)
    {
        Vector3 difference = transform.position - from.point;
        float mag = difference.magnitude;
        if (mag < recoilDistanceThreshold && mag > 0)
        {
            // Velocity
            if (from.normal.Equals(Vector3.up)) controller.velocity.y = 0;
            float strength = (recoilDistanceThreshold - mag) / recoilDistanceThreshold;
            controller.velocity += difference.normalized * (strength * maxVelocity);

            // Speed boost
            float speedBoostAngleMultiplier = 1 - Mathf.Abs(Mathf.Cos(Vector3.Angle(Vector3.up, difference)));
            controller.AddSpeedBoost(recoilSpeedBoost * Mathf.Sqrt(strength) * speedBoostAngleMultiplier);
        }
    }

    public int GetAmmo() { return ammo; }
    public void SetAmmo(int amount)
    {
        ammo = amount;
        crosshairUI.SetActive(ammo > 0);
    }
    public void ReplenishAmmo() { SetAmmo(maxAmmo); }

    public float GetHealth() { return health / maxHealth; }
    public void Hurt(float damage) 
    { 
        health -= damage; 
        if (health <= 0) SceneManager.LoadScene(respawnScene);
    }
}
