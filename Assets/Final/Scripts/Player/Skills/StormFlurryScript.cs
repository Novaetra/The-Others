using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormFlurryScript : MonoBehaviour 
{
	private Animator animator;
	private StatsManager sm;
	private int animSpeed;

	private float duration, startDamge;

	void Start()
	{
		animator = GetComponent<Animator>();
		animSpeed = 2;
	}

	public void SetDuration(float dur)
	{
		duration = dur;
		StartCoroutine(Timer());
	}

	public void SetDamageBuff(float dmg)
	{
		sm = GetComponent<StatsManager>();
		startDamge = sm.getMeleeDamage();
		sm.setMeleeDamage(startDamge*dmg);
		Debug.Log("set damage to: " + ((sm.getMeleeDamage() * dmg)+" and before it was " + startDamge ));
	}

	private void resetDamage()
	{
		sm.setMeleeDamage(startDamge);
		Debug.Log("Set damage back to " + startDamge);
	}

	private IEnumerator Timer()
	{
		yield return new WaitForSeconds(duration);
		resetDamage();
		Destroy(this);
	}


	void Update()
	{
		AnimatorStateInfo name = animator.GetCurrentAnimatorStateInfo(1);
		if (name.IsTag("Melee"))
		{
			animator.speed = animSpeed;
		}
		else
		{
			animator.speed = 1;
		}
	}

	public int AnimSpeed
	{
		get
		{
			return animSpeed;
		}

		set
		{
			animSpeed = value;
		}
	}
}
