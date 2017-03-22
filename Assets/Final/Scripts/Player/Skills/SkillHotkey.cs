using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHotkey 
{
	private KeyCode code;
	private Skill skill;

	public SkillHotkey(KeyCode c, Skill s)
	{
		code = c;
		skill = s;
	}

	public KeyCode Code {
		get {
			return code;
		}
		set {
			code = value;
		}
	}

	public Skill Skill {
		get {
			return skill;
		}
		set {
			skill = value;
		}
	}
}
