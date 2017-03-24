using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Skills
{
	Empty = -1, BasicAttack = 0, Fireball = 2, Heal = 3, Flamethrower=4
};

public enum SkillType
{
	Empty,Stamina, Mana, SkillCharge
};

public class SkillManager : MonoBehaviour 
{
	//Refrences to objects/components
	public List<Skill> allSkills;
	public List<Skill> knownSkills;
	private List<Item> inventoryList;
	private Inventory inventoryClassRefrence;
	private GameObject skillBar;
	private StatsManager sm;
	private Animator anim;
	private HUDManager hudman;

	void Start()
	{
		skillBar = GameObject.Find("SkillBar").gameObject;
		hudman = GetComponent<HUDManager>();
		allSkills = new List<Skill>();
		knownSkills = new List<Skill>();
		sm = GetComponent<StatsManager>();
		anim = GetComponent<Animator>();
		inventoryClassRefrence = GameObject.Find ("InventoryManager").GetComponent<Inventory> ();
		inventoryList = inventoryClassRefrence._inventory;
		setUpList();
	}

	void Update()
	{
		checkKeys();
		updateCooldowns();
	}

	//Sets up skills list
	private void setUpList()
	{
		//(string name, string description, float effect amount, float cost, float cd, Skills enumSkill, SkillType type int requirement, StatsManager sm)
		Skill skill;
		skill = AddToAllSkillsList("Fireball", "", 200f, 25f, 4f, Skills.Fireball, SkillType.Mana, 2,sm);
		skill.Description = "Hurls a flaming ball of fire forward that deals "+skill.EffectAmount + " fire damage at the cost of " +skill.Cost + " mana";
		skill.AddUpgrade(new Upgrade(50f,SkillAttribute.effectAmount,3));
		skill.AddUpgrade(new Upgrade(4f,SkillAttribute.maxEnemiesHit,4));
		skill.AddUpgrade(new Upgrade(2f,SkillAttribute.cooldown,5));
		skill = AddToAllSkillsList("Heal", "", 50f, 1f, 5f, Skills.Heal, SkillType.SkillCharge, 2,sm);
		skill.Description = "Restores " + skill.EffectAmount + " health at the cost of " + skill.Cost + " mana.";
		skill = AddToAllSkillsList("MeleeDamageUpgrade","", 100f,0f,0f,Skills.Empty,SkillType.Empty,5,sm);
		skill.Description = "Melee does " + skill.EffectAmount + " more damage";
		skill = AddToAllSkillsList("Flamethrower", "", 100f*Time.deltaTime, 35f, 10f, Skills.Flamethrower, SkillType.Mana, 4,sm);
		skill.Description = "Blasts fire in a cone in front of you that deals " + skill.EffectAmount + "/sec at the cost of " + skill.Cost + " mana.";
		AddToAllSkillsList("", "Empty", 0f, 0f, 0f, Skills.Empty, SkillType.Empty, 0,sm);
		//Links all the skill tree pieces to the actal skill 
		GameObject.Find("Canvas").BroadcastMessage("setSkill");
	}
	//Adds upgrade to particular skill
	private Skill AddToAllSkillsList (string name, string desc, float effectAmnt, float c, float cd, Skills enumSkill, SkillType st, int req, StatsManager sm)
	{
		Skill skill = new Skill (name, desc, effectAmnt, c, cd, enumSkill, st, req, sm);
		allSkills.Add(skill);
		return skill;
	}

	//Checks to see if a hotkey was pressed
	//REDO THIS METHOD BECAUSE IT'S VERY HARDCODED
	private void checkKeys()
	{
		//Note: basic attacks are not included. Those are handled in the person controller
		foreach(Skill s in knownSkills)
		{
			if (s.Hotkey != null) {
				if (Input.GetKeyUp (s.Hotkey.Code) && anim.GetInteger ("Skill") < 0) {
					if (s.CurrentCooldown >= s.Cooldown) {
						switch (s.SkillType) 
						{
							case SkillType.Mana:
								if (sm.getCurrentMana () < s.Cost) {
									hudman.displayMsg ("Not enough mana", 1f);
									return;
								} else {
									s.Use ();
									sm.useMana (s.Cost);
									return;
								}
								break;
							case SkillType.Stamina:
								if (sm.getCurrentStamina () < s.Cost) {
									hudman.displayMsg ("Not enough stamina", 1f);
									return;
								} else {
									s.Use ();
									sm.useStamina (s.Cost,false);
									return;
								}
								break;
						case SkillType.SkillCharge:
								if (inventoryList[0].Amt < s.Cost) {
									hudman.displayMsg ("Not enough skill charges", 1f);
									return;
								} else {
									s.Use ();
									inventoryClassRefrence.RemoveItemFromInventory(inventoryList[0]);
									return;
								}
								break;
						}
					}
				}
			}

		}
	}
	//Updates cooldowns for each ability
	private void updateCooldowns()
	{
		//Update cooldowns by incremementing the current cooldown and updating the fill amount of image
		if(knownSkills != null)
		{
			foreach (Skill s in knownSkills)
			{
				if (s.CurrentCooldown < s.Cooldown)
				{
					s.CurrentCooldown += Time.deltaTime;
					s.SlotAssignedTo.GetComponent<Image>().fillAmount = (s.CurrentCooldown / s.Cooldown);
				}
			}
		}
	}

	#region getters
	private IEnumerator wait(float secs)
	{
		yield return new WaitForSeconds(secs);
	}

	public List<Skill> getAllSkills()
	{
		return allSkills;
	}

	public void addToKnown(Skill sk)
	{
		knownSkills.Add(sk);
	}

	public void removeFromKnown(Skill sk)
	{
		knownSkills.Remove(sk);
	}

	public List<Skill> getKnownSkills()
	{
		return knownSkills;
	}
	#endregion
}
