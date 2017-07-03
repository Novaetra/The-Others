using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillAttribute
{
	effectAmount,cost,cooldown, duration, maxEnemiesHit
}

public class Upgrade 
{
	private float upgradeAmt;
	private SkillAttribute attributeToUpgrade;
	private int lvlRequirement;

	public Upgrade(float amt, SkillAttribute name, int lvl)
	{
		upgradeAmt = amt;
		attributeToUpgrade = name;
		lvlRequirement = lvl;
	}

	#region getters
	public float UpgradeAmt {
		get {
			return upgradeAmt;
		}
		set {
			upgradeAmt = value;
		}
	}

	public SkillAttribute AttributeToUpgrade {
		get {
			return attributeToUpgrade;
		}
		set {
			attributeToUpgrade = value;
		}
	}
	public int LvlRequirement {
		get {
			return lvlRequirement;
		}
		set {
			lvlRequirement = value;
		}
	}
	#endregion

}
