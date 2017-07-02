using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkill
{
	private Action action;
	private int probability;
	public EnemySkill(Action action, int probability)
	{
		this.action = action;
		this.probability = probability;
	}

	public Action Action
	{
		get
		{
			return action;
		}

		set
		{
			action = value;
		}
	}

	public int Probability
	{
		get
		{
			return probability;
		}

		set
		{
			probability = value;
		}
	}
}
