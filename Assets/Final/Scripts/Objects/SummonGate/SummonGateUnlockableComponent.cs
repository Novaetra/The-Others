using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonGateUnlockableComponent : Unlockable
{
	[SerializeField]
	private GameObject boss;

	[SerializeField]
	private Transform bossSpawn;

	protected override void setUpItemsRequired()
	{
		itemsRequired = new Item[2];
		inv.InventoryList.ForEach((Item obj) =>
		{
			//Find key in inventory and add it to list of required items
			if (obj.ItemIDNumber == 1)
			{
				itemsRequired[0] = obj;
				itemsRequired[1] = obj;
			}
		});
	}

	protected override void Unlock()
	{
		LockOtherGates();
		SummonBoss();
	}

	private void SummonBoss()
	{
		GameObject.Find("Managers").GetComponent<EnemyManager>().spawnEnemy(boss, bossSpawn, "FirstBoss");
	}

	private void LockOtherGates()
	{
		Transform gates = GameObject.Find("Gates").transform;
		foreach (Transform gate in gates)
		{
			gate.GetComponent<TrapGate>().IsOpen = false;
		}
		GameObject.Find("BossRoomToSpawn").GetComponent<Door>().setOpen(false);
	}

	public void BossWasKilled()
	{
		UnlockOtherGates();
		GetComponent<SummonGateDoorComponent>().openDoor();
	}

	private void UnlockOtherGates()
	{
		Transform gates = GameObject.Find("Gates").transform;
		foreach (Transform gate in gates)
		{
			gate.GetComponent<TrapGate>().IsOpen = true;
		}
		GameObject.Find("BossRoomToSpawn").GetComponent<Door>().setOpen(true);
	}

	protected override void CouldNotUnlock()
	{
		hudMan.displayMsg("Not enough keys...", 1f);
	}
}



