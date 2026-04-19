using UnityEngine;

public class Hitable : MonoBehaviour
{
	[SerializeField]
	protected float headOffset = 1;

	protected bool jumpable = true;

	[SerializeField] GameObject damageNumber;

	protected void CreateDamageNumber(float damage)
	{
        GameObject newNumber = Instantiate(damageNumber, transform.position + Vector3.up * headOffset, Quaternion.identity);
		newNumber.GetComponent<DamageNumber>().SetNumber(damage);
	}

    public virtual void Hit(float damage = 1, string type = "shot") { }

	public Vector3 HeadPosition()
	{
		return transform.position + Vector3.up * headOffset;
	}

	public virtual float GetHealth() { return 1; }

	private void OnDestroy()
	{
		FindObjectsByType<EnemyManager>(FindObjectsSortMode.InstanceID)[0].RemoveEnemy(transform);
	}

	public bool GetJumpable() { return jumpable; }
}
