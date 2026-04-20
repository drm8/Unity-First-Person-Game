using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public void GoRandomPathAround(NavMeshAgent agent, Vector3 pos, float minDistance, float maxDistance)
    {
        for (int i = 0; i < 10; i++)
        {
            float random = Random.Range(0, 2 * Mathf.PI);
            Vector2 offset = new Vector2(Mathf.Cos(random), Mathf.Sin(random)) * Random.Range(minDistance, maxDistance);
            Vector3 endPos = new Vector3 (pos.x + offset.x, transform.position.y, pos.z + offset.y);
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(endPos, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetPath(path);
                return;
            }
        }

        // No valid path was found
        agent.SetDestination(pos);
    }
}
