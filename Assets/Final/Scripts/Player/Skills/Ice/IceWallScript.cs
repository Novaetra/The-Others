using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWallScript : SkillGameObject
{
	protected override void doAdditionalSetupOnStart()
	{
		setRotationToPlayer();
	}
	private void setRotationToPlayer()
	{
		Transform player = GameObject.Find("Player").transform;
		transform.LookAt(player);
		Quaternion rot = transform.rotation;
		transform.rotation = Quaternion.Euler(rot.x, rot.y - 90f, rot.z);

	}
}
