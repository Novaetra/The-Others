using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyComponentOnTimer : MonoBehaviour {

	public float time;
	public Component comp;

	void Start()
	{
		StartCoroutine(waitToKill());
	}

	private IEnumerator waitToKill()
	{
		yield return new WaitForSeconds(time);
		Destroy(comp);
	}
}
