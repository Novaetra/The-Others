using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using System;

public class FirstBossBehavior : EnemyController
{
	private GameObject gate;

	[SerializeField]
	private GameObject slam;

	private List<EnemySkill> enemySkills = new List<EnemySkill>();

	private float meleeRange = 1.95f, startSpeed;
	private bool queuedAction = false, completedAction = true;
	private Action selectedActionToDoNext;

	protected override void DoAdditionalSetup()
	{
		SetUpActionsList ();
		gate = GameObject.Find("SummonGate");
		startSpeed = agent.speed;
	}

	private void SetUpActionsList()
	{
		
		enemySkills.Add (new EnemySkill(JumpAttack, 50));
		enemySkills.Add(new EnemySkill(GroundSlam, 50));
	}

	protected override void DoWithinProximity(float playerDistance)
	{
		//This method is called every frame so in order to just pick one action and stick with it
		//We first check if we have an action queued
		if (!queuedAction)
		{
			//If its not queued, queue is true
			queuedAction = true;
			completedAction = false;
			//Store action to do
			selectedActionToDoNext = GetRandomAction();
			//Since the first action is a jump attack, we want to do it from a distance, so just call the function now
			if (selectedActionToDoNext == enemySkills[0].Action)
			{
				selectedActionToDoNext();
				animatorSpeedToZero();
			}
		}
		//if an action is already selected...
		else
		{
			//If the action is the jump attack, it means we are in air
			//So stop the agent at melee range so we dont overshoot player
			checkIfJumpAttack();

			//If the action is the ground slam {
			checkIfSlamAttack();
		}
	}


	private void checkIfJumpAttack()
	{
		if (selectedActionToDoNext == enemySkills[0].Action && completedAction == false)
		{
			//Keep rotating towards player until you're within melee range
			if (playerDistance > meleeRange)
			{
				rotateTowards(targetPlayer);
			}
		}
	}

	private void checkIfSlamAttack()
	{
		if (selectedActionToDoNext == enemySkills[1].Action && completedAction == false)
		{
			//Then wait until we are within melee range
			if (playerDistance <= meleeRange)
			{
				//Then ground slam
				selectedActionToDoNext();
				animatorSpeedToZero();
			}
			else if (agent.enabled == true)
			{
				runAfterPlayer();
			}
		}
	}

	private void RunInAnimator()
	{
		agent.speed = startSpeed * 1.1f;
		startSpeed = agent.speed;
		anim.SetFloat("Speed", agent.speed);
	}

	protected override void AfterTakingDamage()
	{
		if (currentHealth < totalHealth / 2)
		{
			RunInAnimator();
		}
	}

	protected override void DoNotWithinProximity(float playerDistance)
	{
		if (completedAction == true)
		{
			resetAnimator();
			runAfterPlayer();
		}
		else if (completedAction == false && selectedActionToDoNext == enemySkills[0].Action)
		{
			agent.speed = playerDistance;
		}
	}

	private void runAfterPlayer()
	{
		rotateTowards(targetPlayer);
		agent.enabled = true;
		agent.SetDestination(targetPlayer.position);
		walkAnim();
	}

	public void doGroundSlam()
	{
		if (IsAlive)
		{
			GameObject _slam = GameObject.Instantiate(slam);
			_slam.transform.SetParent(transform);
			_slam.transform.localPosition = new Vector3(0, 0, 1);
		}
	}

	private Action GetRandomAction()
	{
		int randNumber = (int)UnityEngine.Random.Range(0, 101);
		int lastCheck = 0;
		foreach (EnemySkill skill in enemySkills)
		{
			if (randNumber > lastCheck && randNumber <= lastCheck + skill.Probability)
			{
				return skill.Action;
			}
			lastCheck += skill.Probability;
		}
		return null;
	}

	private void JumpAttack()
	{
		anim.SetInteger("Skill", 1);

	}

	private void GroundSlam()
	{
		agent.enabled = false;
		anim.SetInteger("Skill", 2);
	}

	protected override void resetAnimator()
	{
		if (isAlive)
		{
			GetComponent<Animator>().SetInteger("Skill", 0);
			enableAgent();
			completedAction = true;
			agent.speed = startSpeed;
			queuedAction = false;
		}
	}

	protected override void DoAdditionalOnDeath()
	{
		gate.GetComponent<SummonGateUnlockableComponent>().BossWasKilled();
		spawnPowerUp(0);
	}
}