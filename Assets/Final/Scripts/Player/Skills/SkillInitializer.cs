using UnityEngine;
using System.Collections;

public class SkillInitializer : MonoBehaviour 
{
    public GameObject SPAWNER, FIREBALL, EXPLOSION, FLAMETHROWER, HEAL;
	public Transform leftSpawner;
    public Transform middleSpawner;
    public Transform floorSpawner;
	private GameObject spellSpawner;
    private GameObject currentEffect;
	private Object[] spellObjects;
	private SkillManager skillManager;
	public LayerMask groundLayer;
	void Start()
	{
		LoadResources ();
		skillManager = GetComponent<SkillManager>();
	}

	private void LoadResources()
	{
		//Loads resources
		spellObjects = Resources.LoadAll("Spells");
		foreach (GameObject s in spellObjects) {
			if(s.name.Equals("Spawner"))
			{
				spellSpawner = s;
			}
		}
	}

	public void CreateAreaObject(string name)
	{
		GameObject s = GetGameObject(name);
		RaycastHit hit;
		if(Physics.Raycast(middleSpawner.position,-middleSpawner.forward, out hit))
		{
			GameObject area = (GameObject)GameObject.Instantiate(s, hit.point, Quaternion.identity);
		}
	}

	public void CreateProjectile(string name)
	{
		GameObject s = GetGameObject (name);
		//Instantiate a spell spawner at the location, then instantite a projectile. Set the rotation of the spell spawner to make the projectile go straight
		GameObject spellSpawnerInstance = (GameObject)GameManager.Instantiate (spellSpawner, leftSpawner.position, leftSpawner.rotation);
		Debug.DrawRay (spellSpawnerInstance.transform.position, spellSpawnerInstance.transform.forward * 2f,Color.red,2f);
		//spellSpawner.transform.rotation += new Vector3 (0f, 90f);
		GameObject projectile = (GameObject)GameObject.Instantiate (s, spellSpawnerInstance.transform.position, spellSpawnerInstance.transform.rotation);
		projectile.transform.SetParent(spellSpawnerInstance.transform);
		projectile.transform.localScale = new Vector3(1f,1f,1f);
	}

	public void CreateFloorSpell(string name)
	{
		GameObject s = GetGameObject (name);
		GameObject spell = (GameObject)GameObject.Instantiate(s, floorSpawner.transform.position, floorSpawner.transform.rotation);
		spell.transform.SetParent(floorSpawner.transform);
		spell.transform.localEulerAngles += new Vector3(0f, 180, 0f);
		spell.transform.localScale = new Vector3(1f, 1f, 1f);
        currentEffect = spell;

    }

    public void CreateHoldSpell(string name)
    {
        //Get the spell from list of spells
        GameObject s = GetGameObject(name);

        GameObject spell = (GameObject)GameObject.Instantiate(s, leftSpawner.transform.position, leftSpawner.transform.rotation);
        spell.transform.SetParent(leftSpawner.transform);
        spell.transform.localScale = new Vector3(1, 1, 1);
        currentEffect = spell;
    }

	public void CreateBuffSpell(string name)
	{
		switch (name)
		{
			case "StormFlurry":
				StormFlurryScript storm = gameObject.AddComponent<StormFlurryScript>();
				Skill stormSkill = skillManager.FindSkillInKnownSkills(Skills.StormFlurry);
				if (stormSkill != null)
				{
					storm.SetDamageBuff(stormSkill.EffectAmount);
					storm.SetDuration(stormSkill.Duration);
				}
				break;
		}
	}

	private GameObject GetGameObject(string name)
	{
		foreach (GameObject s in spellObjects) {
			if (name.Equals (s.name)) {
				return s;
			}
		}
		return null;
	}

    public void DestroyCurrentEffect()
    {
        if(currentEffect!=null)
        {
            GameObject temp = currentEffect;
            currentEffect = null;
            StartCoroutine(temp.GetComponent<SpellScript>().DestroySelf());
        }
    }
}
