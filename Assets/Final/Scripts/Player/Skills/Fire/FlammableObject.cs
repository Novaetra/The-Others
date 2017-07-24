using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableObject : SkillGameObject 
{
	private bool isOnFire;

	virtual public void ignite()
	{
		IsOnFire = true;
	}

	public bool IsOnFire
	{
		get
		{
			return isOnFire;
		}

		set
		{
			isOnFire = value;
		}
	}

}
