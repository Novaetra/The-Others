using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlHotkey 
{
	protected KeyCode[] codes;
	protected PlayerController controller;

	public ControlHotkey(){}

	public ControlHotkey(KeyCode[] c,PlayerController cntrl)
	{
		codes = c; 
		controller = cntrl;
	}
	//Will check each key to see if pressed and will react accordingly
	public virtual void CheckKeys (){}

	public KeyCode[] Codes {
		get {
			return codes;
		}
	}
}
