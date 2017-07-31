using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGameObject : MonoBehaviour 
{
	protected SkillManager skillManager;
	protected Skill currentSkill;
	[SerializeField]
	protected string nameOfSkill;
	void Start()
	{
		skillManager = GameObject.Find("Player").GetComponent<SkillManager>();
		currentSkill = skillManager.getSkillFromKnown(nameOfSkill);
		doAdditionalSetupOnStart();
	}

	protected virtual void doAdditionalSetupOnStart()
	{

	}

	public virtual void onDestroy()
	{

	}
}
