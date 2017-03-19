using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
	//Name and description of skill
	private string name, description;
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
	private Hotkey hotkey;

	private List<Upgrade> upgrades;

	private int upgradeCount;

	private bool isUnlocked;

	//Assigns all variables
	public Skill(string n, string d, float effectAmnt,  float c, float cd, Skills enumSkill, SkillType st, int req, StatsManager sm)
	{
		isUnlocked = false;
		name = n;
		description = d;
		effectAmount = effectAmnt;
		cost = c;
		cooldown = cd;
		currentCooldown = cooldown;
		anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
		currentEnumSkill = enumSkill;
		stats = sm;
		lvlRequirement = req;
		skillType = st;
		upgrades = new List<Upgrade> ();
		upgradeCount = 0;
	}


	//Method is called when skill is activated
	//It subtracts cost from stamina/mana, plays animation, and starts cooldown
	public void Use()
	{
		//Start cooldown
		StartCooldown();
		//Start animation
		anim.SetInteger("Skill", (int)currentEnumSkill);
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
			Debug.Log ("upgrading " + upgrade.AttributeToUpgrade);
			upgradeCount++;
		}
	}

	//Sets current cooldown to 0 to start cooldown
	private void StartCooldown()
	{
		currentCooldown = 0f;
	}

	#region getters/setters
	public Hotkey Hotkey {
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
	}

	public float Cost {
		get {
			return cost;
		}
	}

	public float Cooldown {
		get {
			return cooldown;
		}
	}

	public float Duration {
		get {
			return duration;
		}
	}

	public int LvlRequirement {
		get {
			return lvlRequirement;
		}
	}

	public int MaxEnemiesHit {
		get {
			return maxEnemiesHit;
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
		}
	}

	public string Description {
		get {
			return description;
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

	public void AddUpgrade(Upgrade upgrade)
	{
		upgrades.Add(upgrade);
	}

	public int UpgradeCount {
		get {
			return upgradeCount;
		}
		set {
			upgradeCount = value;
		}
	}
	#endregion

}
