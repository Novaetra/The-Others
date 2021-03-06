﻿using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
	protected Animator anim;
    public float doorCost;
    [SerializeField]
	protected bool isOpen;
    [Header("Last item in array must be the door")]

    [SerializeField]
    private Transform[] adjacentRooms;

    private Unlockable lockComponent;

	//Sets starting values
    void Start()
    {
		anim = GetComponent<Animator>();
        if (GetComponent<Unlockable>() != null)
        {
            lockComponent = GetComponent<Unlockable>();
        }
    }

	//Opens the door upon user interaction
    public virtual void interact(object[] parameters)
    {
        RaycastHit hit = (RaycastHit)parameters[0];
        GameObject player = (GameObject)parameters[1];
        StatsManager sm = player.GetComponent<StatsManager>();
		//If the player has enough to buy the door, open the door (on all clients)

        if (lockComponent != null)
        {
            if (lockComponent.IsUnlocked)
            {

                if (sm.getCurrentExp() - hit.transform.GetComponentInParent<Door>().getCost() >= 0 && hit.transform.GetComponentInParent<Door>().getOpen() == false)
                {
                    openDoor();
                    //Subtract the exp from the player
                    sm.subtractExp(hit.transform.GetComponentInParent<Door>().getCost());
                }
            }
        }
        else
        {
            if (sm.getCurrentExp() - hit.transform.GetComponentInParent<Door>().getCost() >= 0 && hit.transform.GetComponentInParent<Door>().getOpen() == false)
            {
                openDoor();
                //Subtract the exp from the player
                sm.subtractExp(hit.transform.GetComponentInParent<Door>().getCost());
            }
        }
    }
    
	//This displays the door cost if the player approaches it
	public virtual void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && isOpen == false)
        {
            col.transform.GetComponent<HUDManager>().displayMsg("Door requires " + doorCost + " exp",2f);
        }
    }
    
	//Plays the open door animation and removes the colliders
    public virtual void openDoor()
    {
		anim.SetBool("isOpen",true);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
        isOpen = true;
		GameObject.Find("Managers").GetComponent<EnemyManager>().updateSpawnsAvailable();
    }


	#region getters
    public float getCost()
    {
        return doorCost;
    }

    public bool getOpen()
    {
        return isOpen;
    }

	public void setOpen(bool tf)
	{
		isOpen = tf;
		GameObject.Find("Managers").GetComponent<EnemyManager>().updateSpawnsAvailable();
	}

    public Transform[] getAdjacentRooms()
    {
        return adjacentRooms;
    }
	#endregion
}
