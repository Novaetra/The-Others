using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour 
{

	private SkillManager skillManager;
	private float totalTime=5f, currentTime=0f;

	private float dmg;

	private SphereCollider explosionCollider;

	private float explosionDuration = .1f, currentExplosionDuration = 0;

	private bool hasDetonated = false, hitPlayer = false;

	void Awake()
	{
		explosionCollider = GetComponent<SphereCollider>();
		explosionCollider.enabled = false;
		skillManager = GameObject.Find("Player").GetComponent<SkillManager>();
	}

	void Start()
	{
		Skill _skill = skillManager.getSkillFromKnown("Fire Bomb");
		dmg = _skill.EffectAmount;
		totalTime = _skill.Duration;
		currentTime = 0f;
	}

	void Update () 
	{
		if (currentTime < totalTime)
		{
			currentTime += Time.deltaTime;
		}
		else if (hasDetonated == false)
		{
			detonate();
		}
		else
		{
			explosionCollider.enabled = false;
		}
	}

	private void detonate()
	{
		hasDetonated = true;
		explosionCollider.enabled = true;
		GameObject explosion = (GameObject)GameObject.Instantiate(Resources.Load("Spells/Explosion"), transform.position, Quaternion.identity);
		StartCoroutine(destroyBomb());
	}

	private IEnumerator destroyBomb()
	{
		yield return new WaitForSeconds(.1f);
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.transform.tag == "Player" && !hitPlayer)
		{
			object[] parameters = { dmg, SkillType.Fire };
			col.transform.SendMessage("recieveDamage", parameters, SendMessageOptions.DontRequireReceiver);
			hitPlayer = true;
		}
		else if (col.transform.tag == "Enemy")
		{
			object[] parameters = { dmg, SkillType.Fire };
			col.transform.SendMessage("recieveDamage", parameters, SendMessageOptions.DontRequireReceiver);
		}
	}

}
