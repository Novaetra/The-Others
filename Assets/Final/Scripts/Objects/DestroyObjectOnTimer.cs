using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectOnTimer : MonoBehaviour {

    public float time;

    void Start()
    {
        StartCoroutine(waitToKill());
    }

    private IEnumerator waitToKill()
    {
		yield return new WaitWhile(()=>time >0);
        yield return new WaitForSeconds(time);
		transform.SendMessage("onDestroy", SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
