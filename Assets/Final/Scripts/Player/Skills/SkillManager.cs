using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum Skills
{
	Empty = -1, BasicAttack = 0, Fireball = 2, Heal = 3, Flamethrower=4, StormFlurry=5
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
	private GameObject skillBar;
	private StatsManager sm;
	private Animator anim;
	private HUDManager hudman;
    private SkillInitializer skillInitializer;

    private Inventory inventoryClassRefrence;
    private List<Item> inventoryList;

    void Awake()
	{
		skillBar = GameObject.Find("SkillBar").gameObject;
		hudman = GetComponent<HUDManager>();
		allSkills = new List<Skill>();
		knownSkills = new List<Skill>();
		sm = GetComponent<StatsManager>();
		anim = GetComponent<Animator>();
        skillInitializer = GetComponent<SkillInitializer>();

        
	}

	void Start()
	{
		inventoryClassRefrence = GameObject.Find("InventoryManager").GetComponent<Inventory>();
		inventoryList = inventoryClassRefrence.InventoryList;
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
		skill = AddToAllSkillsList("Fireball", "", 100f, 25f, 4f, Skills.Fireball, SkillType.Mana, 2,sm);
		skill.Description = "Hurls a flaming ball of fire forward that deals "+skill.EffectAmount + " fire damage.";

		skill.AddUpgrade(new Upgrade(50f,SkillAttribute.effectAmount,3));
		skill.AddUpgrade(new Upgrade(4f,SkillAttribute.maxEnemiesHit,4));
		skill.AddUpgrade(new Upgrade(2f,SkillAttribute.cooldown,5));

        skill = AddToAllSkillsList("Heal", "", 100f, 1f, 5f, Skills.Heal, SkillType.SkillCharge, 2,sm);
		skill.Description = "Restores " + skill.EffectAmount + " health.";

        skill = AddToAllSkillsList("MeleeDamageUpgrade","", 100f,0f,0f,Skills.Empty,SkillType.Empty,5,sm);
		skill.Description = "Melee does " + skill.EffectAmount + " more damage";

        skill = AddToAllSkillsList("Flamethrower", "", 120f*Time.deltaTime, 50f * Time.deltaTime, 0f, Skills.Flamethrower, SkillType.Mana, 4,sm, true);
		skill.Description = "Blasts fire in a cone in front of you that deals " + (skill.EffectAmount/Time.deltaTime) + "/second.";

		skill = AddToAllSkillsList("Storm Flurry", "Increases attack speed by 100% and melee damage by 10% for 30 seconds", 1.1f, 100f, 60, Skills.StormFlurry, SkillType.Mana,2,sm);
		skill.Duration = 30f;

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

    //Adds upgrade to particular skill with holdable or not at the end
    private Skill AddToAllSkillsList(string name, string desc, float effectAmnt, float c, float cd, Skills enumSkill, SkillType st, int req, StatsManager sm, bool tf)
    {
        Skill skill = new Skill(name, desc, effectAmnt, c, cd, enumSkill, st, req, sm, tf);
        allSkills.Add(skill);
        return skill;
    }

    //Checks to see if a hotkey was pressed
    //REDO THIS METHOD BECAUSE IT'S VERY HARDCODED
    private void checkKeys()
	{
		//Note: basic attacks are not included. Those are handled in the person controllers
		foreach(Skill s in knownSkills)
		{
            if (s.Hotkey != null)
            {
                CheckKeyUp(s);
                CheckKeyDown(s);
                CheckKeyHold(s);
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

	public Skill FindSkillInKnownSkills(Skills skill)
	{
		foreach (Skill s in allSkills)
		{
			if (s.CurrentEnumSkill == skill)
			{
				return s;
			}
		}
		return null;
	}

    #region Key Checks


    //Check if a skill is cast and react accordingly
    private void CheckKeyUp(Skill s)
    {
        if (Input.GetKeyUp(s.Hotkey.Code))
        {
            //If the holdable skill was let go...
            if (s.Hotkey.IsHoldable && anim.GetInteger("Skill") == (int)s.CurrentEnumSkill)
            {
                ResetAnimator();
                ClearCurrentEffect();
            }
            else if (s.Hotkey.IsHoldable == false)
            {
                if (anim.GetInteger("Skill") < 0)
                {
                    if (s.CurrentCooldown >= s.Cooldown)
                    {
                        CheckIfEnoughResources(s, MethodIfNotEnoughForCast, MethodIfEnoughForCast);
                    }
                }
            }
        }
    }

    //Check if a holdable skill is pressed down and react accordingly
    private void CheckKeyDown(Skill s)
    {
        if (Input.GetKeyDown(s.Hotkey.Code) && s.IsHoldable)
        {
            CheckIfEnoughResources(s, MethodIfNotEnoughForPress, MethodIfEnoughForPress);
        }
    }

    //Check if a holdable skill is being held down and react accordingly
    private void CheckKeyHold(Skill s)
    {
        if (Input.GetKey(s.Hotkey.Code) && s.CanUse && s.IsHoldable)
        {
            CheckIfEnoughResources(s, MethodIfNotEnoughForHold,MethodIfEnoughForHold);
        }
    }

    //What will happen when there isn't enough resources to keep holding the spell out
    private void MethodIfNotEnoughForHold(Skill s)
    {
        s.CanUse = false;
        ResetAnimator();
    }

    //What will happen when there is enough resources to use a skill that requires hold down
    private void MethodIfEnoughForPress(Skill s, SkillType type)
    {
        s.CanUse = true;
        sm.UseResource(type, s.Cost, false);
        s.Use();
    }

    //What will happen when there is not enough resources to use a skill that requires hold down
    private void MethodIfNotEnoughForPress(Skill s)
    {
        hudman.displayMsg("Not enough", 1f);
        s.CanUse = false;
        ResetAnimator();
    }

    //What will happen when there is enough resources to keep holding the spell out
    private void MethodIfEnoughForHold(Skill s, SkillType type)
    {
        s.CanUse = true;
        sm.UseResource(type, s.Cost, false);
    }

    //What will happen when there is enough resources to cast a spell
    private void MethodIfNotEnoughForCast(Skill s)
    {
        hudman.displayMsg("Not enough", 1f);
    }

    //What will happen when there is not enough resources to cast a spell
    private void MethodIfEnoughForCast(Skill s, SkillType type)
    {
        s.Use();
        sm.UseResource(type, s.Cost, false);
    }

    //Checks to see if player has enough resources to cast the spell and calls the methods passed according to the scenario
    private void CheckIfEnoughResources(Skill s, Action<Skill> methodIfNotEnough, Action<Skill, SkillType> methodIfEnough)
    {
        switch (s.SkillType)
        {
            case SkillType.Mana:
                if (sm.getCurrentMana() < s.Cost)
                {
                    methodIfNotEnough(s);
                    return;
                }
                else
                {
                    methodIfEnough(s, s.SkillType);
                    return;
                }
            case SkillType.Stamina:
                if (sm.getCurrentStamina() < s.Cost)
                {
                    methodIfNotEnough(s);
                    return;
                }
                else
                {
                    methodIfEnough(s, s.SkillType);
                    return;
                }
            case SkillType.SkillCharge:
                if (inventoryList[0].Amt < s.Cost)
                {
                    methodIfNotEnough(s);
                    return;
                }
                else
                {
                    s.Use();
                    methodIfEnough(s, s.SkillType);
                    return;
                }
        }
    }

    #endregion


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

    private void ResetAnimator()
    {
        anim.SetInteger("Skill", -1);
    }

    private void ClearCurrentEffect()
    {
        skillInitializer.DestroyCurrentEffect();
    }
    #endregion
}
