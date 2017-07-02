using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slam : MonoBehaviour 
{
	private float meleeDamage = 0;
	private bool alreadyAppliedDmg = false;

	void Start()
	{
		meleeDamage = transform.parent.GetComponent<FirstBossBehavior>().MeleeDamage;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.transform.tag == "Player" && alreadyAppliedDmg==false)
		{
			alreadyAppliedDmg = true;
			col.transform.GetComponent<StatsManager>().recieveDamage(meleeDamage);
			Destroy(gameObject);
		}
	}
}
