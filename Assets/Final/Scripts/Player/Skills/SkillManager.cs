using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum Skills
{
	Empty = -1, BasicAttack = 0, Fireball = 2, Heal = 3, Flamethrower=4, StormFlurry=5, OilPool=6, FireBomb = 7, IcePick=8, SlowingMist=9
};

public enum SkillResource
{
	Empty,Stamina, Mana, SkillCharge
};

public enum SkillType
{
	Fire, Storm, Ice,Physical, Heal,Empty
}

public class SkillManager : MonoBehaviour 
{
	//Refrences to objects/components
	public List<Skill> allSkills;
	public List<Skill> knownSkills;
	private List<string> passiveSkillNames;
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
		//Temp 
		addTestData();
	}

	void addTestData()
	{
		//addToKnown(allSkills[2]);
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
		addFireSkills();
		addStormSkills();
		addIceSkills();
		skill = AddToAllSkillsList("Heal", "Restores ~EffectAmount~ health.", 100f, 1f, 5f, Skills.Heal, SkillType.Heal,SkillResource.SkillCharge, 2,sm);
		skill = AddToAllSkillsList("MeleeDamageUpgrade","Melee does ~EffectAmount~ more damage", 100f,0f,0f,Skills.Empty,SkillType.Empty,SkillResource.Empty,5,sm);

        
		AddToAllSkillsList("", "Empty", 0f, 0f, 0f, Skills.Empty,SkillType.Empty, SkillResource.Empty, 0,sm);
		//Links all the skill tree pieces to the actal skill 
		GameObject.Find("Canvas").BroadcastMessage("setSkill");
	}

	private void addFireSkills()
	{
		Skill skill;
		skill = AddToAllSkillsList("Fireball", "Hurls a flaming ball of fire forward that deals ~EffectAmount~ fire damage.", 125f, 50f, 10f, Skills.Fireball, SkillType.Fire, SkillResource.Mana, 2, sm);
		skill.MaxEnemiesHit = 1;
		skill.AddUpgrade(new Upgrade(50f, SkillAttribute.effectAmount, 3));
		skill.AddUpgrade(new Upgrade(2f, SkillAttribute.cooldown, 5));

		skill = AddToAllSkillsList("Flamethrower", "Blasts fire in a cone in front of you that deals ~EffectAmount~ damage/second.", 50f * Time.deltaTime, 50f * Time.deltaTime, 0f, Skills.Flamethrower, SkillType.Fire, SkillResource.Mana, 4, sm, true);
		skill.AddUpgrade(new Upgrade(50f * Time.deltaTime, SkillAttribute.effectAmount, 5));

		skill = AddToAllSkillsList("Ignite", "(Passive) All fire skills have a ~HitChance~% chance to ignite enemies dealing ~EffectAmount~ damage/second for ~Duration~ seconds.", 30, 0, 0, Skills.Empty, SkillType.Fire, SkillResource.Empty, 4, sm);
		skill.Duration = 20;
		skill.HitChance = 40f;
		skill.AddUpgrade(new Upgrade(20f, SkillAttribute.effectAmount, 6));

		skill = AddToAllSkillsList("Oil Pool", "Spawns a pool of oil that can be ignited. Once ignited, it deals ~EffectAmount~ damage/second to anything standing on it for ~Duration~ seconds.", 50 * Time.deltaTime, 200, 300, Skills.OilPool, SkillType.Fire, SkillResource.Mana, 5, sm);
		skill.Duration = 30;

		skill.AddUpgrade(new Upgrade(10f, SkillAttribute.duration, 8));

		skill = AddToAllSkillsList("Fire Bomb", "Creates a fire bomb that detonates after ~Duration~ seconds and deals ~EffectAmount~ damage to anything nearby.", 100, 5, 120, Skills.FireBomb, SkillType.Fire, SkillResource.Mana, 6, sm);
		skill.Duration = 5;
		skill.AddUpgrade(new Upgrade(50f, SkillAttribute.effectAmount, 9));
	}

	private void addStormSkills()
	{
		Skill skill;

		skill = AddToAllSkillsList("Storm Flurry", "Increases attack speed by 100% and melee damage by ~EffectAmount~ for 30 seconds", 1.1f, 100f, 90, Skills.StormFlurry, SkillType.Storm, SkillResource.Mana, 2, sm);
		skill.Duration = 30f;

		skill = AddToAllSkillsList("Storm Dash", "(Passive) Makes dash deal ~EffectAmount~ damage to enemies you pass through.", 120, 0, 0f, Skills.Empty, SkillType.Storm, SkillResource.Empty, 3, sm);
	
		skill = AddToAllSkillsList("Paralyzing Strike", "(Passive) Each melee attack has a ~HitChance~% chance to paralyze the enemy for ~Duration~ seconds.", 0, 0, 0f, Skills.Empty, SkillType.Storm, SkillResource.Empty, 4, sm);
		skill.HitChance = 40f;
		skill.Duration = 2f;
	}

	private void addIceSkills()
	{
		Skill skill;

		skill = AddToAllSkillsList("Ice Pick", "Launches an ice pick forward dealing ~EffectAmount~ to the first enemy hit.", 125f, 25f, 10, Skills.IcePick, SkillType.Ice, SkillResource.Mana, 2, sm);
		skill.AddUpgrade(new Upgrade(20f, SkillAttribute.maxEnemiesHit, 4));

		skill = AddToAllSkillsList("Slowing Mist", "Creates a field of icy mist that slows enemies down by ~EffectAmount~ and lasts for ~Duration~", .6f, 100f, 90, Skills.SlowingMist, SkillType.Ice, SkillResource.Mana, 3, sm);
		skill.Duration = 30f;
	}

	public bool skillIsKnown(string name)
	{
		foreach (Skill s in knownSkills)
		{
			if (s.Name.Equals(name))
			{
				return true;
			}
		}
		return false;
	}

    //Adds upgrade to particular skill
	private Skill AddToAllSkillsList (string name, string desc, float effectAmnt, float c, float cd, Skills enumSkill, SkillType _type,SkillResource st, int req, StatsManager sm)
	{
		Skill skill = new Skill (name, desc, effectAmnt, c, cd, enumSkill, _type,st, req, sm);
		allSkills.Add(skill);
		return skill;
	}

    //Adds upgrade to particular skill with holdable or not at the end
    private Skill AddToAllSkillsList(string name, string desc, float effectAmnt, float c, float cd, Skills enumSkill, SkillType _type,SkillResource st, int req, StatsManager sm, bool tf)
    {
        Skill skill = new Skill(name, desc, effectAmnt, c, cd, enumSkill, _type,st, req, sm, tf);
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
    private void MethodIfEnoughForPress(Skill s, SkillResource type)
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
    private void MethodIfEnoughForHold(Skill s, SkillResource type)
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
    private void MethodIfEnoughForCast(Skill s, SkillResource type)
    {
        s.Use();
        sm.UseResource(type, s.Cost, false);
    }

    //Checks to see if player has enough resources to cast the spell and calls the methods passed according to the scenario
    private void CheckIfEnoughResources(Skill s, Action<Skill> methodIfNotEnough, Action<Skill, SkillResource> methodIfEnough)
    {
        switch (s.SkillResource)
        {
            case SkillResource.Mana:
                if (sm.getCurrentMana() < s.Cost)
                {
                    methodIfNotEnough(s);
                    return;
                }
                else
                {
                    methodIfEnough(s, s.SkillResource);
                    return;
                }
            case SkillResource.Stamina:
                if (sm.getCurrentStamina() < s.Cost)
                {
                    methodIfNotEnough(s);
                    return;
                }
                else
                {
                    methodIfEnough(s, s.SkillResource);
                    return;
                }
            case SkillResource.SkillCharge:
                if (inventoryList[0].Amt < s.Cost)
                {
                    methodIfNotEnough(s);
                    return;
                }
                else
                {
                    s.Use();
                    methodIfEnough(s, s.SkillResource);
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

	public Skill getSkillFromKnown(string name)
	{
		foreach (Skill s in knownSkills)
		{
			if (s.Name.Equals(name))
			{
				return s;
			}
		}
		return null;
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
	public List<string> PassiveSkillNames
	{
		get
		{
			return passiveSkillNames;
		}

		set
		{
			passiveSkillNames = value;
		}
	}

    #endregion
}
