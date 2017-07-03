using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
	//Name and description of skill
	private string name, description, originalDescription;
	//Effect amount is dmg or heal amnt, 
	private float effectAmount,cost,cooldown,currentCooldown, duration;
	//Lvl requirement to unlock
	//Max enemies hit is the max number of enemies that can be affected by the spell
	private int lvlRequirement,maxEnemiesHit;
	//Refrence to the gameobject this skill is assigned to
	private GameObject slotAssignedTo;
	//Refrence to animator to play animations
	private Animator anim;
	//The skill # assigned to this skill for animation purposes
	private Skills currentEnumSkill;
	//Skill type to determine whether to use stamina or mana
	private SkillType skillType;
	//Refrence to stats manager to modify and read player's stats
	private StatsManager stats;
	//Hotkey assigned to it
	private SkillHotkey hotkey;
    private HUDManager hudMan;
	private List<Upgrade> upgrades;

	[SerializeField]
	private Upgrade nextUpgrade; 

	private int upgradeCount;

	private bool isUnlocked,isHoldable, canUse;
	//Assigns all variables
	public Skill(string n, string d, float effectAmnt,  float c, float cd, Skills enumSkill, SkillType st, int req, StatsManager sm)
	{
		isUnlocked = false;
		name = n;
		description = d;
        originalDescription = d;
        effectAmount = effectAmnt;
		cost = c;
		cooldown = cd;
		currentCooldown = cooldown;
		anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
		CurrentEnumSkill = enumSkill;
		stats = sm;
		lvlRequirement = req;
		skillType = st;
		upgrades = new List<Upgrade> ();
		upgradeCount = 0;
        isHoldable = false;
        hudMan = GameObject.FindGameObjectWithTag("Player").GetComponent<HUDManager>();
    }

    //Assigns all variables
    public Skill(string n, string d, float effectAmnt, float c, float cd, Skills enumSkill, SkillType st, int req, StatsManager sm, bool hold)
    {
        isUnlocked = false;
        name = n;
        description = d;
        originalDescription = d;
        effectAmount = effectAmnt;
        cost = c;
        cooldown = cd;
        currentCooldown = cooldown;
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        CurrentEnumSkill = enumSkill;
        stats = sm;
        lvlRequirement = req;
        skillType = st;
        upgrades = new List<Upgrade>();
        upgradeCount = 0;
        isHoldable = hold;
        hudMan = GameObject.FindGameObjectWithTag("Player").GetComponent<HUDManager>();
    }


    //Method is called when skill is activated
    //It subtracts cost from stamina/mana, plays animation, and starts cooldown
    public void Use()
	{
		//Start cooldown
		StartCooldown();
		//Start animation
		anim.SetInteger("Skill", (int)CurrentEnumSkill);
	}

	public void UpgradeSkill()
	{
		if (upgradeCount < upgrades.Count) 
		{
			Upgrade upgrade = upgrades [upgradeCount];
			switch (upgrade.AttributeToUpgrade) {
			case SkillAttribute.effectAmount:
				effectAmount += upgrade.UpgradeAmt;
				break;
			case SkillAttribute.cooldown:
				cooldown -= upgrade.UpgradeAmt;
				if (cooldown < 0) {
					cooldown = 0.5f;
				}
				break;
			case SkillAttribute.cost:
				cost += upgrade.UpgradeAmt;
				break;
			case SkillAttribute.duration:
				duration += upgrade.UpgradeAmt;
				break;
			case SkillAttribute.maxEnemiesHit:
				maxEnemiesHit += (int)upgrade.UpgradeAmt;
				break;
			}
            upgradeCount++;

			if (upgradeCount < upgrades.Count)
			{
				Debug.Log("set next upgrade here...");
				nextUpgrade = upgrades[upgradeCount];
			}
			else
			{
				nextUpgrade = null;
			}

			hudMan.UpdateToolTip(this);
        }
	}

	//Sets current cooldown to 0 to start cooldown
	private void StartCooldown()
	{
		currentCooldown = 0f;
	}

	#region getters/setters
	public SkillHotkey Hotkey {
		get {
			return hotkey;
		}
		set {
			hotkey = value;
		}
	}

	public float EffectAmount {
		get {
			return effectAmount;
		}
		set{
			effectAmount = value;
		}
	}

	public float Cost {
		get {
			return cost;
		}
		set{
			cost = value;
		}
	}

	public float Cooldown {
		get {
			return cooldown;
		}
		set{
			cooldown = value;
		}
	}

	public float Duration {
		get {
			return duration;
		}
		set{
			duration = value;
		}
	}

	public int LvlRequirement {
		get {
			return lvlRequirement;
		}
		set{
			lvlRequirement = value;
		}
	}

	public int MaxEnemiesHit {
		get {
			return maxEnemiesHit;
		}
		set{
			maxEnemiesHit = value;
		}
	}

	public string Name {
		get {
			return name;
		}
	}

	public bool IsUnlocked {
		get {
			return isUnlocked;
		}
		set {
            isUnlocked = value;
			hudMan.UpdateToolTip(this);
        }
	}

	public string Description {
		get {
			return description;
		}
		set{
			description = value;
		} 
	}

	public SkillType SkillType {
		get {
			return skillType;
		}
	}

	public float CurrentCooldown {
		get {
			return currentCooldown;
		}
		set {
			currentCooldown = value;
		}
	}

	public GameObject SlotAssignedTo {
		get {
			return slotAssignedTo;
		}
		set {
			slotAssignedTo = value;
		}
	}

	public List<Upgrade> Upgrades {
		get {
			return upgrades;
		}
		set {
			upgrades = value;
		}
	}

	public void AddUpgrade(Upgrade _upgrade)
	{
		upgrades.Add(_upgrade);
		nextUpgrade = upgrades[0];
	}

	public int UpgradeCount {
		get {
			return upgradeCount;
		}
		set {
			upgradeCount = value;
		}
	}

    public bool IsHoldable
    {
        get
        {
            return isHoldable;
        }

        set
        {
            isHoldable = value;
        }
    }

    public Skills CurrentEnumSkill
    {
        get
        {
            return currentEnumSkill;
        }

        set
        {
            currentEnumSkill = value;
        }
    }

    public bool CanUse
    {
        get
        {
            return canUse;
        }

        set
        {
            canUse = value;
        }
    }

	public Upgrade NextUpgrade
	{
		get
		{
			return nextUpgrade;
		}

		set
		{
			nextUpgrade = value;
		}
	}

	public float GetAttribute(SkillAttribute at)
	{
		switch (at)
		{
			case SkillAttribute.effectAmount:
				return effectAmount;
			case SkillAttribute.duration:
				return duration;
			case SkillAttribute.cooldown:
				return cooldown;
			case SkillAttribute.cost:
				return cost;
			case SkillAttribute.maxEnemiesHit:
				return (float)maxEnemiesHit;
			default:
				return -1f;
		}
	}
    #endregion

}
