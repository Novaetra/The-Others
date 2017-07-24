using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlowingMistScript : SkillGameObject 
{
	private float effectAmt;
	private float duration;
	protected override void doAdditionalSetupOnStart()
	{
		duration = currentSkill.Duration;
		GetComponent<DestroyObjectOnTimer>().time = duration;
		effectAmt = currentSkill.EffectAmount;
	}
	void OnTriggerEnter(Collider col)
	{
		if (col.transform.tag.Equals("Enemy"))
			{
			col.transform.SendMessage("applySlow", effectAmt, SendMessageOptions.RequireReceiver);
			}
		else if (col.transform.tag.Equals("Player"))
			{
			col.transform.SendMessage("applySlow", effectAmt, SendMessageOptions.RequireReceiver);
			}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.transform.tag.Equals("Enemy"))
		{
			col.transform.SendMessage("revertSlow", effectAmt, SendMessageOptions.RequireReceiver);
		}
		else if (col.transform.tag.Equals("Player"))
		{
			col.transform.SendMessage("revertSlow", effectAmt, SendMessageOptions.RequireReceiver);
		}
	}
}
