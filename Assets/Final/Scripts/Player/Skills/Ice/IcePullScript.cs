using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePullScript : SkillGameObject 
{

	void OnTriggerStay(Collider col)
	{
		//Destination is me
		//Origin is the col
		if (col.transform.tag.Equals("Enemy"))
		{
			Vector3 direction = (transform.position - col.transform.position).normalized;
			col.transform.position += direction * (Time.deltaTime * 2.7f);
		}
	}

	protected override void doAdditionalSetupOnStart()
	{
		GetComponent<DestroyObjectOnTimer>().time = currentSkill.Duration;
	}
}
