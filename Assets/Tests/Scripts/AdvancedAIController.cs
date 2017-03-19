using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using System;

public class AdvancedAIController : EnemyController
{
	private List<Action> actions = new List<Action>();
	private float waitTime = 5f;
	private bool doingAction = false;

	protected override void DoAdditionalSetup()
	{
		SetUpActionsList ();
	}

	private void SetUpActionsList()
	{
		actions.Add (attackAnim);
		actions.Add (talk);
	}

	private void talk()
	{
		Debug.Log ("Hey how are ya?");
	}

	protected override void DoWithinProximity(float playerDistance)
	{
		if (playerDistance < proximityRange/2) 
		{
			Debug.Log ("Too close");
			//play 'Back up' animation
		} 
		else 
		{
			idleAnim ();
			agent.enabled = false;
			if (!doingAction) 
			{
				doingAction = true;
				StartCoroutine(GenerateRandomAction());
			}
		}
	}

	public override void checkAttack()
	{
		if (isAlive)
		{
			RaycastHit hit;
			foreach (Raycaster caster in casters)
			{
				Transform raycaster = caster.transform;
				Debug.DrawRay(raycaster.position, raycaster.forward * attackRange, Color.blue, 1);
				if (Physics.Raycast(raycaster.position, raycaster.forward, out hit, attackRange))
				{
					if (hit.transform.tag == "Player")
					{
						//hit.transform.GetComponent<StatsManager>().recieveDamage(meleeDamage);
						Debug.Log("Hit player");
						return;
					}
				}
			}
		}
	}

	IEnumerator GenerateRandomAction ()
	{
		int randNumber = (int)UnityEngine.Random.Range(0,actions.Count);
		Action action = actions [randNumber];
		action ();
		yield return new WaitForSeconds (waitTime);
		doingAction = false;
	}
}