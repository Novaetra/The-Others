﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTreePiece : MonoBehaviour 
{
	//Attributes
	[SerializeField]
	private string skillName;
	private Skill skill;
	private List<Skill> skillList;
	private Image img;
    private StatsManager sm;
	private SkillManager skillManager;
	private HUDManager hudman;
    private bool isUnlocked;
	[SerializeField]
	private int numberInTree;

	private string originalDescription;


	public void Start()
    {
        StartCoroutine(setSkillTimer());
    }

    private IEnumerator setSkillTimer()
    {
        yield return new WaitForSeconds(0.3f);
        setSkill();
    }

    //Links the skill name to the actual skill in the list of all skills
	public void setSkill()
	{
        isUnlocked = false;
		skillList = GameObject.FindObjectOfType<Player>().gameObject.GetComponent<SkillManager> ().allSkills;
        sm =  GameObject.Find("Player").GetComponent<StatsManager>();
        hudman = GameObject.Find("Player").GetComponent<HUDManager>();
		skillManager = sm.GetComponent<SkillManager>();
		img = gameObject.GetComponent<Image> ();
		foreach (Skill s in skillList) 
		{
			if (s.Name.Equals (skillName)) 
			{
				skill = s;
				originalDescription = s.Description;
				return;
			}
		}
	}

	//Unlocks/Upgrades skills
	public void unlockOrUpgradeSkill()
    {
		Debug.Log("Unlock or upgrading " + sm.getUpgradePnts());
		if (!isUnlocked && sm.getUpgradePnts()>0) 
		{
			unlockSkill();
		}

		else if(isUnlocked && sm.getUpgradePnts() > 0)
		{
			upgradeSkill();
		}
    }

	private void unlockSkill()
	{
		isUnlocked = true;
		skill.IsUnlocked = true;
		sm.addUpgradePoint(-1);
		hudman.updateUpgradePoints();
		GetComponentInParent<SkillTree>().unlockSkill(sm);
		skillManager.addToKnown(skill);
		GetComponentInParent<Toggle>().interactable = false;
		sm.displayUnlockablesForAllTrees();
	}

	public void upgradeSkill()
	{
		skill.UpgradeSkill();
		sm.addUpgradePoint(-1);
		hudman.updateUpgradePoints();
		sm.displayUnlockablesForAllTrees();
	}


	//This is for one-time effects (passives)
	#region passives

	public void healthUpgradeOnUnlock()
	{
		sm.setTotalHealth (sm.getTotalHealth() + skill.EffectAmount);
	}

	public void meleeUpgradeOnUnlock()
	{
		sm.setMeleeDamage (sm.getMeleeDamage() + skill.EffectAmount);
	}

	public void staminaUpgradeOnUnlock()
	{
		sm.setTotalStamina (sm.getTotalStamina() + skill.EffectAmount);
	}

	#endregion

	public void showTooltip(PointerEventData data, GameObject go)
	{
		if (skill.IsUnlocked && skill.UpgradeCount < skill.Upgrades.Count) {
			
			hudman.ShowSkillTooltip (skill, data,go);
		} else if (skill.IsUnlocked && skill.UpgradeCount > skill.Upgrades.Count) {
			skill.Description = originalDescription;
			hudman.ShowSkillTooltip (skill, data,go);
		} else {
			hudman.ShowSkillTooltip (skill, data,go);
		}
	}

	public void hideTooltip()
	{
		hudman.HideTooltip ();
	}

    public bool getUnlocked()
    {
        return isUnlocked;
    }

	private string SkillAttributeToString(SkillAttribute sa)
	{
		switch (sa) {
		case SkillAttribute.cooldown:
		case SkillAttribute.cost:
		case SkillAttribute.duration:
			return sa.ToString();
			break;
		case SkillAttribute.effectAmount:
			return "effect amount";
			break;
		case SkillAttribute.maxEnemiesHit:
			return "max enemies hit";
			break;
		}
		return "";
	}

    public void setSkill(Skill s)
	{
		skill = s;
		if (s != null) 
		{
			skillName = s.Name;
		} 
		else 
		{
			skillName = null;
		}
	}
	public Skill getSkill()
	{ 
		return skill;
	}

	public int NumberInTree
	{
		get
		{
			return numberInTree;
		}

		set
		{
			numberInTree = value;
		}
	}
}
