using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frost : SkillGameObject 
{

	private EnemyController enemy;

	protected override void doAdditionalSetupOnStart()
	{
		GetComponent<DestroyObjectOnTimer>().time = currentSkill.Duration;
		enemy = GetComponentInParent<EnemyController>();
		enemy.applySlow(currentSkill.EffectAmount);
	}

	public override void onDestroy()
	{
		enemy.revertSlow(currentSkill.EffectAmount);
	}
}
