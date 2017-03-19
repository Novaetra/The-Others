﻿using UnityEngine;
using System.Collections;

public class FireballScript : MonoBehaviour 
{
    public GameObject SPARK;
    private SkillManager sm;
    private float dmg;
    private float range = 0.75f;
    private Raycaster[] casters;
    private bool isDestroyed;
	private int enemiesHit = 0, maxEnemiesHit;

	void Start()
	{
        sm = GameManager.currentplayer.GetComponent<SkillManager>();
        SPARK = GameManager.currentplayer.GetComponent<SkillInitializer>().SPARK;
        for(int i = 0; i<sm.getKnownSkills().Count;i++)
        {
			if(sm.getKnownSkills()[i].Name.Equals("Fireball"))
            {
				Skill s = sm.getKnownSkills () [i];
				dmg = s.EffectAmount;
				maxEnemiesHit = s.MaxEnemiesHit;
            }
        }

        casters = GetComponentsInChildren<Raycaster>();
        isDestroyed = false;
    }
		
	private IEnumerator destroyFire()
	{
        GetComponent<ParticleSystem> ().Stop (true);
        yield return new WaitForSeconds (1f);
        Destroy(transform.parent.gameObject);
    }

    public void destroyFireball()
	{
        isDestroyed = true;
        StartCoroutine (destroyFire ());
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            col.transform.BroadcastMessage("recieveDamage", dmg, SendMessageOptions.RequireReceiver);
            GameObject.Instantiate(SPARK, transform.position,transform.rotation);
			enemiesHit++;
			if (enemiesHit >= maxEnemiesHit) 
			{
				Destroy (gameObject);
			}
        }
    }
}