using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteScript : SkillGameObject 
{
	private float dmg;
	private EnemyController enemyController;

	float currentTime, timerTime;

	protected override void doAdditionalSetupOnStart()
	{
		enemyController = GetComponentInParent<EnemyController>();
		GetComponent<DestroyObjectOnTimer>().time = currentSkill.Duration;
		dmg = currentSkill.EffectAmount;
		timerTime = 1f;
		currentTime = timerTime;
	}

	void Update()
	{
		if (currentTime < timerTime)
		{
			if (enemyController.IsAlive)
			{
				currentTime += Time.deltaTime;
			}
			else
			{
				Destroy(gameObject);
			}
		}
		else
		{
			if (enemyController.IsAlive)
			{
				enemyController.recieveDamage(dmg);
			}
			else
			{
				Destroy(gameObject);
			}
			currentTime = 0;
		}
	}
}
