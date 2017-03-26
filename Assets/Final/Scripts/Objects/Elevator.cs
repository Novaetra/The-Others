using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{

    private bool isDown = true;
	private bool isMoving = false;
    private bool canMove = true;
    private GameObject player;
	private Animation anim;
    private EnemyManager enemyMan;

    public float waitTime;

	void Start()
	{
		anim = GetComponent<Animation> ();
        enemyMan = GameObject.Find("Managers").GetComponent<EnemyManager>();
	}

	public void interact(object[] paramtrs)
    {
        player = (GameObject)paramtrs[1];
		//If the elevator isn't moving
		if (!isMoving && canMove) 
		{
            enemyMan.CurrentRoom = null;
			//If the elevator is downstairs, make it go up
			player.GetComponent<PlayerController>().CanMove = false;
			if(isDown)
			{
				isDown = false;
				isMoving = true;
				anim.Play("ElevatorUp");
			}
			//if the elevator is upstairs, make it go down
			else
			{
				isDown = true;
				isMoving = true;
				anim.Play("ElevatorDown");
			}
            StartCoroutine(StartWaitTime());
		}
    }

    private IEnumerator StartWaitTime()
    {
        canMove = false;
        yield return new WaitForSeconds(waitTime);
        canMove = true;
    }

	//Reset the player's parent so it isn't attached to elevator anymore
    private void resetPlayerParent()
    {
        player.transform.parent = null;
		player.GetComponent<PlayerController> ().CanMove = true;
    }
    
	//Sets moving to false so it can be interacted with again
	private void stoppedMoving()
	{
		isMoving = false;
	}

	private void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			col.transform.parent = transform;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player" && !isMoving) 
		{
			col.transform.parent = null;
		}
	}

}
