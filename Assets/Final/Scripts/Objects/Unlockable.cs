using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//This class basically checks if you have the required items to unlock whatever it is that you have
public class Unlockable : MonoBehaviour
{
    [SerializeField]
    private Item[] itemsRequired;
    [SerializeField]
    private int amtRequired;

    private bool isUnlocked;
    private Inventory inv;

    void Start()
    {
        inv = GameObject.Find("InventoryManager").GetComponent<Inventory>();
    }

    public void interact(object[] parameters)
    {
        GameObject player = (GameObject)parameters[1];
        if (CheckIfPlayerHasAllItems())
        {
            Unlock();
            subtractItems();
        }

    }

    public void subtractItems()
    {
        foreach(Item i in itemsRequired)
        {
            int indx = getIndxOfItem(i);
            inv.SubtractItemToInventory(indx,amtRequired);
        }
    }

    private int getIndxOfItem(Item i)
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
        foreach (Item itemRequired in itemsRequired)
        {
            foreach (Item i in inv.InventoryList)
            {
                if (itemRequired.ItemIDNumber == i.ItemIDNumber)
                {
                    if (i.Amt >= amtRequired)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void Unlock()
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
