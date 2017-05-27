using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    Animation anim;
    public float doorCost;
    [SerializeField]
    private bool isOpen;
    [Header("Last item in array must be the door")]

    [SerializeField]
    private Transform[] adjacentRooms;

    private Unlockable lockComponent;

	//Sets starting values
    void Start()
    {
        anim = GetComponent<Animation>();
        if (GetComponent<Unlockable>() != null)
        {
            lockComponent = GetComponent<Unlockable>();
        }
    }

	//Opens the door upon user interaction
    public void interact(object[] parameters)
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
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && isOpen == false)
        {
            col.transform.GetComponent<HUDManager>().displayMsg("Door requires " + doorCost + " exp and 2 keys",2f);
        }
    }
    
	//Plays the open door animation and removes the colliders
    public void openDoor()
    {
        anim.Play("OpenDoor");
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
        isOpen = true;
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

    public Transform[] getAdjacentRooms()
    {
        return adjacentRooms;
    }
	#endregion
}
