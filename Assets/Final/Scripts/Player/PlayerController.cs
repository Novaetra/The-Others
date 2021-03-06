using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{
    public float TIME_SCALE;
    //Temp
    public int expIncreaseAmt;
    //------------
	public float walkSpeed = 4f;
	public float runSpeed = 6f;
    public float dashSpeed = 15f;
	public float currentSpeed;
    public Transform upperBody;
	public float lastUpperRot;
	public bool cursorLocked = true;
    private bool isReviving;
    private bool isGrounded = true;
	private bool canMove = true;
	private CharacterController cs;
	private Animator anim;
	private float lastFullRot;
	private float XSensitivity = 2f;
	private float YSensitivity = 2f;
	private float MinimumY = 80f;
	private float MaximumY = 70f;
	private float meleeDistance = 1.95f;
	private float interactDistance = 2f;
	private StatsManager sm;
	private SkillManager skillManager;
	private HUDManager hudman;
    [SerializeField]
    private LayerMask lyrMask;

    private Raycaster[] raycasters;

    private GameObject personReviving;

	private bool isSlowed = false;

    private bool isSettingUp = true;
	[SerializeField]
	private List<ControlHotkey> controlHotkeys;

	public void set_Up () 
	{
        lyrMask = ~lyrMask;
        cs = GetComponent<CharacterController> ();
		skillManager = GetComponent<SkillManager>();
		hudman = GetComponent<HUDManager> ();
		currentSpeed = walkSpeed;
		anim = GetComponent<Animator> ();
		lastUpperRot = 0f;
		lastFullRot = 0f;
		sm = GetComponent<StatsManager> ();
        isReviving = false;
        personReviving = null;
        raycasters = GetComponentsInChildren<Raycaster>();
		isSettingUp = false;
		controlHotkeys = new List<ControlHotkey> ();
		SetUpControlHotkeys ();
    }

	private void Update()
	{
        if(sm.getAlive() == true)
        {
            if (!isSettingUp)
			{
				CheckAllControlHotkeys();
				updateCursorLock();
				debugControls(); //Temporary
            }
        }
    }

	//Sets up all control hotkeys
	private void SetUpControlHotkeys ()
	{
		controlHotkeys.Add (new LocomotionHotkeys (new KeyCode[6]{KeyCode.LeftShift,KeyCode.E,KeyCode.Mouse0,KeyCode.L,KeyCode.R, KeyCode.Space}, this, lyrMask));
	    controlHotkeys.Add(new DisplayPanelHotkey(new KeyCode[2]{KeyCode.Tab,KeyCode.Q},this));

	}

	//Checks all control hotkeys
	private void CheckAllControlHotkeys()
	{
		foreach (ControlHotkey ch in controlHotkeys) 
		{
			ch.CheckKeys ();
		}
	}

	//Checks to see if 'interact' rays hit anything and call the 'interact' method 
	//If it hit something interactable
	public void checkInteract(RaycastHit hit)
	{
		if (hit.transform.parent != null) 
		{
            if (hit.transform.parent.tag.Equals("Room") || hit.transform.parent.tag.Equals("Spawn Room"))
            {
                hit.transform.SendMessage("interact", new object[2] { hit, gameObject }, SendMessageOptions.DontRequireReceiver);
			}
			else
            {
                hit.transform.parent.SendMessage("interact", new object[2] { hit, gameObject }, SendMessageOptions.DontRequireReceiver);
            }
		}
	}

	public void checkMelee()
	{
		RaycastHit hit;
		if(raycasters != null)
		{
			foreach (Raycaster caster in raycasters)
			{
				Debug.DrawRay(caster.transform.position, -caster.transform.forward * meleeDistance, Color.red, 1);
				if (Physics.Raycast(caster.transform.position, -caster.transform.forward, out hit, meleeDistance))
				{
					if (hit.transform.tag == "Enemy")
					{
						sm.dealMeleeDamage (hit);
						applyOnHitEffects(hit.transform.GetComponent<EnemyController>());
						return;
					}
				}
			}
		}
		//Uses stamina for the attack
		Sm.useStamina (Sm.getMeleeCost(),false);
	}

	private void applyOnHitEffects(EnemyController enemy)
	{
		foreach (Skill _skill in skillManager.getKnownSkills())
		{
			if (_skill.IsPassive && willApplyEffect(_skill.HitChance))
			{
				if (_skill.Name == "Paralyzing Strike")
				{
					enemy.SendMessage("applyParalyze",_skill.Duration,SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	private bool willApplyEffect(float hitChance)
	{
		float roll = Random.value * 100;
		if (roll <= hitChance)
		{
			return true;
		}
		return false;
	}
    
	//temporary
    private void debugControls()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
			sm.recieveExp(expIncreaseAmt);
        }

		if (Input.GetKeyUp(KeyCode.Escape))
		{
			toggleCursorLock(!cursorLocked);
		}
        /*
		if (Input.GetKeyUp (KeyCode.P)) 
		{
			UnityEditor.EditorApplication.isPaused = true;
		}
		if (Input.GetKeyUp (KeyCode.O)) {
            if (Time.timeScale == 1)
            {
                Time.timeScale = TIME_SCALE;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
     	*/
    }
		
	//Updates camera rotation at the end of each frame to avoid issues
	void LateUpdate ()
    {
        if (sm.getAlive() == true)
        {
            cameraRot();
        }
    }

	//Rotates camera according to mouse
	private void cameraRot()
	{
		if (cursorLocked == true) 
		{
			float horizontal = Input.GetAxis ("Mouse X") * XSensitivity;
			float vertical = Input.GetAxis ("Mouse Y") * YSensitivity;
            
			if ((lastUpperRot - vertical < MinimumY && lastUpperRot - vertical > -MaximumY)) 
			{
                //upperBody.transform.Rotate ((lastUpperRot - vertical),0f, 0f);
                upperBody.transform.Rotate(0f, 0f, (lastUpperRot - vertical));
                lastUpperRot = lastUpperRot - vertical;
                
            }
			else
            {
                //upperBody.transform.Rotate((lastUpperRot), 0f, 0f);
                upperBody.transform.Rotate(0f, 0f, (lastUpperRot));
            }
            transform.Rotate(0f, horizontal, 0f);
        }
		else
        {
            //upperBody.transform.Rotate((lastUpperRot), 0f, 0f);
            upperBody.transform.Rotate(0f, 0f, (lastUpperRot));
        }
	}

	public void applySlow(float amt)
	{
		if (currentSpeed >= runSpeed)
		{
			currentSpeed = walkSpeed;
		}
		currentSpeed = currentSpeed - (currentSpeed * amt);
		IsSlowed = true;
	}

	public void revertSlow(float amt)
	{
		float oldSpeed = currentSpeed / (1 - amt);
		currentSpeed = oldSpeed;
		if (currentSpeed >= runSpeed)
		{
			currentSpeed = walkSpeed;
		}
		IsSlowed = false;
	}

	public void toggleCursorLock(bool val)
	{
		cursorLocked = val;
        updateCursorLock();
	}

	private void updateCursorLock()
	{
		if (cursorLocked == true) 
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;	
		} 
		else 
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void resetAnimator()
	{
		anim.SetInteger ("Skill", -1);
	}

	#region GettersAndProperties
	public bool CanMove {
		get {
			return canMove;
		}
		set 
		{
			canMove = value;
			if (canMove == false) 
			{
				anim.SetFloat ("Speed", 0f);
				anim.SetFloat ("Direction", 0f);
			}
		}
	}

    public List<ControlHotkey> ControlHotkeys
    {
        get { return controlHotkeys; }
    }

	public HUDManager Hudman {
		get {
			return hudman;
		}
	}

	public Animator Anim {
		get {
			return anim;
		}
	}

	public CharacterController Cs {
		get {
			return cs;
		}
	}

	public StatsManager Sm {
		get {
			return sm;
		}
	}

	public Raycaster[] Raycasters {
		get {
			return raycasters;
		}
	}

	public float InteractDistance {
		get {
			return interactDistance;
		}
	}

    public float DashSpeed
    {
        get
        {
            return dashSpeed;
        }
    }

	public bool IsSlowed
	{
		get
		{
			return isSlowed;
		}

		set
		{
			isSlowed = value;
		}
	}
	#endregion
}
