using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    public virtual void Hit(float damage = 1)
    {

    }

	private void OnDestroy()
	{
		FindObjectsByType<EnemyManager>(FindObjectsSortMode.InstanceID)[0].RemoveEnemy(transform);
	}
}
