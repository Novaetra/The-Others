using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class basically checks if you have the required items to unlock whatever it is that you have
public class Unlockable : MonoBehaviour
{
    [SerializeField]
	protected Item[] itemsRequired;

    protected bool isUnlocked;
    protected Inventory inv;
	protected HUDManager hudMan;

    void Start()
    {
        inv = GameObject.Find("InventoryManager").GetComponent<Inventory>();
		hudMan = GameObject.Find("Player").GetComponent<HUDManager>();
		setUpItemsRequired();
    }

	//Add items required here
	protected virtual void setUpItemsRequired() { }

	public void interact(object[] parameters)
    {
        GameObject player = (GameObject)parameters[1];
	    if (CheckIfPlayerHasAllItems()==true)
		{
			Unlock();
			SubtractItems();
			DiscontinueItems();
		}
		else
		{
			CouldNotUnlock();	
		}
    }

	protected virtual void CouldNotUnlock()
	{

	}

    public void SubtractItems()
    {
        foreach(Item i in itemsRequired)
        {
            int indx = GetIndxOfItem(i);
            inv.SubtractItemToInventory(indx,1);
        }
    }

    public void DiscontinueItems()
    {
        foreach (Item i in itemsRequired)
        {
            inv.InventoryList[i.ItemIDNumber].IsDiscontinued = true;
        }
        inv.UpdateItemsAvailableToSpawn();
    }

    private int GetIndxOfItem(Item i)
    {
        foreach (Item item in inv.InventoryList)
        {
            if (i.ItemIDNumber == i.ItemIDNumber)
            {
                return i.ItemIDNumber;
            }
        }
        return -1;
    }

    private bool CheckIfPlayerHasAllItems()
    {
		Item lastCheckedItem = null;
		int amtToCheck = 1;
        foreach (Item itemRequired in itemsRequired)
        {
			try
			{
				if (lastCheckedItem!=null && lastCheckedItem.ItemIDNumber == itemRequired.ItemIDNumber)
				{
					amtToCheck++;
				}
			}
			catch (NullReferenceException e) { Debug.Log("null was thrown");}
			lastCheckedItem = itemRequired;

            foreach (Item i in inv.InventoryList)
            {
                if (itemRequired.ItemIDNumber == i.ItemIDNumber)
                {
					if (i.Amt < amtToCheck)
                    {
						return false;
                    }
                }
            }

        }
		return true;
    }

	protected virtual void Unlock()
    {
        isUnlocked = true;
    }

    public bool IsUnlocked
    {
        get
        {
            return isUnlocked;
        }
    }

}
