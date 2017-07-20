using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPanelHotkey : ControlHotkey 
{
	private GameObject currentPanel;
	private int currentPanelIndex = 1;
	
	public DisplayPanelHotkey(KeyCode[] c,PlayerController cntrl)
	{
		codes = c; 
		controller = cntrl;
	}
		
	public override void CheckKeys()
	{
		foreach (KeyCode c in Codes) 
		{
			if (Input.GetKeyUp (c)) 
			{
				if (c == codes [0]) 
				{
					HideShowPanels ();
				}
				else if (c == codes [1]) 
				{
					GoToNextPanel ();
				}
			}
		}
	}

	private void HideShowPanels()
	{
		controller.toggleCursorLock (!controller.cursorLocked);
		if (controller.cursorLocked == false) {
			controller.Hudman.showPanels ();
		} else {

			controller.Hudman.hidePanels ();
			controller.Hudman.HideTooltip ();
		}
	}

	private void GoToNextPanel()
	{
		if (currentPanelIndex + 1 > controller.Hudman.Panels.Count - 1) 
		{
			currentPanelIndex = 0;
		} 
		else
		{
			currentPanelIndex++;
		}
		currentPanel = controller.Hudman.Panels[currentPanelIndex];
		currentPanel.transform.SetAsLastSibling ();
	}
}
