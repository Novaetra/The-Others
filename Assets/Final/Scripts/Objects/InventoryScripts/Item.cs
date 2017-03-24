using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
	private string name, description;
	private int itemIDNumber, amt, dropRate;
	private Inventory inv;
	private HUDManager hudMan;
	private bool isPickedUp;

	void Start()
	{
		isPickedUp = false;
		name = gameObject.name;
		inv = GameObject.Find ("InventoryManager").GetComponent<Inventory> ();
		hudMan = GameObject.Find ("Player").GetComponent<HUDManager> ();
		foreach (Item i in inv._inventory) 
		{
			if (i.Name.Equals (name)) 
			{
				ItemIDNumber = i.ItemIDNumber;
				description = i.Description;
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
		if (col.tag == "Player" && !isPickedUp) 
		{
			isPickedUp = true;
			PickUpItem ();
		}
	}

	public Item(string n, string desc, int id,int dr)
	{
		name = n;
		description = desc;
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

	public string Description {
		get {
			return description;
		}
		set {
			description = value;
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

	public void OnPointerEnter(PointerEventData data)
	{
		hudMan.ShowItemTooltip (this, data,gameObject);
	}

	public void OnPointerExit(PointerEventData data)
	{
		StartCoroutine (StartHide());
	}

	IEnumerator StartHide()
	{
		yield return new WaitForSeconds(.1f);
		if (hudMan.IsOnTooltip == false) 
		{
			hudMan.HideTooltip ();
		}
	}
}
