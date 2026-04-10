using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Hitable : MonoBehaviour
{
	[SerializeField]
	private float head_offset = 1;

    public virtual void Hit(float damage = 1)
    {

    }

	public Vector3 HeadPosition()
	{
		return transform.position + Vector3.up * head_offset;
	}

	private void OnDestroy()
	{
		FindObjectsByType<EnemyManager>(FindObjectsSortMode.InstanceID)[0].RemoveEnemy(transform);
	}
}
