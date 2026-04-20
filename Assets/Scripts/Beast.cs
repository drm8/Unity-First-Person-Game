using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;

public class Beast : EnemyAI
{
    [SerializeField] private NavMeshAgent agent;

    private PlayerController player;

    [SerializeField] private float wanderMinDelay = 1.2f;
    [SerializeField] private float wanderMaxDelay = 3;
    private float wanderTimer;

    [SerializeField] private float fleeDistance = 5.5f;
    [SerializeField] private float fleeCooldownDuration = 0.4f;
    [SerializeField] private float fleeCooldown = 0;

    [SerializeField] private float fireDistance = 4;
    [SerializeField] private float fireVelocity = 7;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private float burstRate = 0.1f;
    [SerializeField] private int maxBursts = 3;
    private int burstsLeft = 0;
    private float fireCooldown;
    [SerializeField] GameObject projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0];
        wanderTimer = Random.Range(wanderMinDelay, wanderMaxDelay);

        burstsLeft = maxBursts;
        fireCooldown = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (fleeCooldown > 0) fleeCooldown -= Time.deltaTime;

        if (fleeCooldown <= 0 && Vector3.Distance(agent.destination, player.transform.position) <= fleeDistance)
        {
            fleeCooldown = fleeCooldownDuration;
            GoRandomPathAround(agent, player.transform.position, 6, 9);
        }
        else
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                wanderTimer = Random.Range(wanderMinDelay, wanderMaxDelay);
                GoRandomPathAround(agent, transform.position, 3, 7);
            }
        }

        Vector3 positionDifference = (player.transform.position - transform.position);
        float distance = positionDifference.magnitude;

        if (Vector3.Distance(transform.position, player.transform.position) >= fireDistance)
        {
            Vector3 dir = positionDifference.normalized;

            if (!Physics.Raycast(transform.position + dir, dir, out _, distance - 1))
            {
                fireCooldown -= Time.deltaTime;
                if (fireCooldown <= 0)
                {
                    if (burstsLeft <= 0)
                    {
                        burstsLeft = maxBursts;
                        fireCooldown += fireRate;
                    }
                    else
                    {
                        burstsLeft -= 1;
                        fireCooldown += burstRate;
                        GameObject proj = Instantiate(projectile, transform.position + dir * 0.5f, Quaternion.identity);
                        proj.GetComponent<EnemyProjectile>().SetVelocity(dir * fireVelocity);
                    }
                }
            }
        }
    }
}
