using UnityEngine;
using System.Collections;

public class TestPlayerController : MonoBehaviour 
{

	//Temp
	public int expIncreaseAmt;
	public int baseMeleeDamage;
    //------------
	public float walkSpeed = 4f;
	public float runSpeed = 6f;
	public float currentSpeed;
    public Transform upperBody;
	public float lastUpperRot;
	public bool cursorLocked = true;
    private bool isReviving;
    private bool isGrounded = true;
	private CharacterController cs;
	private Animator anim;
	private float lastFullRot;
	private float XSensitivity = 2f;
	private float YSensitivity = 2f;
	private float MinimumY = 80f;
	private float MaximumY = 70f;
	private float meleeDistance = 1.5f;
    private float interactDistance = 2f;

    private Raycaster[] raycasters;

    private bool isSettingUp = true;

	public void Start() 
	{
		cs = GetComponent<CharacterController> ();
		currentSpeed = walkSpeed;
		anim = GetComponent<Animator> ();
		lastUpperRot = 0f;
		lastFullRot = 0f;
        isReviving = false;
        raycasters = GetComponentsInChildren<Raycaster>();
        isSettingUp = false;
    }

	private void Update()
	{
        if (!isSettingUp)
        {
            checkClick();
            checkSprint();
            checkMovement();
            updateCursorLock();
            checkInteract();
        }
    }
    
	void LateUpdate ()
    {
        cameraRot();
    }

	private void checkClick()
	{
		if (Input.GetButtonUp ("Click")&& (cursorLocked)) 
		{
            //Attack
			anim.SetInteger("Skill",(int)Skills.BasicAttack);
		}
	}

    private void checkInteract()
    {
        //Check if E is pressed down
        if(Input.GetKey(KeyCode.E) )
        {
            throwRays();
        }
        //If its not, then set anything that relies on it to false
        else
        {
            isReviving = false;
        }
    }

    private void throwRays()
    {
        RaycastHit hit;
        foreach (Raycaster caster in raycasters)
        {
            Transform raycaster = caster.transform;
            Debug.DrawRay(raycaster.transform.position, -raycaster.transform.forward * interactDistance, Color.blue);
            if (Physics.Raycast(raycaster.transform.position, -raycaster.transform.forward, out hit, interactDistance))
            {
                //Has to check these two separately because each one has their own specific protocols
               // checkRevive(hit);
                checkInteract(hit);
            }
        }
    }

    private void checkInteract(RaycastHit hit)
    {
        if ((hit.transform.parent.name.Substring(0,hit.transform.parent.name.Length-1)) != "Room")
        {

            hit.transform.parent.SendMessage("interact", new object[2] { hit, gameObject }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            hit.transform.SendMessage("interact", new object[2] { hit, gameObject }, SendMessageOptions.DontRequireReceiver);
        }
    }

    /*
	private void checkRevive(RaycastHit hit)
    {
        if (hit.transform.tag == "Player")
        {
            if (hit.transform.GetComponent<StatsManager>().getAlive() == false)
            {
                isReviving = true;
                personReviving = hit.transform.gameObject;
                hit.transform.GetComponent<StatsManager>().startRevive(gameObject);
            }
        }
    }
    */

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
						dealMeleeDamage (hit);
                        return;
                    }
                }
            }
        }
       

    }

	private void dealMeleeDamage(RaycastHit hit)
	{
		hit.transform.SendMessage("recieveDamage", baseMeleeDamage);
	}


	private void checkSprint()
	{
		if (Input.GetButton ("Sprint"))
		{
				currentSpeed = runSpeed;
		}

        if(Input.GetButtonUp("Sprint"))
        {
            currentSpeed = walkSpeed;
        }
	}

	private void checkMovement()
	{
        float speed = Input.GetAxis("Vertical") * currentSpeed;
        float direction = Input.GetAxis("Horizontal") * currentSpeed;
        anim.SetFloat("Speed", speed);
        anim.SetFloat("Direction", direction);
        Vector3 finalMove = new Vector3 (direction, 0f, speed);
		finalMove = transform.rotation * finalMove;;
        cs.Move(finalMove * Time.deltaTime);
    }

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

    public bool getReviving()
    {
        return isReviving;
    }

	public void resetAnimator()
	{
		anim.SetInteger ("Skill", -1);
	}
}
