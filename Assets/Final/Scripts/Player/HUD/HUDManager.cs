using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class HUDManager : MonoBehaviour 
{
	private float tooltipOffset = 20;
    [SerializeField]
	private Image healthbar,manabar, staminabar, expbar;
    [SerializeField]
    private Text healthText, staminaText, manaText;

    private StatsManager sm;
	private PlayerController pc;

    private Animator damageIndicator;

	private GameObject currentPlayer;
	private GameObject canvasObj;
	private GameObject tooltip, tooltipBG;
	private List<GameObject> panels;
	private Transform panelsOBJ;

	private Text messageObj;
    private Text revivingTxt;
    private Text upgradePnts;
	private Text expText;
    private Text roundsTxt;
	private Text tooltipName, tooltipLvl, tooltipCost, tooltipCooldown,tooltipDesc,tooltipNextUpgrade, tooltipStaticNextLevel;
	private Text currentLvlTxt;


	private bool setUpDone = false;
	[SerializeField]
	private bool isOnTooltip = false;

	private List<Message> queuedMessages;

	bool messageDisplaying = false;

	private float messageTimer, messageCurrentTime;

	//Assigns all the variables to their correspinding values
	void Start()
	{
	    if(!setUpDone)
		    SetStartingValues ();
	}

	private void SetStartingValues()
	{
	    canvasObj = GameObject.Find("Canvas");
	    messageObj = canvasObj.transform.Find ("ShortMessage").GetComponent<Text>();
		currentPlayer = gameObject;
		pc = currentPlayer.GetComponent<PlayerController>();
		sm = currentPlayer.GetComponent<StatsManager> ();
		roundsTxt = canvasObj.transform.Find("RoundsTxt").GetComponent<Text>();
		roundsTxt.enabled = false;
		upgradePnts = GameObject.Find ("SkillTree Panel").transform.Find("UpgradePoints").GetComponent<Text>();
		currentLvlTxt = GameObject.Find ("SkillTree Panel").transform.Find ("CurrentLevelText").GetComponent<Text> ();
		tooltip = canvasObj.transform.Find ("Tooltip").gameObject;
		tooltipStaticNextLevel = GameObject.Find("NextLvl").GetComponent<Text>();
		tooltipBG = tooltip.transform.Find("Border").gameObject;
		tooltipName = tooltip.transform.Find ("Name").GetComponent<Text> ();
		tooltipLvl = tooltip.transform.Find("LvlRequired").GetComponent<Text>();
		tooltipCost = tooltip.transform.Find("Cost").GetComponent<Text>();
		tooltipCooldown = tooltip.transform.Find("Cooldown").GetComponent<Text>();
		tooltipDesc = tooltip.transform.Find ("Description").GetComponent<Text> ();
		tooltipNextUpgrade = tooltip.transform.Find ("Upgrades").GetComponent<Text> ();
		expText = canvasObj.transform.Find ("Exp Counter").GetComponent<Text> ();
	    damageIndicator = canvasObj.transform.Find("Damage Indicator").GetComponent<Animator>();
		SetUpPanels ();
		tooltip.SetActive (false);
		upgradePnts.enabled = false;
		messageCurrentTime = 0;
		messageTimer = 0;
		queuedMessages = new List<Message> ();
		HidePanels ();
	    updateCurrentLvlTxt();
	    setUpDone = true;
	}

	void Update()
	{
		updateBars ();
		UpdateMessageTimer ();
	}
    
	private void SetUpPanels()
	{
		panels = new List<GameObject> ();
		panelsOBJ = GameObject.Find("Panels").transform;
		GameObject pnls = GameObject.Find("Panels");
		foreach (Transform t in pnls.transform) 
		{
			panels.Add (t.gameObject);
		}
	}

	//Increments message's timer and hides message when message's current time = message's timer/max time
	//If there is a message queued, it will play the next one too
	private void UpdateMessageTimer()
	{
		if (messageCurrentTime < messageTimer) 
		{
			messageCurrentTime += Time.deltaTime;
		} else if (queuedMessages.Count > 0)
		{
		    messageDisplaying = false;
		    PlayNextMessage();
		}
		else
		{
		    hideMessage();
		}
	}

	//Updates the string that display's player's current level
	public void updateCurrentLvlTxt()
	{
	    if(currentLvlTxt != null && sm.getCurrentLvl() != null)
        {
            currentLvlTxt.text = "Level " + sm.getCurrentLvl();
        }
	}

	//Updates player's status bars
	public void updateBars()
	{
        if (sm != null) 
		{
			healthbar.fillAmount = sm.getCurrentHealth () / sm.getTotalHealth ();
            healthText.text = (int)sm.getCurrentHealth() + "/" + (int)sm.getTotalHealth();
            manabar.fillAmount = sm.getCurrentMana () / sm.getTotalMana ();
            manaText.text = (int)sm.getCurrentMana() + "/" + (int)sm.getTotalMana();
            staminabar.fillAmount = sm.getCurrentStamina () / sm.getTotalStamina ();
            staminaText.text = (int)sm.getCurrentStamina() + "/" + (int)sm.getTotalStamina();
            expbar.fillAmount = sm.getCurrentExp () / sm.getGoalExp ();
			expText.text = (int)sm.getCurrentExp () + " / " + (int)sm.getGoalExp ();
		}
	}

	//Updates the string that displays player's upgrade points
    public void updateUpgradePoints()
    {
        if(sm.getUpgradePnts() > 0)
        {
            upgradePnts.text = "Upgrade points: " + sm.getUpgradePnts();
            upgradePnts.enabled = true;
        }
        else
        {
            upgradePnts.enabled = false;
        }
    }

	//Displays message on screeen
	public void displayMsg(string msg, float dur)
	{
	    if (setUpDone)
	    {
	        //If a message is already displaying, but the next message's text is the same, just reset the timer
	        //Otherwise queue the message to display after
	        if (messageDisplaying)
	        {
	            if (messageObj.text.Equals (msg))
	            {
	                messageCurrentTime = 0f;
	            } else
	            {
	                QueueMessage (msg, dur);
	            }
	        }
	        //If a message is not displaying, display one
	        else
	        {
	            messageObj.text = msg;
	            messageObj.enabled = true;
	            messageTimer = dur;
	            messageCurrentTime = 0;
	            messageDisplaying = true;
	        }
	    }
	    else
	    {
	        SetStartingValues();
	        displayMsg(msg,dur);
	    }
	}
	
	//Adds a message to the queue
	private void QueueMessage(string msg, float dur)
	{
		queuedMessages.Add(new Message (msg, dur));
	}

	//Plays next message in queue and deletes it from queue
	private void PlayNextMessage()
	{
        Message msg = queuedMessages [0];
        displayMsg (msg._Message, msg.Duration);
        queuedMessages.Remove (msg);
	}

	//hides message
	private void hideMessage()
	{
		messageObj.text = null;
		messageObj.enabled = false;;
	}

	//Updates the text displaying the current round number
    public void updateRoundsTxt(int round)
    {
        roundsTxt.enabled = true;
        roundsTxt.text = "Round " + round;
    }

    public void RecieveDamage()
    {
        damageIndicator.SetTrigger("recievedDamage");
    }

	//Fills in tooltip information and adds it to screen
	public void ShowSkillTooltip(Skill skill,PointerEventData data,GameObject go)
	{
		UpdateToolTip(skill);
		RepositionTooltip (data,go);
		tooltip.SetActive(true);
	}

    public void UpdateToolTip(Skill skill)
    {
        tooltipName.text = skill.Name;
		tooltipLvl.text = "Level: " + skill.LvlRequirement;
		tooltipCost.text = skill.Cost + " " + skill.SkillType;
		tooltipCooldown.text = skill.Cooldown + " second cooldown";
		tooltipDesc.text = skill.Description;

		if (skill.NextUpgrade != null)
		{
			AddNextUpgradeToTooltip(skill.NextUpgrade, skill);
			ShowUpgradePartOfTooltip();
		}
		else {
			HideUpgradePartOfTooltip();
		}
    }

	private void AddNextUpgradeToTooltip(Upgrade up, Skill skill)
	{
		tooltipNextUpgrade.text = up.AttributeToUpgrade + ": " + skill.GetAttribute(up.AttributeToUpgrade) + " => " + (skill.GetAttribute(up.AttributeToUpgrade)+up.UpgradeAmt);
	}

	private void ShowUpgradePartOfTooltip()
	{
		tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 150);
		tooltipBG.GetComponent<RectTransform>().sizeDelta = new Vector2(240, 140);
		tooltipNextUpgrade.enabled = true;
		tooltipStaticNextLevel.enabled = true;
	}


	private void HideUpgradePartOfTooltip()
	{
		tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 100);
		tooltipBG.GetComponent<RectTransform>().sizeDelta = new Vector2(240, 90);
		tooltipNextUpgrade.enabled = false;
		tooltipStaticNextLevel.enabled = false;
	}

    private void RepositionTooltip(PointerEventData data,GameObject go)
	{
		Transform originalParent = tooltip.transform.parent;
		RectTransform rect = tooltip.GetComponent<RectTransform> ();
		tooltip.transform.parent = go.transform;
		tooltip.transform.localPosition = new Vector2 (0+tooltipOffset, 0);
		tooltip.transform.parent = originalParent;

		if ((rect.anchoredPosition.x+(rect.rect.width)) > Screen.currentResolution.width) 
		{
			tooltip.transform.parent = go.transform;
			tooltip.transform.localPosition = new Vector2 (-tooltipOffset - rect.rect.width * 2, 0f);
			tooltip.transform.parent = originalParent;
		}

		if ((rect.anchoredPosition.y-(rect.rect.height)) < -Screen.currentResolution.height) 
		{
			tooltip.transform.localPosition = new Vector2 (tooltip.transform.localPosition.x,  tooltip.transform.transform.localPosition.y + rect.rect.height*2);//12.5f*(rect.rect.height*2));
		}
	}

	//Hides tooltip
	public void HideTooltip()
	{
		//Hides tooltip 
		tooltip.SetActive(false);
	}

	public void ShowItemTooltip(Item i, PointerEventData data,GameObject go)
	{
		tooltipName.text = i.Name;
		tooltipDesc.text = i.Description;
		RepositionTooltip (data,go);
		tooltipNextUpgrade.enabled = false;
		tooltipLvl.enabled = false;
		tooltip.SetActive(true);
	}

	//Shows currentPanel
	public void showPanels()
	{
		panelsOBJ.gameObject.SetActive (true);
	}

	//Hides currentPanel
	public void HidePanels()
	{
		panelsOBJ.gameObject.SetActive (false);
	}

	public List<GameObject> Panels {
		get {
			return panels;
		}
	}

	public Transform PanelsOBJ {
		get {
			return panelsOBJ;
		}
	}

	public bool IsOnTooltip {
		get {
			return isOnTooltip;
		}
		set {
			if (value == false) 
			{
				HideTooltip ();
			}
			isOnTooltip = value;
		}
	}
}
