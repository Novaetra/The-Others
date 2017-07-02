using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonGateDoorComponent : Door 
{
	public override void interact(object[] parameters)
	{
		//Interact will be handled by unlockable component
	}

	public override void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player" && isOpen == false)
		{
			col.transform.GetComponent<HUDManager>().displayMsg("Requires two keys", 1f);
		}
	}
}
