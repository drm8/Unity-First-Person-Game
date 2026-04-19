using UnityEngine;

public class Hitable : MonoBehaviour
{
	[SerializeField]
	private float head_offset = 1;

	protected bool jumpable = true;

	public virtual void Hit(float damage = 1) { }

	public Vector3 HeadPosition()
	{
		return transform.position + Vector3.up * head_offset;
	}

	public virtual float GetHealth() { return 1; }

	private void OnDestroy()
	{
		FindObjectsByType<EnemyManager>(FindObjectsSortMode.InstanceID)[0].RemoveEnemy(transform);
	}

	public bool GetJumpable() { return jumpable; }
}
