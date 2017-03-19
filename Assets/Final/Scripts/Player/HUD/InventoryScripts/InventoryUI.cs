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
					itemSlot.GetChild (1).GetComponent<Text> ().text = "x" + inv._inventory [i.ItemIDNumber].Amt;
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
			if (child.GetChild (0).GetComponent<Text> ().text.Equals (i.Name)) 
			{
				Destroy (child.gameObject);
			}
		}
	}

	private void SetUpNewItemSlot(Item i)
	{
		GameObject slotCreated = GameObject.Instantiate (INVENTORY_SLOT,slots);
		Transform transf = slotCreated.transform;
		transf.GetChild (0).GetComponent<Text> ().text = i.Name;
		transf.GetChild (1).GetComponent<Text> ().text = "x"+i.Amt;
		RectTransform rect = slotCreated.GetComponent<RectTransform> ();
		rect.rotation = slots.rotation;
		rect.localScale = new Vector3 (1f, 1f, 1f);
		rect.transform.localPosition = new Vector3(0,0,0);
	}

	private Transform GetItemInventorySlot(Item i)
	{
		foreach (Transform child in slots) 
		{
			if (child.GetChild (0).GetComponent<Text> ().text.Equals (i.Name)) 
			{
				return child;
			}
		}
		return null;
	}
}
