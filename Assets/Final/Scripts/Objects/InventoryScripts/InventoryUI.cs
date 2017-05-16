using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour 
{
	private Inventory inv;
	private Transform slots;

	private GameObject INVENTORY_SLOT;

	void Awake()
	{
		inv = GetComponent<Inventory> ();
		slots = GameObject.Find ("Inventory Slots").transform;
		INVENTORY_SLOT = (GameObject)Resources.Load ("InventorySlot");
	}
    
	public void UpdateInventoryUI()
	{
		foreach (Item i in inv._inventory) 
		{
			if (i.Amt > 0) 
			{
				Transform itemSlot = GetItemInventorySlot (i);
				if (itemSlot != null) {
					itemSlot.GetChild (1).GetComponent<Text> ().text = "x" + i.Amt;
				} else {
					SetUpNewItemSlot (i);
				}
			} else 
			{
				RemoveItem (i);
			}
		}
	}

	private void RemoveItem(Item i)
	{
		foreach (Transform child in slots) 
		{
			if(i.Name == child.GetComponent<Item>().Name)
			{
				Destroy (child.gameObject);
			}
		}
	}

	private void SetUpNewItemSlot(Item i)
	{
		GameObject slotCreated = GameObject.Instantiate (INVENTORY_SLOT,slots);
		slotCreated.name = i.Name;
		Transform transf = slotCreated.transform;
		transf.GetChild (0).GetComponent<Text> ().text = i.Name;
		transf.GetChild (1).GetComponent<Text> ().text = "x"+i.Amt;
		RectTransform rect = slotCreated.GetComponent<RectTransform> ();
		rect.rotation = slots.rotation;
		rect.localScale = new Vector3 (1f, 1f, 1f);
		rect.transform.localPosition = new Vector3(0,0,0);
		Item item = slotCreated.AddComponent<Item> ();
		CopyItemComponent (i, item);
	}
	//Copies i1's attributes to i2
	private void CopyItemComponent(Item i1,Item i2)
	{
		i2.Name = i1.Name;
		i2.Description = i1.Description;
		i2.ItemIDNumber = i1.ItemIDNumber;
		i2.Amt = i1.Amt;
		i2.DropRate = i1.DropRate;
	}

	private Transform GetItemInventorySlot(Item i)
	{
		foreach (Transform child in slots) 
		{
			if (i.Name == child.GetComponent<Item>().Name) 
			{
				return child;
			}
		}
		return null;
	}
}
