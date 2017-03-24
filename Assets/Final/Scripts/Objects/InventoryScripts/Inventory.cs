using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour 
{
	[SerializeField]
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
		inventory = new List<Item> ();
		inventory.Add (new Item ("Skill Charge","Strong energy used to cast powerful spells.",0,20));
	}

	public void AddItemToInventory(Item i)
	{
		inventory[i.ItemIDNumber].Amt++;
		//UPDATE GUI HERE
		ui.UpdateInventoryUI();
		hudman.displayMsg ("You picked up a " + inventory[i.ItemIDNumber].Name + "!",2.5f);

	}

	public void UpdateInventoryUI()
	{
		ui.UpdateInventoryUI ();
	}

	public void RemoveItemFromInventory(Item i)
	{
		int amt = inventory[i.ItemIDNumber].Amt;
		if (amt > 1 && Input.GetKey (KeyCode.LeftShift) || amt == 1) 
		{
			amt = 0;
		} 
		else if (amt > 1) 
		{
			amt--;
		}
		inventory [i.ItemIDNumber].Amt = amt;
		GetComponent<InventoryUI>().UpdateInventoryUI();
	}

	public List<Item> _inventory {
		get {
			return inventory;
		}
	}
}
