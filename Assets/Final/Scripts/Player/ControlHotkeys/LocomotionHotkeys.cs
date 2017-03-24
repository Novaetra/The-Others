﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionHotkeys : ControlHotkey 
{
	private EnemyManager em;

	public LocomotionHotkeys(KeyCode[] c,PlayerController cntrl)
	{
		codes = c; 
		controller = cntrl;
		em = GameObject.Find ("Managers").GetComponent<EnemyManager> ();
	}

	//codes[0] = sprint
	//codes[1] = interact
	//codes[2] = leftClick
	public override void CheckKeys()
	{
		if (controller.CanMove) 
		{
			Move ();
		}

		foreach (KeyCode c in Codes) 
		{
			if (c == Codes [0]) {
				if (Input.GetKey (c)) {
					Sprint ();
				}

				if (Input.GetKeyUp (c)) {
					StopSprint ();
				}
			} else if (c == Codes [1]) {
				if (Input.GetKey (c)) {
					Interact ();
				}
			} else if (c == Codes [2]) {
				if (Input.GetKeyUp (c) && controller.cursorLocked) {
					Melee ();
				}
			} else if (c == Codes [3]) {
				if (Input.GetKeyUp (c) && controller.Sm.getCurrentExp() - controller.Sm.getGoalExp() >=0) 
				{
					controller.Sm.lvlUp(controller.Sm.getCurrentExp() - controller.Sm.getGoalExp());
				}
			} else if (c==Codes[4])
				{
				if (Input.GetKeyUp (c) && em.RoundHasStarted == false) 
					{
						em.startNextRound ();
					}
				}
		}
	}

	private void Move()
	{
		float speed = Input.GetAxis("Vertical") * controller.currentSpeed;
		float direction = Input.GetAxis("Horizontal") * controller.currentSpeed;
		controller.Anim.SetFloat("Speed", speed);
		controller.Anim.SetFloat("Direction", direction);
		Vector3 finalMove = new Vector3 (direction, 0f, speed);
		finalMove = controller.transform.rotation * finalMove;
		controller.Cs.Move(finalMove * Time.deltaTime);
	}

	private void Interact()
	{
		RaycastHit hit;
		foreach (Raycaster caster in controller.Raycasters)
		{
			Transform raycaster = caster.transform;
			Debug.DrawRay(raycaster.transform.position, -raycaster.transform.forward * controller.InteractDistance, Color.blue);
			if (Physics.Raycast(raycaster.transform.position, -raycaster.transform.forward, out hit, controller.InteractDistance))
			{
				//Has to check these two separately because each one has their own specific protocols
				// checkRevive(hit);
				controller.checkInteract(hit);
			}
		}
	}

	private void Sprint()
	{
		//Sprint
		if (controller.Sm.getCurrentStamina() > 0) 
		{
			controller.currentSpeed = controller.runSpeed;
			controller.Sm.useStamina ((controller.Sm.getSprintStamCost() /* - (sm.sprintStaminaCost  (sm.dexterity / 100))*/),true);
		} 
		else 
		{
			StopSprint ();
		}
	}

	private void StopSprint()
	{
		controller.currentSpeed = controller.walkSpeed;
	}

	private void Melee()
	{
		if (controller.Sm.getCurrentStamina() - controller.Sm.getMeleeCost() >= 0 && controller.Anim.GetInteger("Skill") != (int)Skills.BasicAttack)  
		{
			controller.Sm.useStamina (controller.Sm.getMeleeCost(),false);
			controller.Anim.SetInteger("Skill",(int)Skills.BasicAttack);
		}
	}
}
