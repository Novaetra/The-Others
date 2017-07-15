using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilPool : FlammableObject 
{
	ParticleSystem particleSystem;
	SkillManager skillManager;
	float dmg;
	// Use this for initialization
	void Start () 
	{
		particleSystem = GetComponentInChildren<ParticleSystem>();
		particleSystem.enableEmission = false;
		groundThePool();
		skillManager = GameObject.Find("Player").GetComponent<SkillManager>();
		GetComponent<DestroyObjectOnTimer>().time = skillManager.getSkillFromKnown("Oil Pool").Duration;
		dmg = skillManager.getSkillFromKnown("Oil Pool").EffectAmount;
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
				col.SendMessage("recieveDamage", dmg, SendMessageOptions.RequireReceiver);
			}
		}
	}

	public override void ignite()
	{
		base.ignite();
		particleSystem.enableEmission = true;
	}
}
