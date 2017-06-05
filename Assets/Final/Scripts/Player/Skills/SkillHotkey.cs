using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHotkey 
{
	private KeyCode code;
	private Skill skill;
    private bool isHoldable;

	public SkillHotkey(KeyCode c, Skill s)
	{
		code = c;
		skill = s;
        IsHoldable = s.IsHoldable;
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

    public bool IsHoldable
    {
        get
        {
            return isHoldable;
        }

        set
        {
            isHoldable = value;
        }
    }
}
