using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour 
{
	public List<SkillTreePiece> treePieces;
	private List<Toggle> toggles;

	private SkillTreePiece piece;
	private StatsManager _sm;
	private int skillsUnlocked;

	//Its called as soon as program is started, and just sets the starting values
    public void Awake()
    {
        fillTreePiecesList();
        skillsUnlocked = 0;
    }
    
	//Populates the 'treepieces' list to refrence all of the skills within the skill tree
	private void fillTreePiecesList()
	{
        toggles = new List<Toggle>();
		treePieces = new List<SkillTreePiece>();
		foreach (Transform child in transform.GetChild(0).transform) 
		{
			if (child.gameObject.GetComponentInChildren<SkillTreePiece> () != null) 
			{
				piece = child.gameObject.GetComponentInChildren<SkillTreePiece> ();
                toggles.Add(child.GetComponent<Toggle>());
				child.gameObject.GetComponent<Toggle>().interactable = false;
				treePieces.Add (piece);
			}
		}
	}

	//This is called every time player levels up
	//It shows player all the upgrades/unlocks available
    public void DisplayUnlockables(StatsManager sm)
    {
		_sm = sm;
        //try
        //{
            int x = 0;
			//Skills with the level requirement met by the player will light up and allow player to unlock/upgrade them if player has unlocked all previous skills
            foreach (SkillTreePiece piece in treePieces)
			{
				if (piece.getUnlocked() == false && piece.getSkill().LvlRequirement <= sm.getCurrentLvl() && sm.getUpgradePnts() > 0 && x <= skillsUnlocked)
				{
					toggles[x].interactable = true;
                }
				else if(canUpgradeSkill(piece.getSkill()))
				{
					toggles[x].interactable = true;
					toggles[x].isOn = false;
				}
				else if(piece.getUnlocked() && !canUpgradeSkill(piece.getSkill()))
				{
					toggles[x].isOn = true;
					toggles[x].interactable = false;
				}
				else
				{
					toggles[x].interactable = false;
				}	

                x++;
            }
		/*}
		
        catch(System.NullReferenceException e)
        {
			Debug.Log ("OH OH: "); 
        }
        */
    }

	public bool canUpgradeSkill(Skill s)
	{
		if (s.Upgrades == null) 
		{
			return false;
		}
		if (s.UpgradeCount < s.Upgrades.Count) 
		{
			if(s.Upgrades [s.UpgradeCount].LvlRequirement <= _sm.getCurrentLvl ()&&_sm.getUpgradePnts()>0)
			{
				return true;
			}
		}

		return false;
	}

	public void unlockSkill(StatsManager sm)
	{
		skillsUnlocked++;
		DisplayUnlockables (sm);
	}

}
