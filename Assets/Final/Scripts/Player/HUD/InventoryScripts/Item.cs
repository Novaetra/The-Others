using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour 
{
	private string name;
	private int itemIDNumber, amt, dropRate;
	[SerializeField]
	private Inventory inv;

	void Start()
	{
		name = gameObject.name;
		inv = GameObject.Find ("InventoryManager").GetComponent<Inventory> ();
		foreach (Item i in inv._inventory) 
		{
			if (i.Name.Equals (name)) 
			{
				ItemIDNumber = i.ItemIDNumber;
				DropRate = i.DropRate;
			}
		}
	}

	public override string ToString()
	{
		return this.name;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			PickUpItem ();
		}
	}

	public Item(string n, int id,int dr)
	{
		name = n;
		itemIDNumber = id;
		amt = 0;
		dropRate = dr;
	}
		
	public void PickUpItem()
	{
		inv.AddItemToInventory (this);
		inv.UpdateInventoryUI ();
		Destroy (gameObject);
	}

	public int Amt {
		get {
			return amt;
		}
		set {
			amt = value;
		}
	}

	public string Name {
		get {
			return name;
		}
		set {
			name = value;
		}
	}

	public int ItemIDNumber {
		get {
			return itemIDNumber;
		}
		set {
			itemIDNumber = value;
		}
	}

	public int DropRate {
		get {
			return dropRate;
		}
		set {
			dropRate = value;
		}
	}
}
