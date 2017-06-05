﻿using UnityEngine;
using System.Collections;

public class FlamethrowerScript : SpellScript
{
    
    private SkillManager sm;
    private float dmg;

	private PlayerController ps;

	void Start ()
    {
        sm = GameManager.currentplayer.GetComponent<SkillManager>();
        for (int i = 0; i < sm.getKnownSkills().Count; i++)
        {
            if (sm.getKnownSkills()[i].Name.Equals("Flamethrower"))
            {
				dmg = sm.getKnownSkills()[i].EffectAmount;
            }
        }

		ps = GetComponentInParent<PlayerController> ();
        
	}
    

    public override IEnumerator DestroySelf()
    {
        GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
    

    private void OnTriggerStay(Collider col)
    {
        if(col.tag == "Enemy")
        {
            col.transform.GetComponent<EnemyController>().recieveDamage(dmg);
        }
    }

}
