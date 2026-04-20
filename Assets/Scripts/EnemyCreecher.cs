using UnityEngine;

public class EnemyCreecher : Enemy
{
	public override void Hit(float damage = 1, string type = "shot")
	{
        if (type == ("shot"))
        {
            base.Hit(0);
        }
        else
        {
            base.Hit(damage*2);
        }
	}
}
