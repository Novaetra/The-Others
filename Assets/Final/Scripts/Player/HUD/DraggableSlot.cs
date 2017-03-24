using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{

	public static Skill skillBeingDragged;
	public static Sprite imgBeingDragged;
	public static Transform originalParent;
	public static Transform originalSlot;
	public bool isDraggable;
	public static bool foundTarget;

	private static float planeDistance;

	private Vector3 startPos;
	private Transform background;
	private static GameObject currentPlayer;
	private HUDManager hudman;
    private Skill skill;

	public void Start()
	{
		currentPlayer = GameObject.Find ("Player");
		hudman = currentPlayer.GetComponent<HUDManager> ();
        planeDistance = GameObject.Find("Canvas").GetComponent<Canvas>().planeDistance;
    }

	public void OnBeginDrag(PointerEventData data)
	{
		if (isDraggable) 
		{
			//Check if it has a skill
			if (GetComponent<SkillTreePiece> ().getSkill () != null) 
			{
                skill = GetComponent<SkillTreePiece>().getSkill();
				if (skill.CurrentCooldown >= skill.Cooldown)
                {
					SetStartingInfo ();
                }
			}
		}
	}

	private void SetStartingInfo()
	{
		foundTarget = false;
		startPos = transform.localPosition;
		//Sets the object being dragged
		skillBeingDragged = transform.GetComponent<SkillTreePiece>().getSkill();
		//Sets the image of the object being dragged
		imgBeingDragged = transform.GetComponent<Image>().sprite;
		//Sets the background image of the object being dragged
		background = transform.parent;
		background = background.FindChild("SlotBG").transform;
		//Sets the original parent so it can go back to it later
		originalParent = gameObject.transform.parent.transform;
		originalSlot = originalParent.GetChild(1);
		//Changes the parent to Canvas so it can be dragged outside the skill tree mask
		gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
		gameObject.transform.SetAsLastSibling();
		//Allows the event system to pass through the object being dragged
		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	//Update position of image as you drag it
	public void OnDrag(PointerEventData data)
	{
		//Update position of object
		if (isDraggable && skill.CurrentCooldown >= skill.Cooldown) 
		{
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(data.position.x,data.position.y,planeDistance));
		}
	}

	public void OnEndDrag(PointerEventData data)
	{
		if (isDraggable && skill.CurrentCooldown >= skill.Cooldown)
		{
			ResetEverything ();
		}
	}

	private void ResetEverything()
	{
		//Reset raycast block
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		//Reset parent to its original
		gameObject.transform.SetParent (originalParent);
		//Reset object to start position
		transform.localPosition = startPos;

		//if it doesn't find a target, then clear the slot
		if(foundTarget == false)
		{
			GetComponent<SkillTreePiece> ().setSkill (null);
			background.GetComponent<Image> ().sprite = originalParent.GetComponent<Image> ().sprite;
			GetComponent<Image> ().sprite = originalParent.GetComponent<Image> ().sprite;
		}

		//Reset all attributes to null
		skillBeingDragged = null;
		imgBeingDragged = null;
		originalSlot = null;
		originalParent = null;
	}

	public void OnPointerEnter(PointerEventData data)
	{
		skill = GetComponent<SkillTreePiece>().getSkill();
		if (skill.Name != "") 
		{
			hudman.ShowSkillTooltip (skill, data,gameObject);
		}	
	}

	public void OnPointerExit(PointerEventData data)
	{
		StartCoroutine (StartHide());
	}

	IEnumerator StartHide()
	{
		yield return new WaitForSeconds(.1f);
		skill = GetComponent<SkillTreePiece>().getSkill();
		if (skill.Name != "" && hudman.IsOnTooltip == false) 
		{
			hudman.HideTooltip ();
		}
	}

}
