using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScript : MonoBehaviour
{
    

    public virtual IEnumerator DestroySelf()
    {
        yield return 0;
    }
}
