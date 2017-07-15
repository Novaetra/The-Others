using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteScript : MonoBehaviour 
{
	private float dmg;
	private SkillManager skillManager;
	private EnemyController enemyController;

	float currentTime, timerTime;


	void Start()
	{
		skillManager = GameObject.Find("Player").GetComponent<SkillManager>();
		GetComponent<DestroyObjectOnTimer>().time = skillManager.getSkillFromKnown("Ignite").Duration;
		skillManager = GameObject.Find("Player").GetComponent<SkillManager>();
		enemyController = GetComponentInParent<EnemyController>();
		dmg = skillManager.getSkillFromKnown("Ignite").EffectAmount;
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
