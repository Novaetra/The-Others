using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormDash: MonoBehaviour 
{
	private StatsManager sm;
	private BoxCollider collider;
	[SerializeField]
	private float waitTime;

	[SerializeField]
	private GameObject stormDashParticle, particle;
	void Start()
	{
		collider = GetComponent<BoxCollider>();
		collider.enabled = false;
		sm = GameObject.Find("Player").GetComponent<StatsManager>();
	}

	public void dash()
	{
		collider.enabled = true;
		particle = GameObject.Instantiate(stormDashParticle);
		particle.transform.SetParent(transform.parent);
		particle.transform.localPosition = new Vector3(0, 2.4f, 2f);
		particle.transform.localRotation = Quaternion.Euler(0, 180, 0);
		StartCoroutine(disable());
	}

	IEnumerator disable()
	{
		yield return new WaitForSeconds(waitTime);
		collider.enabled = false;
		particle.GetComponent<ParticleSystem>().enableEmission = false;
		yield return new WaitForSeconds(1f);
		Destroy(particle);
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Enemy")
		{
			col.GetComponent<EnemyController>().recieveDamageWithType(sm.getMeleeDamage(),SkillType.Storm);
		}
	}
}
