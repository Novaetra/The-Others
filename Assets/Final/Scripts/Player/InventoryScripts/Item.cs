using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public ArrayList rooms;

	private string name, description;
    [SerializeField]
	private int itemIDNumber, amt, dropRate, maxNum;
	private Inventory inv;
	private HUDManager hudMan;
	private bool isPickedUp, isDiscontinued;
    
	void Start()
	{
		isPickedUp = false;
		inv = GameObject.Find ("InventoryManager").GetComponent<Inventory> ();
		hudMan = GameObject.Find ("Player").GetComponent<HUDManager> ();
	}

    public void AddRoom(string roomName)
    {
        Transform room = GameObject.Find(roomName).transform;
        rooms.Add(room);
    }

    public void AddRoom(Transform t)
    {
        rooms.Add(t);
    }

    public void AddFloor(string floorName)
    {
        Transform floor = GameObject.Find(floorName).transform;
        foreach(Transform room in floor.FindChild("Rooms"))
        {
            AddRoom(room);
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

	public Item(string n, string desc, int id,int dr, int max)
	{
		name = n;
		description = desc;
		itemIDNumber = id;
		amt = 0;
		dropRate = dr;
        maxNum = max;
        rooms = new ArrayList();
		isDiscontinued = false;
	}

	public Item(string n, string desc, int id,int dr, int max, bool discontinue)
	{
		name = n;
		description = desc;
		itemIDNumber = id;
		amt = 0;
		dropRate = dr;
		maxNum = max;
		rooms = new ArrayList();
		isDiscontinued = discontinue;
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

	public int Max
	{
		get{ return maxNum; }
	}

	public bool IsDiscontinued
	{
		get { return isDiscontinued; }
		set { isDiscontinued = value; }
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
