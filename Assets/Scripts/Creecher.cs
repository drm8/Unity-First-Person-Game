using UnityEngine;
using UnityEngine.AI;

public class Creecher : EnemyAI
{
    [SerializeField] private NavMeshAgent agent;

    private PlayerController player;

    [SerializeField] private float wanderMinDelay = 2;
    [SerializeField] private float wanderMaxDelay = 5;
    private float wanderTimer;

    [SerializeField] private float retargetDistance = 4;

    [SerializeField] private float fireDistance = 6;
    [SerializeField] private float fireVelocity = 10;
    [SerializeField] private float fireRate = 1;
    private float fireCooldown;
    [SerializeField] GameObject projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0];
        wanderTimer = Random.Range(wanderMinDelay, wanderMaxDelay);

        fireCooldown = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        NavMeshHit hit;
        if (!agent.Raycast(player.transform.position, out hit))
        {
            if (Vector3.Distance(agent.destination, player.transform.position) >= retargetDistance)
            {
                GoRandomPathAround(agent, player.transform.position, 0.5f, 3);
            }
        }
        else
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                wanderTimer = Random.Range(wanderMinDelay, wanderMaxDelay);
                GoRandomPathAround(agent, transform.position, 3, 6.5f);
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= fireDistance)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0)
            {
                fireCooldown += fireRate;
                float random = Random.Range(0, 2 * Mathf.PI);
                for (float i = 0; i < 8; i++)
                {
                    Vector3 dir = new Vector3(Mathf.Cos(random + ((i / 4) * Mathf.PI)), 0, Mathf.Sin(random + ((i / 4) * Mathf.PI)));
                    GameObject proj = Instantiate(projectile, transform.position + dir * 0.5f, Quaternion.identity);
                    proj.GetComponent<EnemyProjectile>().SetVelocity(dir * fireVelocity);
                }
            }
        }
    }
}
