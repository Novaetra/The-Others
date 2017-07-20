using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSkillTreePiece : MonoBehaviour 
{
	private SkillTreePiece parentSkillTreePiece;
	private PlayerController currentPlayer;

	private StatsManager sm;
	void Start () 
	{
		StartCoroutine(setParent());
	}

	private IEnumerator setParent()
	{
		yield return new WaitForSeconds(.5f);
		parentSkillTreePiece = GetComponentInParent<SkillTreePiece>();
		currentPlayer = GameObject.Find("Player").GetComponent<PlayerController>();
		sm = GameObject.Find("Player").GetComponent<StatsManager>();
	}

	public void onClick()
	{
		if (parentSkillTreePiece.getUnlocked() && sm.getUpgradePnts() > 0)
		{
			Debug.Log("unlock");
			parentSkillTreePiece.upgradeSkill();
		}
	}


	public void OnPointerEnter(PointerEventData data)
	{
		parentSkillTreePiece.showTooltip(data, gameObject);
	}

	public void OnPointerExit(PointerEventData data)
	{
		StartCoroutine(StartHide());
	}
	IEnumerator StartHide()
	{
		yield return new WaitForSeconds(.1f);
		if (currentPlayer.GetComponent<HUDManager>().IsOnTooltip == false)
		{
			GetComponent<SkillTreePiece>().hideTooltip();
		}
	}
}
