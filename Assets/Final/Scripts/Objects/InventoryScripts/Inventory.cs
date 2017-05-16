using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour 
{
	[SerializeField]
	private List<Item> availableItemsList;
    private List<Item> inventory;

    private InventoryUI ui;
	private HUDManager hudman;

	void Awake()
	{
		SetUpAllItemsList ();
		ui = GetComponent<InventoryUI> ();
		hudman = GameObject.Find ("Player").GetComponent<HUDManager> ();
	}

	private void SetUpAllItemsList()
	{
		availableItemsList = new List<Item> ();
        inventory = new List<Item>();
        inventory.Add (new Item ("Skill Charge","Strong energy used to cast powerful spells.",0,40,5));
        inventory.Add(new Item("Key", "Mysterious looking key...", 1, 50,2));
        SortInventoryListByDropRate();
        PopulateAvailableItemsList();
    }

    //This puts all items available to spawn based on location and events so that the correct items can spawn in the correct places
    private void PopulateAvailableItemsList()
    {
        //Temporary
        //Adds all items from master list to available
        foreach (Item i in inventory)
        {
            availableItemsList.Add(i);
        }
    }

    //Removes any item that has reach
    private void CheckIfItemMaxReached(Item i)
    {
        //Temporary
        if (inventory[i.ItemIDNumber].Max > 0 && inventory[i.ItemIDNumber].Amt >= inventory[i.ItemIDNumber].Max)
        {
            availableItemsList.RemoveAt(i.ItemIDNumber);
        }
    }

    //Sorts inventory list by rarity
    private void SortInventoryListByDropRate()
    {
        for (int x = 0; x < inventory.Count-1; x++)
        {
            Item one = inventory[x];
            for (int y = x+1; y < inventory.Count; y++)
            {
                Item other = inventory[y];
                if (other.DropRate < one.DropRate)
                {
                    //Swap
                    inventory[x] = other;
                    inventory[y] = one;
                }
            }
        }
    }

	public void AddItemToInventory(Item i)
	{
        inventory[i.ItemIDNumber].Amt++;
		//UPDATE GUI HERE
		ui.UpdateInventoryUI();
		hudman.displayMsg ("You picked up a " + inventory[i.ItemIDNumber].Name + "!",2.5f);

        CheckIfItemMaxReached(i);
    }

	public void UpdateInventoryUI()
	{
		ui.UpdateInventoryUI ();
	}

	public void RemoveItemFromInventory(Item i)
	{
		int amt = availableItemsList[i.ItemIDNumber].Amt;
		if (amt > 1 && Input.GetKey (KeyCode.LeftShift) || amt == 1) 
		{
			amt = 0;
		} 
		else if (amt > 1) 
		{
			amt--;
		}
		availableItemsList [i.ItemIDNumber].Amt = amt;
		GetComponent<InventoryUI>().UpdateInventoryUI();
	}

	public List<Item> _inventory {
		get {
			return availableItemsList;
		}
	}
}
