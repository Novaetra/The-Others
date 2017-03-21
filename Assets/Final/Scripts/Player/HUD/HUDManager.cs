using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour 
{
	public Image healthbar;
	public Image manabar;
	public Image staminabar;
	public Image expbar;
    public Image revivingBar;
    public Image revivingBarBG;

	private StatsManager sm;
	private PlayerController pc;

	private GameObject currentPlayer;
	private GameObject canvasObj;
	private GameObject currentPanel;
	private GameObject tooltip;
	private List<GameObject> panels;

	private Text messageObj;
    private Text revivingTxt;
    private Text upgradePnts;
	private Text expText;
    private Text roundsTxt;
	private Text tooltipName;
	private Text tooltipDesc;
	private Text tooltipLvl;
	private Text currentLvlTxt;

	private bool setUpDone = false;

	private int currentPanelIndex = 1;

	private List<Message> queuedMessages;

	bool messageDisplaying = false;

	private float messageTimer, messageCurrentTime;

	//Assigns all the variables to their correspinding values
	void Start()
	{
		SetStartingValues ();
	}

	private void SetStartingValues()
	{
		currentPlayer = gameObject;
		pc = currentPlayer.GetComponent<PlayerController>();
		sm = currentPlayer.GetComponent<StatsManager> ();
		canvasObj = GameObject.Find("Canvas");
		currentPanel = GameObject.Find ("SkillTree Panel").gameObject;
		roundsTxt = canvasObj.transform.FindChild("RoundsTxt").GetComponent<Text>();
		roundsTxt.enabled = false;
		messageObj = canvasObj.transform.FindChild ("ShortMessage").GetComponent<Text>();
		upgradePnts = currentPanel.transform.FindChild("UpgradePoints").GetComponent<Text>();
		currentLvlTxt = currentPanel.transform.FindChild ("CurrentLevelText").GetComponent<Text> ();
		revivingBarBG = canvasObj.transform.FindChild("ReviveBarBG").GetComponent<Image>();
		revivingTxt = revivingBarBG.transform.FindChild("Text").GetComponent<Text>();
		revivingBar = revivingBarBG.transform.FindChild("Image").GetComponent<Image>();
		tooltip = canvasObj.transform.FindChild ("Tooltip").gameObject;
		tooltipName = tooltip.transform.FindChild ("Name").GetComponent<Text> ();;
		tooltipDesc = tooltip.transform.FindChild ("Description").GetComponent<Text> ();
		tooltipLvl = tooltip.transform.FindChild ("LvlRequired").GetComponent<Text> ();
		expText = canvasObj.transform.FindChild ("Exp Counter").GetComponent<Text> ();
		SetUpPanels ();
		tooltip.SetActive (false);
		upgradePnts.enabled = false;
		setUpDone = true;
		messageCurrentTime = 0;
		messageTimer = 0;
		queuedMessages = new List<Message> ();
	}

	void Update()
	{
		updateBars ();
		UpdateMessageTimer ();
	}
    
	private void SetUpPanels()
	{
		panels = new List<GameObject> ();
		GameObject pnls = GameObject.Find("Panels");
		foreach (Transform t in pnls.transform) 
		{
			panels.Add (t.gameObject);
		}
		HideAllPanels ();
	}

    private void HideOtherPanels()
    {
		foreach(GameObject go in panels)
        {
			if(currentPanel!=go)
            {
				go.SetActive(false);
            }
            else
            {
				go.SetActive(true);
            }
        }
    
    }

	private void HideAllPanels()
	{
		foreach(GameObject go in panels)
		{
			go.SetActive(false);
		}
	}
    
    public void GoToNextPanel()
    {
		if (currentPanelIndex + 1 > panels.Count - 1) 
		{
			currentPanelIndex = 0;
		} 
		else
		{
			currentPanelIndex++;
		}
		currentPanel = panels[currentPanelIndex];
        HideOtherPanels();
    }
    
	//Increments message's timer and hides message when message's current time = message's timer/max time
	//If there is a message queued, it will play the next one too
	private void UpdateMessageTimer()
	{
		if (messageCurrentTime < messageTimer) 
		{
			messageCurrentTime += Time.deltaTime;
		} else 
		{
			messageDisplaying = false;
			PlayNextMessage ();
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
			manabar.fillAmount = sm.getCurrentMana () / sm.getTotalMana ();
			staminabar.fillAmount = sm.getCurrentStamina () / sm.getTotalStamina ();
			expbar.fillAmount = sm.getCurrentExp () / sm.getGoalExp ();
			expText.text = (int)sm.getCurrentExp () + " / " + (int)sm.getGoalExp ();
            
            /*
            if(pc.getReviving() == true)
            {
                StatsManager personReviving = pc.getPersonReviving().GetComponent<StatsManager>();
                if (personReviving.getCurrentReviveTimer() / personReviving.getTotalReviveTimer() < 1)
                {
                    revivingBarBG.enabled = true;
                    revivingTxt.enabled = true;
                    revivingBar.enabled = true;
                    revivingBar.fillAmount = personReviving.getCurrentReviveTimer() / personReviving.getTotalReviveTimer();
                }
                else
                {

                    revivingBarBG.enabled = false;
                    revivingTxt.enabled = false;
                    revivingBar.enabled = false;
                }
               
            }
            else
            {
                revivingBarBG.enabled = false;
                revivingTxt.enabled = false;
                revivingBar.enabled = false;
            }
            */

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
	
	//Adds a message to the queue
	private void QueueMessage(string msg, float dur)
	{
		queuedMessages.Add(new Message (msg, dur));
	}

	//Plays next message in queue and deletes it from queue
	private void PlayNextMessage()
	{
		if (queuedMessages.Count > 0) 
		{
			Message msg = queuedMessages [0];
			displayMsg (msg._Message, msg.Duration);
			queuedMessages.Remove (msg);
		} 
		else 
		{
			hideMessage ();
		}
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

	//Fills in tooltip information and adds it to screen
	public void ShowSkillTooltip(string name, string desc, string lvlRequired)
	{
		tooltipName.text = name;
		tooltipDesc.text = desc;
		tooltipLvl.text = "Level: " + lvlRequired;
		tooltip.SetActive(true);
	}

	//Hides tooltip
	public void HideTooltip()
	{
		//Hides tooltip 
		tooltip.SetActive(false);
	}

	public void ShowItemTooltip(string name, string desc, bool equipable)
	{
		tooltipName.text = name;
		tooltipDesc.text = desc;
		tooltipLvl.text = "";
		tooltip.SetActive(true);
	}

	//Shows currentPanel
	public void showPanels()
	{
		HideOtherPanels ();
	}

	//Hides currentPanel
	public void hidePanels()
	{
		HideAllPanels ();
	}

	//returns refrence to the currentPanel
	public GameObject getcurrentPanel()
	{
		return currentPanel;
	}
}
