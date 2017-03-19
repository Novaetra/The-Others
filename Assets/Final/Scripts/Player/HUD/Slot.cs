using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
	private Transform skillBar;
	private Transform bg;
	private SkillManager manager;
	private DraggableSlot dragSlot;
	private GameObject currentPlayer;
	[SerializeField]
	private KeyCode key;

	public void Start()
	{
        currentPlayer = GameObject.Find("Player");
		//Gets the skill bar for iterating through it later
		skillBar = GameObject.Find("Canvas").transform.Find("SkillBar").transform;
		//Get the skill manager
		manager = currentPlayer.GetComponent<SkillManager>();
		//Gets the background object so that we can change it to gray or white
		bg = transform.parent;
		bg = bg.FindChild ("SlotBG");
		bg.GetComponent<Image> ().color = Color.gray;
		//Make the slot undraggable if empty
		dragSlot = GetComponent<DraggableSlot> ();
		dragSlot.isDraggable = false;
	}
		
	//Scans the bar for duplicates when a skill is dragged onto this slot
	public void OnDrop(PointerEventData data)
	{
		//Check to see if the skill is placed somewhere else before assigning it to the slot
		scanSkillBar ();
	}

	//Scans skill bar to see if the skill being dragged is already there
	private void scanSkillBar()
	{
		if (DraggableItem.skillBeingDragged != null) 
		{
			//For each slot in the bar
			foreach (Transform child in skillBar) 
			{
				Transform content = child.GetChild (1);
				//If the skill being dragged from the tree is already assigned somewhere else, then clear
				//The other instance of it and assign the new one 
				if (content.GetComponent<SkillTreePiece> ().getSkill () != null) 
				{
					if (content.GetComponent<SkillTreePiece> ().getSkill () == DraggableItem.skillBeingDragged) 
					{
						ErasePreviousSkillInstance (content, child);
						assignSkillToSlot ();
						return;
					}
				}
			}
		}
		//This is called when no one instance is found
		assignSkillToSlot ();
	}

	//Removes other instances in the skill bar so there are no duplicates
	private void ErasePreviousSkillInstance(Transform content, Transform child)
	{
		bg.GetComponent<Image> ().sprite = null;
		bg.GetComponent<Image> ().color = Color.white;
		content.GetComponent<DraggableSlot> ().isDraggable = false;
		content.GetComponent<SkillTreePiece> ().getSkill ().SlotAssignedTo = (null);
		content.GetComponent<SkillTreePiece> ().getSkill ().Hotkey = (null);
		content.GetComponent<SkillTreePiece> ().setSkill (null);
		content.GetComponent<Image> ().sprite = child.GetComponent<Image> ().sprite;
		manager.removeFromKnown (content.GetComponent<SkillTreePiece> ().getSkill ());
	}

	private void assignSkillToSlot()
	{
		//This just checks to see if the thing being dragged is from the skill tree or another slot
		if (DraggableItem.skillBeingDragged != null) 
		{
			//Assigns the image and skill to the slot
			GetComponent<SkillTreePiece> ().setSkill (DraggableItem.skillBeingDragged);
			GetComponent<SkillTreePiece> ().getSkill ().Hotkey = new Hotkey (key, GetComponent<SkillTreePiece> ().getSkill());
			GetComponent<Image> ().sprite = DraggableItem.imgBeingDragged;
			bg.GetComponent<Image>().sprite = DraggableItem.imgBeingDragged;
		} 
		//If its being dragged from another slot
		else 
		{
			SwapSlots ();
		}
        
		//Adds the skill to the list of known skills
		manager.addToKnown (GetComponent<SkillTreePiece> ().getSkill ());
		//Assign the skill to the slot
		GetComponent<SkillTreePiece> ().getSkill ().SlotAssignedTo = (gameObject);
		//Makes the slot draggable
		dragSlot.isDraggable = true;
		//Changes background color to gray
		bg.GetComponent<Image> ().color = Color.gray;
		//Debug.Log (manager.knownSkills[0].Hotkey);
	}

	private void SwapSlots()
	{
		//Swap the two skills
		Skill dragged = DraggableSlot.skillBeingDragged;
		DraggableSlot.foundTarget = true;
		if (GetComponent<SkillTreePiece>().getSkill()!=null)
		{
			Skill thisSlotsSkills = GetComponent<SkillTreePiece>().getSkill();
			if (dragged != null && thisSlotsSkills.Cooldown >= thisSlotsSkills.Cooldown)
			{
				SkillTreePiece skillTreePieceOfDragged = DraggableSlot.originalSlot.GetComponent<SkillTreePiece> ();
				AssignDraggedToMine (skillTreePieceOfDragged,thisSlotsSkills);
				AssignMineToDragged (skillTreePieceOfDragged,thisSlotsSkills);
			}
		}
	}

	//Assigns their hotkey to my skill that's being swapped
	private void AssignDraggedToMine(SkillTreePiece skillTreePieceOfDragged,Skill thisSlotsSkills)
	{
		skillTreePieceOfDragged.setSkill(thisSlotsSkills);
		skillTreePieceOfDragged.getSkill().SlotAssignedTo = (gameObject);
		skillTreePieceOfDragged.getSkill ().Hotkey = new Hotkey (DraggableSlot.originalSlot.GetComponent<Slot>().key, thisSlotsSkills);
		DraggableSlot.originalSlot.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
		DraggableSlot.originalParent.GetChild(0).GetComponent<Image>().sprite = GetComponent<Image>().sprite;
	}

	//Assigns my hotkey to the skill being dragged over
	private void AssignMineToDragged(SkillTreePiece skillTreePieceOfDragged,Skill thisSlotsSkills)
	{
		Skill dragged = DraggableSlot.skillBeingDragged;
		Sprite img = DraggableSlot.imgBeingDragged;
		thisSlotsSkills.SlotAssignedTo = (DraggableSlot.originalSlot.gameObject);
		GetComponent<Image>().sprite = img;
		bg.GetComponent<Image>().sprite = img;
		GetComponent<SkillTreePiece>().setSkill(dragged);
		GetComponent<SkillTreePiece>().getSkill().Hotkey = new Hotkey(key,dragged);
	}
}
