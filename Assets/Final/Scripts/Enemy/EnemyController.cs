using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	protected UnityEngine.AI.NavMeshAgent agent;
	protected Transform targetPlayer;
	protected Animator anim;
	protected float playerDistance;
	[SerializeField]
	protected float proximityRange;
	protected float attackRange = 1.5f;
	[SerializeField]
	protected float meleeDamage;
	protected float movementSpeed;

    [SerializeField]
    protected float numberOfAttacks;
    //private float totalHealth;
    [SerializeField]
    protected float totalHealth, currentHealth, expOnKill;

	protected bool doneSettingUp = false;
	protected bool isAlive = true;
    private bool alreadySpawnedItem = false;
    //Keeps track of how many times been hit to trigger hit response every 4th hit
    private int hitResponseFrequency = 4;
    private int tookDamageCount = 0;
    //-------------

    protected Raycaster[] casters;

	private List<Item> inventory;

    private EnemyManager enemyMan;
    private bool isInside;
    [SerializeField]
    private GameObject blood;

	private Inventory invClass;

    //Assigns the enemy's nav agent, animator, and list of players
    public void Start()
    {
		currentHealth = totalHealth;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.SetFloat("Health", currentHealth);
        casters = GetComponentsInChildren<Raycaster>();
        doneSettingUp = true;
        targetPlayer = GameObject.Find("Player").transform;
        //proximityRange = agent.stoppingDistance + 1f;
        movementSpeed = 1.5f;
		invClass = GameObject.Find("InventoryManager").GetComponent<Inventory>();
		inventory = invClass.AvailableItemsList;
		DoAdditionalSetup ();
        enemyMan = GameObject.Find("Managers").transform.GetComponent<EnemyManager>();

    }

	//This method is used to do additional setup for children
	protected virtual void DoAdditionalSetup ()
	{
		//Additional setup	
	}

    //Finds closest player, follow it, and if player is within melee range, attacks player
	//Check if enemy is alive
    void Update()
    {
        if (doneSettingUp == true && IsAlive == true)
        {
            chasePlayer();
        }
    }
    

	//Chases the closest player
	protected virtual void chasePlayer()
    {
		//Get closest player and assign it as the target
        //targetPlayer = GetClosestEnemy(livePlayers.ToList());

        if (targetPlayer != null && IsAlive == true)
        {
			//Gets the distance between the player and the enemy
            playerDistance = Vector3.Distance(targetPlayer.transform.position, gameObject.transform.position);
            //If the distance is greater than the enemy's melee range, then walk toward the target player
            if (playerDistance > proximityRange)
                {
				DoNotWithinProximity (playerDistance);
                }
                //Else if the player is within melee range, attack
                else
                {
					DoWithinProximity (playerDistance);
                }
        }
    }

	protected virtual void DoWithinProximity(float playerDistance)
	{
		animatorSpeedToZero();
		attackAnim();
		// agent.enabled = false;
		rotateTowards(targetPlayer);
	}

	protected virtual void DoNotWithinProximity(float playerDistance)
	{
		agent.enabled = true;
		agent.SetDestination(targetPlayer.position);
		walkAnim();
		resetAnimator();
		rotateTowards(targetPlayer);
	}

	protected void rotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * movementSpeed);
    }

    //Throws rays to check if the melee attack hit a player
    //If it hit a player, then apply damage
    public virtual void checkAttack()
    {
        if (IsAlive && isInside)
        {
            /*
            RaycastHit hit;
            foreach (Raycaster caster in casters)
            {
                Transform raycaster = caster.transform;
                Debug.DrawRay(raycaster.position, raycaster.forward * attackRange, Color.blue, 1);
                if (Physics.Raycast(raycaster.position, raycaster.forward, out hit, attackRange))
                {
                    if (hit.transform.tag == "Player")
                    {
                        hit.transform.GetComponent<StatsManager>().recieveDamage(meleeDamage);
                        return;
                    }
                }
            }
            */
            GameManager.currentplayer.GetComponent<StatsManager>().recieveDamage(MeleeDamage);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.transform.tag =="Player")
        {
            isInside = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            isInside = false;
        }
    }

	protected void checkPlayerPassives(SkillType type)
	{
		SkillManager skillManager = targetPlayer.GetComponent<SkillManager>();
		if (skillManager.skillIsKnown("Ignite") && type == SkillType.Fire && transform.Find("Ignite(Clone)")==null)
		{
			float roll = Random.value * 10;
			if (roll <= 2.5)
			{
				applyIgnite();			
			}
		}
	}

	private void applyIgnite()
	{
		GameObject igniteGO = (GameObject)Resources.Load("Spells/Ignite");;
		GameObject ignite = (GameObject)Instantiate(igniteGO, Vector3.zero, Quaternion.identity);
		ignite.transform.SetParent(transform);
		ignite.transform.localPosition = new Vector3(0,0,0);
		ignite.transform.localRotation = Quaternion.Euler(270, 0, 0);
	}

	//Lower the enemy's health by x amt
	public void recieveDamageWithType(float dmg, SkillType type)
    {
        currentHealth -= dmg;
        anim.SetFloat("Health", currentHealth);
		if (type == SkillType.Empty || type == SkillType.Physical)
		{
			splatterBlood();
		}
		checkPlayerPassives(type);
		if (currentHealth<=0 && isAlive)
        {
            startDeath();
            return;
        }

        if (tookDamageCount>=hitResponseFrequency)
        {
            anim.SetTrigger("TakeDamage");
            tookDamageCount = 0;
        }
        tookDamageCount++;
		AfterTakingDamage();
    }

	public void recieveDamage(float dmg)
	{
		recieveDamageWithType(dmg, SkillType.Empty);
	}
	private void splatterBlood()
	{
		GameObject _blood = GameObject.Instantiate(blood, transform.Find("Blood Splatter").transform);
		_blood.transform.localPosition = new Vector3(0f, 0f, 0f);
		_blood.transform.localScale = new Vector3(1f, 1f, 1f);//(0.5f, 0.5f, 0.5f);
	}

	//Is called from animation event and starts the die coroutine
    public void startDeath()
    {
		PossiblySpawnPowerup ();
        die();
    }

    public void Kill()
    {
        IsAlive = false;
        agent.enabled = false;

        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<Rigidbody>());
        anim.SetFloat("Health", -1);
        StartCoroutine(destroy(5f));
    }

	private void PossiblySpawnPowerup()
	{
        if(!alreadySpawnedItem)
        {
            int randNum = (int)Random.Range(0, 100);
            int lastDropRate = 0;
            foreach(Item i in inventory)
            {
               // Debug.Log(" I want to spawn a " + i.Name + " because the number is " + randNum + " and the droprate is " + i.DropRate + " and the last drop was " + lastDropRate);
                if (randNum >= lastDropRate && randNum < i.DropRate+lastDropRate && i.Amt < i.Max)
                {
					spawnPowerUp(i);
                    return;
                }
                lastDropRate = i.DropRate+1;
            }
        }
    }

	protected void spawnPowerUp(Item i)
	{
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load(i.Name), new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
		Item item = obj.GetComponent<Item>();
		item.ItemIDNumber = i.ItemIDNumber;
		alreadySpawnedItem = true;
	}

	protected void spawnPowerUp(int itemID)
	{
		Item i = invClass.GetItemFromID(itemID);

		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load(i.Name), new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
		Item item = obj.GetComponent<Item>();
		item.ItemIDNumber = i.ItemIDNumber;
		alreadySpawnedItem = true;
	}

	//Firsts starts the death animation, waits x seconds, and then destroys the enemy
    private void die()
    {
        if (IsAlive == true)
        {
            IsAlive = false;
            agent.enabled = false;

            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<Rigidbody>());
            sendPlayersExp();
            enemyMan.decreaseEnemyCount();
            enemyMan.RemoveEnemyFromList(gameObject);
        }
		DoAdditionalOnDeath();
        StartCoroutine(destroy(20f));
    }

	protected virtual void DoAdditionalOnDeath() { }

    private IEnumerator destroy(float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(gameObject);
    }

	public void setTotalHealth(float h)
	{
		totalHealth = h;
		currentHealth = h;
	}

	public void setMeleeDamage(float d)
	{
		MeleeDamage = d;
	}

	public void setExpOnKill(float exp)
	{
		expOnKill = exp;
	}

    public void setMovementSpeed(float speed)
    {
        GetComponent<NavMeshAgent>().speed = speed;
        movementSpeed = speed;
    }

    public void setAttackProximity(float range)
    {
        proximityRange = range;
    }

	//Temporary...might just give the person with the killing blow any exp
	//Send all players exp
    void sendPlayersExp()
    {
        GameManager.currentplayer.GetComponent<StatsManager>().recieveExp(expOnKill);
    }

	//Triggers an attack animation on all clients
	protected void attackAnim()
    {
        int attack = (int) Random.RandomRange(1, numberOfAttacks+1);
        GetComponent<Animator>().SetInteger("Skill", attack);
    }

	protected virtual void resetAnimator()
    {
        if(isAlive)
            GetComponent<Animator>().SetInteger("Skill", 0);
    }

	protected virtual void AfterTakingDamage()
	{
	}

    //Triggers an walk animation on all clients
	protected void walkAnim()
    {
		GetComponent<Animator>().SetFloat("Speed", agent.speed);
    }

	public void animatorSpeedToZero()
    {
        GetComponent<Animator>().SetFloat("Speed", 0);
    }

	protected void disableAgent()
	{
		agent.enabled = false;
	}

	protected void enableAgent()
	{
		agent.enabled = true;
	}

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }

        set
        {
            isAlive = value;
        }
    }

	public float MeleeDamage
	{
		get
		{
			return meleeDamage;
		}

		set
		{
			meleeDamage = value;
		}
	}
}