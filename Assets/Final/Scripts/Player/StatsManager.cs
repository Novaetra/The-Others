﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
    //Attributes
    [SerializeField]
    private float totalHealth,currentHealth,totalMana,currentMana,totalStamina,currentStamina;
	[SerializeField]
	private float sprintStamCost;
    [SerializeField]
    public float DashStamCost { get; private set; }
    [SerializeField]
	private float meleeCost;
    [SerializeField]
	private float baseMeleeDamage;
	[SerializeField]
	private float healthUpgradeAmntOnLvlUp = 5f;
	[SerializeField]
	private float staminaUpgradeAmtOnLvlUp = 10f;

	private int currentLvl;
	private float currentExp;
	[SerializeField]
    private float totalExpRequiredToLvlUp;
	private float expGained;
	[SerializeField]
    private int upgradePoints;
    private bool isAlive;

    //private bool isReviving;
    //private float reviveDistance;
	//Multipliers
	[SerializeField]
	private float healthRegen;
	[SerializeField]
	private float manaRegen;
	[SerializeField]
    private float staminaRegen;

    //Timers
    private float stamTimer;
    private float stamCurrentTime;
    private float manaTimer;
    private float manaCurrentTime;
    private float healthTimer;
    private float healthCurrentTime;

    private Inventory inventoryClassRefrence;
    private List<Item> inventoryList;

    // private float reviveTimer;
    //private float currentReviveTimer;

    private HUDManager hudman;
	private SkillManager skillManager;
    private Animator anim;
    //private GameObject currentReviver;
    private GameManager pgm;
    [SerializeField]
    private SkillTree[] trees;

    void Start()
    {
        isAlive = true;
        trees = GameObject.Find("Canvas").GetComponentsInChildren<SkillTree>();
        //isReviving = false;
        totalHealth = 100f;
        currentHealth = 100f;
        totalMana = 100f;
        currentMana = 100f;
        totalStamina = 300f;
        currentStamina = totalStamina;
        sprintStamCost = 20f;
        DashStamCost = .3f * totalStamina;
        meleeCost = 40f;
		baseMeleeDamage = 100f;
        currentLvl = 1;
        currentExp = 0f;
        totalExpRequiredToLvlUp = 100f;
        upgradePoints = 0;
        expGained = 0f;
        //reviveDistance = 3.5f;

        healthRegen = 10f;
        manaRegen = 10f;
        staminaRegen = 50f;

        stamTimer = 2f;
        stamCurrentTime = stamTimer;
        healthTimer = 3.5f;
        healthCurrentTime = healthTimer;
        manaTimer = 2f;
        manaCurrentTime = manaTimer;

        //reviveTimer = 2f;
        //currentReviveTimer = reviveTimer;

        hudman = GetComponent<HUDManager>();
		skillManager = GetComponent<SkillManager>();
        anim = GetComponent<Animator>();
        pgm = GameObject.Find("Managers").GetComponent<GameManager>();
		hudman.updateCurrentLvlTxt ();
		GetComponent<PlayerController> ().set_Up ();
    inventoryClassRefrence = GameObject.Find("InventoryManager").GetComponent<Inventory>();
        inventoryList = inventoryClassRefrence.InventoryList;

		//TEMPORARY ------------------------------
		//addTestData();
		//TEMPORARY -------------------------------
	}

	private void addTestData()
	{
        upgradePoints = 10;
	}

    void Update()
    {
        updateAttributes();
    }

    private void updateAttributes()
    {
        if (manaCurrentTime < manaTimer)
        {
            manaCurrentTime += 1 * Time.deltaTime;
        }
        else
        {
            if (currentMana < totalMana)
            {
                currentMana += manaRegen * Time.deltaTime;
            }
        }
        if (stamCurrentTime < stamTimer)
        {
            stamCurrentTime += 1 * Time.deltaTime;
        }
        else
        {
            if (currentStamina < totalStamina) {
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }

        if (healthCurrentTime < healthTimer)
        {
            healthCurrentTime += 1 * Time.deltaTime;
        }
        else
        {
            if (currentHealth < totalHealth)
            {
                currentHealth += healthRegen * Time.deltaTime;
            }
        }
    }

	public void recieveExp(float exp)
    {
        currentExp += exp;
        expGained += exp;
        checkIfDisplayLvlUpHint ();
    }

    public float getExpGained()
    {
        return expGained;
    }

    public void resetExpGained()
    {
        expGained = 0f;
    }

	public void lvlUp(float leftOver)
    {
		upgradeStats ();
        currentExp = leftOver;
		currentLvl++;
		if (currentLvl < 4) 
		{
			totalExpRequiredToLvlUp *= 2f;
		} else 
		{
			totalExpRequiredToLvlUp += totalExpRequiredToLvlUp * .1f;
		}
        upgradePoints++;
        displayUnlockablesForAllTrees();
		hudman.updateCurrentLvlTxt ();
        displayLvlUpTxt();
    }
	private void upgradeStats()
	{
		totalHealth += healthUpgradeAmntOnLvlUp;
		totalStamina += staminaUpgradeAmtOnLvlUp;
		if (currentLvl < 3) 
		{
			baseMeleeDamage *= 1.5f;
			totalMana *= 1.5f;
		}
		else 
		{
			baseMeleeDamage += baseMeleeDamage * .05f;
			totalMana += totalMana * .05f;
		}

        DashStamCost = .3f * totalStamina;
		upgradeAllDamageSkills();
    }

	private void upgradeAllDamageSkills()
	{
		foreach (Skill s in skillManager.getAllSkills())
		{
			if (s.LvlRequirement < currentLvl)
			{
				if (s.SkillType != SkillType.Heal)
				{
					if (currentLvl < 4)
					{
						s.EffectAmount *= 1.5f;
					}
					else
					{
						s.EffectAmount *= 1.1f;
					}
					s.fillInDescriptionData();
				}
			}
		}
	}

	public void recieveDamage(float dmg)
    {
        currentHealth -= dmg;
        hudman.updateBars();
        checkDeath();
        hudman.showRecievedDamage();
        healthCurrentTime = 0f;
    }

	public void recieveDamage(object[] parameters)
	{
		float dmg = (float)parameters[0];
		currentHealth -= dmg;
		hudman.updateBars();
		checkDeath();
		hudman.showRecievedDamage();
		healthCurrentTime = 0f;
	}

    private void checkDeath()
    {
        if (currentHealth <= 0f)
        {
            //Death
            death();
        }
    }
    /*
    public void startRevive(GameObject reviver)
    {
        //So it doesn't get called more than once
           if(isReviving==false)
           {
            currentReviver = reviver;
            currentReviveTimer = 0f;
            isReviving = true;
        }
    }

    public void checkRevive()
    {
        if (isReviving == true)
        {
            float distance = Vector3.Distance(transform.position, currentReviver.transform.position);
            if (distance <= reviveDistance && currentReviver.GetComponent<PlayerController>().getReviving() == true && isAlive == false)
            {
                if (currentReviveTimer < reviveTimer)
                {
                    currentReviveTimer += 2 * Time.deltaTime;
                }
                else if (currentReviveTimer >= reviveTimer)
                {
                    
                }
            }
            else
            {
                isReviving = false;
                currentReviveTimer = 0;
                currentReviver.GetComponent<PlayerController>().setPersonReviving(null);
            }
        }
    }
    */

    void death()
    {
        isAlive = false;
        pgm.checkGameEnded();
        anim.SetBool("isAlive",isAlive);
    }
    
    void revive()
    {
        isAlive = true;
        anim.SetBool("isAlive", isAlive);
        currentHealth = totalHealth;
    }

    public void useMana(float mana)
    {
        currentMana -= mana;
        manaCurrentTime = 0f;
    }

    //Bool is if its over time or not
    public void useStamina(float stam, bool tf)
    {
        if (tf)
        {
            currentStamina -= stam * Time.deltaTime;
        }
        else
        {
            currentStamina -= stam;
        }

        stamCurrentTime = 0f;
    }

    public void UseResource(SkillResource type, float amt, bool overTime)
    {
        switch(type)
        {
            case SkillResource.Mana:
                useMana(amt);
                break;
            case SkillResource.Stamina:
                useStamina(amt, overTime);
                break;
            case SkillResource.SkillCharge:
                inventoryClassRefrence.RemoveItemFromInventory(inventoryList[inventoryClassRefrence.GetIndxOfItem(0)]);
                break;
        }
    }
    public void displayUnlockablesForAllTrees()
    {
        hudman.updateUpgradePoints();

        foreach (SkillTree tree in trees)
        {
            tree.displayUnlockablesForOneTree(this);
        }
    }

    #region gettersSetters

    /*public bool getReviving()
    {
        return isReviving;
    }
    public float getCurrentReviveTimer()
    {
        return currentReviveTimer;
    }

    public float getTotalReviveTimer()
    {
        return reviveTimer;
    }
    */
    public bool getAlive()
    {
        return isAlive;
    }

    public float getMeleeCost()
	{
		return meleeCost;
	}

	public float getTotalHealth()
	{
		return totalHealth;
	}

	public float getCurrentHealth()
	{
		return currentHealth;
	}

    public void setCurrentHealth(float h)
    {
        currentHealth = h;
    }

	public float getTotalMana()
	{
		return totalMana;
	}

	public float getCurrentMana()
	{
		return currentMana;
	}

	public float getTotalStamina()
	{
		return totalStamina;
	}

    public void setCurrentStamina(float s)
    {
        currentStamina = s;
    }

	public float getCurrentStamina()
	{
		return currentStamina;
	}

	public float getCurrentExp()
	{
		return currentExp;
	}

    public int getCurrentLvl()
    {
        return currentLvl;
    }

    public int getUpgradePnts()
    {
        return upgradePoints;
    }

    public void addUpgradePoint(int num)
    {
        upgradePoints += num;
    }

    public float getGoalExp()
	{
		return totalExpRequiredToLvlUp;
	}

    public void subtractExp(float pnts)
    {
		currentExp -= pnts;
    }

	public float getSprintStamCost()
	{
		return sprintStamCost;
	}

	public float getMeleeDamage()
	{
		return baseMeleeDamage;
	} 

	public void setMeleeDamage(float dmg)
	{
		baseMeleeDamage = dmg;
	}

	public void setTotalStamina(float stam)
	{
		totalStamina = stam;
	}

	public void setTotalHealth(float health)
	{
		totalHealth = health;
	}

	public void dealMeleeDamage(RaycastHit hit)
	{
		hit.transform.SendMessage("recieveDamage", baseMeleeDamage);
	}

	#endregion

	private void displayLvlUpTxt()
	{
        hudman.displayMsg("You have reached level " + currentLvl, 2f);
	}


	private void checkIfDisplayLvlUpHint()
	{
		if (currentExp >= totalExpRequiredToLvlUp) 
		{
			hudman.displayMsg ("Press L to level up", 1f);
		}
	}
}
