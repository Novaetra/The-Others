using UnityEngine;
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
	private HUDManager hudman;
    private bool isUnlocked;
    
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
		img = gameObject.GetComponent<Image> ();
		foreach (Skill s in skillList) 
		{
			if (s.Name.Equals (skillName)) 
			{
				skill = s;
				return;
			}
		}
	}

	//Unlocks/Upgrades skills
	public void unlockOrUpgradeSkill()
    {
		if (!isUnlocked) 
		{
			isUnlocked = true;
			skill.IsUnlocked = true;
			GetComponentInParent<Toggle> ().interactable = false;
			sm.addUpgradePoint (-1);
			sm.activateUnlockable ();
			hudman.updateUpgradePoints ();
			GetComponentInParent<SkillTree> ().unlockSkill (sm);
		} 
		else 
		{
			skill.UpgradeSkill ();
			GetComponentInParent<SkillTree> ().DisplayUnlockables (sm);
		}
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

	public void showTooltip()
	{
		if (skill.IsUnlocked && skill.UpgradeCount<skill.Upgrades.Count) 
		{
			string description = "Upgrade ";
			description += skill.Upgrades [skill.UpgradeCount].AttributeToUpgrade + " by " + skill.Upgrades [skill.UpgradeCount].UpgradeAmt;
			hudman.showTooltip (skill.Name, description, ""+skill.Upgrades[skill.UpgradeCount].LvlRequirement);
		} else
		{
			hudman.showTooltip (skill.Name, skill.Description, skill.LvlRequirement.ToString ());
		}
	}

	public void hideTooltip()
	{
		hudman.hideTooltip ();
	}

    public bool getUnlocked()
    {
        return isUnlocked;
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
}
