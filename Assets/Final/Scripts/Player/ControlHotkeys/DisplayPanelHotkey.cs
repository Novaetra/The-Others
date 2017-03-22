using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPanelHotkey : ControlHotkey 
{

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

			controller.Hudman.HidePanels ();
			controller.Hudman.HideTooltip ();
		}
	}

	private void GoToNextPanel()
	{
		controller.toggleCursorLock (!controller.cursorLocked);
		if (controller.cursorLocked == false) {
			controller.Hudman.showPanels ();
		} else {

			controller.Hudman.HidePanels ();
			controller.Hudman.HideTooltip ();
		}
	}
}
