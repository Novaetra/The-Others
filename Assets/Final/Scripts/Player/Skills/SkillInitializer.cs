using UnityEngine;
using System.Collections;

public class SkillInitializer : MonoBehaviour 
{
    public GameObject SPAWNER, FIREBALL, SPARK, FLAMETHROWER, HEAL;
	public Transform leftSpawner;
    public Transform middleSpawner;
    public Transform floorSpawner;
	private GameObject spellSpawner;

	private Object[] spellObjects;

	void Start()
	{
		LoadResources ();
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
	}

    public void createFlamethrower()
    {
        GameObject flameThrower = (GameObject)GameObject.Instantiate(FLAMETHROWER, leftSpawner.transform.position, leftSpawner.transform.rotation);
        flameThrower.transform.SetParent(leftSpawner.transform);
        flameThrower.transform.localScale = new Vector3(1, 1, 1);
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
}
