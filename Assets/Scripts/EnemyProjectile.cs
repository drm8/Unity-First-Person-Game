using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    [SerializeField] private float timeLeft = 10;
    [SerializeField] private float damage = 2;
    [SerializeField] private float friction = 0;

    public void SetVelocity(Vector3 vel) { velocity = vel; }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        velocity /= 1 + friction * Time.deltaTime;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0) GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hitable"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerCombat>().Hurt(damage);
            }
            GameObject.Destroy(gameObject);
        }
    }
}
