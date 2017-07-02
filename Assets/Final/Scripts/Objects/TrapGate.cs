using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapGate : MonoBehaviour 
{
	[SerializeField]
	private bool isOpen;
	private Animator anim;

	void Start()
	{
		anim = GetComponent<Animator>();
		anim.SetBool("isOpen", IsOpen);
	}

	public bool IsOpen
	{
		get
		{
			return isOpen;
		}

		set
		{
			isOpen = value;
			anim.SetBool("isOpen", value);
		}
	}
}
