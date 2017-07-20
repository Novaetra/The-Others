using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour 
{
	[SerializeField]
	private List<Item> availableItemsList; //LIst of possible items that can spawn
	[SerializeField]
	private List<Item> inventory; // actual inventoyry of player

    private InventoryUI ui;
	private HUDManager hudman;
    private EnemyManager em;

	void Awake()
	{
		ui = GetComponent<InventoryUI>();
		hudman = GameObject.Find("Player").GetComponent<HUDManager>();
		em = GameObject.Find("Managers").GetComponent<EnemyManager>();
		SetUpAllItemsList();
    }

	void Start()
	{
		//temp
		//AddTestData();
		//temp
	}

	private void SetUpAllItemsList()
	{
		availableItemsList = new List<Item> ();
        inventory = new List<Item>();
        inventory.Add (new Item ("Skill Charge","Strong energy used to cast powerful spells.",0,10,4));
        inventory.Add(new Item("Key", "Mysterious looking key...", 1, 10,2));
        inventory[1].AddFloor(GameObject.Find("Dungeon").transform.GetChild(0).name);
        SortInventoryListByDropRate();
	}

	private void AddTestData()
	{
		AddItemToInventory(1);
		AddItemToInventory(1);
		AddItemToInventory(0);
		AddItemToInventory(0);
		AddItemToInventory(0);
		hudman.displayMsg("Started with keys", 2);
	}

    //This puts all items available to spawn based on location and events so that the correct items can spawn in the correct places
    public void UpdateItemsAvailableToSpawn()
    {
        availableItemsList.Clear();
        //Temporary
        //Adds all items from master list to available
        foreach (Item i in inventory)
        {
	        if (i.IsDiscontinued==false)
	        {
		        //If tthe item has room requirements and the current room isnt null
		        if (i.rooms.Count > 0)
		        {
			        if(em.CurrentRoom != null)
			        {
				        if (i.rooms.Contains(em.CurrentRoom) && !itemHasReachedMax(i))
				        {
					        availableItemsList.Add(i);
				        }
			        }
		        }
		        //If the room doesn't have room requirements...
		        else
		        {
			        if (!itemHasReachedMax(i))
			        {
				        availableItemsList.Add(i);
			        }
		        }
	        }
        }
    }

    //Removes any item that has reach
    private bool itemHasReachedMax(Item i)
    {
        //Temporary
        if (inventory[i.ItemIDNumber].Max > 0 && inventory[i.ItemIDNumber].Amt > inventory[i.ItemIDNumber].Max)
        {
            return true;
        }
        return false;
    }

	//Removes any item that has reach
	private bool itemHasReachedMax(int i)
	{
		//Temporary
		if (inventory[i].Max > 0 && inventory[i].Amt > inventory[i].Max)
		{
			return true;
		}
		return false;
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

        if(itemHasReachedMax(i))
        {
            availableItemsList.RemoveAt(i.ItemIDNumber);
			if (i.ItemIDNumber == 1)
			{
				hudman.displayMsg("You have 2 keys...",2);
				hudman.displayMsg("I'm sure you can use them for something...", 2);
			}
        }
    }

	public void AddItemToInventory(int i)
	{
		inventory[i].Amt++;
		//UPDATE GUI HERE
		ui.UpdateInventoryUI();
		hudman.displayMsg("You picked up a " + inventory[i].Name + "!", 2.5f);

		if (itemHasReachedMax(i))
		{
			availableItemsList.RemoveAt(i);
			if (i == 1)
			{
				hudman.displayMsg("You have 2 keys...", 2);
				hudman.displayMsg("I'm sure you can use them for something...", 2);
			}
		}
	}

    public void SubtractItemToInventory(int indx, int used)
    {
        inventory[indx].Amt -= used;
        ui.UpdateInventoryUI();
    }


    public void UpdateInventoryUI()
	{
		ui.UpdateInventoryUI ();
	}

	public void RemoveItemFromInventory(Item i)
	{
		int amt = inventory[i.ItemIDNumber].Amt;
		if (amt == 1) 
		{
			amt = 0;
		} 
		else if (amt > 1) 
		{
			amt--;
		}
        inventory[i.ItemIDNumber].Amt = amt;
		GetComponent<InventoryUI>().UpdateInventoryUI();
	}

    public int GetIndxOfItem(int indx)
    {
        int x = 0;
        foreach (Item i in inventory)
        {

            if (i.ItemIDNumber == indx)
            {
                return x;
            }
            x++;
        }
        return -1;
    }

    public Item GetItemFromID(int id)
    {
        int x = 0;
        foreach (Item i in inventory)
        {
            if (i.ItemIDNumber == id)
            {
                return i;
            }
            x++;
        }
        return null;
    }

	public List<Item> AvailableItemsList {
		get {
			return availableItemsList;
		}
	}

    public List<Item> InventoryList
    {
        get
        {
            return inventory;
        }
    }
}
