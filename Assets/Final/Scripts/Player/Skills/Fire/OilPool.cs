using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilPool : FlammableObject 
{
	ParticleSystem particleSystem;
	private float effectAmt;
	private float duration;

	protected override void doAdditionalSetupOnStart()
	{
		duration = currentSkill.Duration;
		GetComponent<DestroyObjectOnTimer>().time = duration;
		effectAmt = currentSkill.EffectAmount;
		particleSystem = GetComponentInChildren<ParticleSystem>();
		particleSystem.enableEmission = false;
		groundThePool();
	}

	
	// Update is called once per frame
	void Update () 
	{
	}

	private void groundThePool()
	{
		Debug.DrawRay(transform.position, -transform.up, Color.red, 4);
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -transform.up, out hit))
		{
			if (hit.transform.tag == "Ground")
			{
				transform.position = hit.point;
			}
		}
		transform.position = new Vector3(transform.position.x, transform.position.y + .25f, transform.position.z);
	}

	public void OnTriggerStay(Collider col)
	{
		if (IsOnFire)
		{

			if (col.transform.tag == "Enemy" || col.transform.tag == "Player")
			{
				object[] parameters = { currentSkill.EffectAmount, SkillType.Fire };
				col.SendMessage("recieveDamage", parameters, SendMessageOptions.RequireReceiver);
			}
		}
	}

	public override void ignite()
	{
		Debug.Log("Ignited");
		base.ignite();
		particleSystem.enableEmission = true;
		GetComponent<DestroyObjectOnTimer>().time = duration;
	}
}
