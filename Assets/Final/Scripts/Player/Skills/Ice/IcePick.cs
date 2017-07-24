using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePick : SkillGameObject
{
	private int enemiesHit = 0;
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Enemy")
		{
			enemiesHit++;
			col.transform.GetComponent<EnemyController>().recieveDamageWithType(currentSkill.EffectAmount, SkillType.Ice);
			currentSkill.MaxEnemiesHit++;
			if (enemiesHit >= currentSkill.MaxEnemiesHit)
			{
				Destroy(transform.parent.gameObject);
			}
		}
	}

	public void destroyIcePick()
	{
		Destroy(transform.parent.gameObject);
	}
}
