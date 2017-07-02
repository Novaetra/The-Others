using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    public bool spawnEnemies;
	private bool roundHasStarted;
    [SerializeField]
    public GameObject[] ENEMY_OBJECTS;
    [SerializeField]
    private GameObject[] enemyObjectsAvailable;
    private MeteorManager meteorMan;
    private StatsManager playerSM;
    [SerializeField]
    private int currentEnemyCount;
    [SerializeField]
    private int currentWaveCount;
    private int nextRoundToAddEnemyType;
	private int maxEnemies;
    [SerializeField]
    private int enemysToSpawn;
    [SerializeField]
    private int enemiesSpawned;
    private int enemiesKilled;
    private float timeBetweenRounds;
    private float timeBetweenSpawns;

	private Coroutine startWaveCouroutine, spawningWaveCouroutine;

    [SerializeField]
    private Transform currentRoom; //This is to detect which room the player is in, and spawn enemies accordingly
	[SerializeField]
	private GameObject readyBtn;

	//This dictionary holds all the spawn points per room
    private Dictionary<string, Transform[]> spawnPointsInRoom = new Dictionary<string, Transform[]>();

    [SerializeField]
	private Dictionary<string,float[]> statsPerEnemy = new Dictionary<string, float[]>();
    private ArrayList enemyNames = new ArrayList();

	//This list keeps track of the rooms are adjacent to other rooms and which ones
    private List<Transform[]> adjacentRooms = new List<Transform[]>();

    //This list is the list that holds all the spawn points an enemy can spawn in
    [SerializeField]
    private List<Transform> spawnPointsAvailable = new List<Transform>();

    private List<GameObject> allEnemies = new List<GameObject>();

	//List of doors so we can figure out the adjacent rooms
    private Door[] doors;

	//Refrence to the player's HUD Manager
    private HUDManager hudMan;

    private PlayerController controller;
    private Inventory inv;

    //Sets starting values
    public void setUp()
    {
		//This if statement is for development purposes
		if (spawnEnemies == true)
        {
            CurrentRoom = (GameObject.FindGameObjectWithTag("Spawn Room").transform);
            roundHasStarted = false;
			//Sets all the starting values for the variables
			hudMan = GameObject.Find("Player").GetComponent<HUDManager> ();
            controller = GameObject.Find("Player").GetComponent<PlayerController> ();
			maxEnemies = 25;
			currentWaveCount = 0;
			enemiesSpawned = 0;
			timeBetweenRounds = 30f;
			timeBetweenSpawns = 1.5f;
			setupSpawnLists ();
			startWaveCouroutine = StartCoroutine (waitToStartNewRound ());
			doors = GameObject.FindObjectsOfType<Door> ();
            enemyObjectsAvailable = new GameObject[1];
            enemyObjectsAvailable[0] = ENEMY_OBJECTS[0];
            nextRoundToAddEnemyType = 5;
        }
        inv = GameObject.Find("InventoryManager").GetComponent<Inventory>();
        playerSM = GameManager.currentplayer.GetComponent<StatsManager>();
        enemiesKilled = 0;
		if (Meteor.Accounts.IsLoggedIn) 
		{
			meteorMan = GameObject.Find("MeteorManager").GetComponent<MeteorManager>();   
		}
        setUpEnemyStartingStats();
    }

    private void setUpEnemyStartingStats()
    {
        //health, exp, damagem, move speed (max of 5)
        statsPerEnemy["Skelly"] = new float[4] { 7f, 20f, 50f, 3f};
        enemyNames.Add("Skelly");
        statsPerEnemy["Weakling"] = new float[4] { 10f, 20f, 20f, 3f };
        enemyNames.Add("Weakling");
		statsPerEnemy["FirstBoss"] = new float[4] { 500f, 400f, 80f, 3.5f };
		enemyNames.Add("FirstBoss");
    }

    //Fills the list that contains all adjacent rooms and links the room to its spawn points
    private void setupSpawnLists()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
		//For each floor
        for (int i = 0; i < floors.Length; i++)
        {
			Transform rooms = floors[i].transform.Find("Rooms");
			//For each room
            for (int r = 0; r < rooms.childCount; r++)
            {
				//Add the room's spawns to the list "spawnPointsInRoom"
                Transform room = rooms.GetChild(r);
                linkRoomsToSpawns(room);
            }
			//Add the rooms that are adjacent to each other to the list "adjacentRooms"
            addAdjacentRooms(floors[i].transform);
        }
    }
    
	//Adds the adjacent rooms to list "adjacent rooms"
    private void addAdjacentRooms(Transform floor)
    {
		//For each door
        foreach(Transform door in floor.Find("Doors"))
        {
			//Temporary list to store adjacent rooms for later use
            List<Transform> _adjacentRoomsList = new List<Transform>(0);
            //Transform[] _adjacentRooms = new Transform[3];
            int i = 0;
			//For each adjacent room
            foreach (Transform t in door.GetComponent<Door>().getAdjacentRooms())
            {
                //Add the adjacent rooms to the temporary list of adjacent rooms
                _adjacentRoomsList.Add(t);
                i++;
            }
            //Create a final array of all adjacent rooms to that door
            Transform[] finalAdjacentRoomsList = new Transform[_adjacentRoomsList.Count];
            for(int x = 0; x<_adjacentRoomsList.Count;x++)
            {
                finalAdjacentRoomsList[x] = _adjacentRoomsList[x];
            }

			//Use the temporary list of adjacent rooms to populate the dictionary storing the adjacent rooms
            adjacentRooms.Add(finalAdjacentRoomsList);
        }
    }
    
	//Links room to the spawn points inside it
    private void linkRoomsToSpawns(Transform room)
    {
		//Makes sure there are spawns in the room
        if (room.Find("Spawns") != null)
        {
			//temporary list to store the spawn points per room for later use
            Transform[] spawns = new Transform[room.Find("Spawns").childCount];
            int z = 0;
			//For each spawn in room
            foreach (Transform spawn in room.Find("Spawns"))
            {
				//Add the spawn point to the temporary list
                spawns[z] = spawn;
                z++;
            }
			//Use temporary list to populate the list of spawnPointsInRoom
            spawnPointsInRoom.Add(room.name, spawns);
        }
    }

    //Updates the list of spawns available
    public void updateSpawnsAvailable()
    {
        inv.UpdateItemsAvailableToSpawn();
        //first it clears the list
        spawnPointsAvailable.Clear();
        if (adjacentRooms.Count > 0)
        {
            //Then it goes through the list of adjacent rooms and picks out all the elements that contain the player's current location (room)
            //I = each indivual group of adjacent rooms declared in each door object
            for (int i = 0; i < adjacentRooms.Count; i++)
            {
                for (int a = 0; a < adjacentRooms[i].Length-1; a++)
                {
                    //If a room in the list == player current room
					if (adjacentRooms[i][a] == currentRoom)
                    {
						//If the door linking the two rooms is open, then it adds the spawn points of that adjacent room to the list of available points
						//(The last element in array will always be the door
						if (adjacentRooms[i][adjacentRooms[i].Length-1].GetComponentInChildren<Door>().getOpen())
                        {
                            //For each spawn point in _______ room 
                            for (int b=0; b<adjacentRooms[i].Length-1;b++)
                            {
                                foreach (Transform spawn in spawnPointsInRoom[adjacentRooms[i][b].name])
                                {
                                    //If the list of available spawns doesn't contain the current spawn, then add it
                                    if (!spawnPointsAvailable.Contains(spawn))
                                    {
                                        spawnPointsAvailable.Add(spawn);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //Add current room's spawn points
        if(spawnPointsInRoom[CurrentRoom.name] != null)
        {
            foreach (Transform spawn in spawnPointsInRoom[CurrentRoom.name])
            {
                if (!spawnPointsAvailable.Contains(spawn))
                {
                    //Spawn is added twice to make it more likely for enemy to spawn in that room
                    spawnPointsAvailable.Add(spawn);
                    spawnPointsAvailable.Add(spawn);
                }
            }
        }
    }

	//Increases the current round by one, and starts spawning more enemies.
	//The number of enemies to spawn increases as well
	public void startNextRound()
	{
	    enemiesSpawned = 0;
		roundHasStarted = true;
		if (startWaveCouroutine != null) 
		{
			StopCoroutine (startWaveCouroutine);
		}
		readyBtn.SetActive (false);
		startWaveCouroutine = null;
        updateSpawnsAvailable();
        currentWaveCount++;
        hudMan.updateRoundsTxt(currentWaveCount);
	    enemysToSpawn = getNumberOfEnemysToSpawnInWave(currentWaveCount,enemysToSpawn);
        currentEnemyCount = enemysToSpawn;
        spawningWaveCouroutine = StartCoroutine(spawnWave(true));
	    if (currentWaveCount >= nextRoundToAddEnemyType && (enemyObjectsAvailable.Length+1 <= ENEMY_OBJECTS.Length))
	    {
	        nextRoundToAddEnemyType += currentWaveCount;
	        enemyObjectsAvailable = new GameObject[enemyObjectsAvailable.Length+1];
	        for (int i = 0; i < enemyObjectsAvailable.Length; i++)
	        {
	            enemyObjectsAvailable[i] = ENEMY_OBJECTS[i];
	        }
	    }
    }

    private int getNumberOfEnemysToSpawnInWave(int waveNumber, int enemiesSpawnedLastWave)
    {
        if (currentWaveCount < 10)
        {
            return enemiesSpawnedLastWave += 3;
        }
        else
        {
            //C = cap, P = principle, K = growth rate
            //float C=401f, P=2f, B=1.2f, K=.0007f, O=50f;
            //int count = (int)C / (1 + Mathf.Pow((((C / P) - 1) * B),(-K*C * (waveNumber + O))));

            return (int)(.15f * currentWaveCount * 24f);
        }
    }

	//Checks if there are any enemies left
	//If there are no more enemies left, then start the next round
    private void checkIfRoundEnd()
    {
        if (currentEnemyCount <= 0)
        {
            UpdateEnemyValues();
			startWaveCouroutine = StartCoroutine(waitToStartNewRound());
			if (Meteor.Accounts.IsLoggedIn) 
			{
				StartCoroutine(meteorMan.UpdateStats(currentWaveCount,(int) playerSM.getExpGained(), enemiesKilled));
			}
            enemiesKilled = 0;
            playerSM.resetExpGained();
        }
    }

    public int[] getStats()
    {
        int[] stats = new int[3] {currentWaveCount,(int)playerSM.getExpGained(),enemiesKilled};
        return stats;
    }

	//Decreases the number of enemies alive 
	//Is called every time an enemy is killed
    public void decreaseEnemyCount()
    {
        currentEnemyCount--;
        enemiesSpawned--;
        enemiesKilled++;
        checkIfRoundEnd();
    }

    public void RemoveEnemyFromList(GameObject go)
    {
        allEnemies.Remove(go);
    }

	//Spawns an enemy at a random spawn point
    public void spawnEnemy()
    {
        //Change this so it uses any of the spawn points in the array
        int randIndx = (int)Random.Range(0, (spawnPointsAvailable.Count));
		Transform spawn = spawnPointsAvailable[randIndx];
		randIndx = (int)Random.Range(0, enemyObjectsAvailable.Length);
        GameObject enemy = (GameObject)GameObject.Instantiate(enemyObjectsAvailable[randIndx], spawn.position, spawn.rotation);
        string enemytype = (string)enemyNames[randIndx];
        setStartingStats(enemy.GetComponent<EnemyController>(),enemytype);
        allEnemies.Add(enemy);
    }

	public void spawnEnemy(GameObject enemy, Transform spawn, string enemyType)
	{
		GameObject _enemy = (GameObject)GameObject.Instantiate(enemy, spawn.position, spawn.rotation);
		string enemytype = enemyType;
		setStartingStats(enemy.GetComponent<EnemyController>(), enemytype);
		allEnemies.Add(enemy);
	}

    private void setStartingStats(EnemyController enemy, string enemyName)
    {
        enemy.setTotalHealth(statsPerEnemy[enemyName][0]);
        enemy.setExpOnKill(statsPerEnemy[enemyName][1]);
        enemy.setMeleeDamage(statsPerEnemy[enemyName][2]);
        //enemy.setAttackProximity(statsPerEnemy[enemyName][3]);
        enemy.setMovementSpeed(statsPerEnemy[enemyName][3]);
    }

    //Every round increase enemy health, exp value, and dmg
    private void UpdateEnemyValues()
    {
        if (currentWaveCount < 10)
        {
            for(int i = 0;i<enemyNames.Count;i++)
            {
                statsPerEnemy[(string)enemyNames[i]][0] += 5f;
                statsPerEnemy[(string)enemyNames[i]][1] += 5f;
                statsPerEnemy[(string)enemyNames[i]][2] += 5f;
            }
        }
        else
        {
            for (int i = 0; i < enemyNames.Count; i++)
            {
                //Temporary values....These will escalate out of control in higher rounds
                statsPerEnemy[(string)enemyNames[i]][0] += statsPerEnemy[(string)enemyNames[i]][0] * .02f;
                statsPerEnemy[(string)enemyNames[i]][1] += statsPerEnemy[(string)enemyNames[i]][0] * .02f;
                statsPerEnemy[(string)enemyNames[i]][2] += statsPerEnemy[(string)enemyNames[i]][2] * .02f;
            }
        }
    }


	//Spawns the whole wave of enemies
    private IEnumerator spawnWave(bool spawnWithDelays)
    {
        int tempEnemiesToSpawn = enemysToSpawn;
        for (int x = 0; x< tempEnemiesToSpawn; x++)
        {
            while (enemiesSpawned >= maxEnemies)
            {
                yield return null;
            }

            while(currentRoom==null)
            {
                yield return null;
            }
            if(spawnWithDelays)
			    yield return new WaitForSeconds(timeBetweenSpawns);

            enemiesSpawned++;
            enemysToSpawn--;
			spawnEnemy();
        }
    }

    private void KillAllEnemies()
    {
        int count = allEnemies.Count;
        for(int i = 0;i<count;i++)
        {
            GameObject go = allEnemies[i];
            if (go.GetComponent<EnemyController>().IsAlive)
            {
                go.GetComponent<EnemyController>().Kill();
                enemysToSpawn++;
            }
            else
            {
                Destroy(go);
            }
        }
        allEnemies.Clear();
    }

	//Waits x seconds before starting the next round
    private IEnumerator waitToStartNewRound()
    {
        yield return new WaitForSeconds(2f);
        hudMan.displayMsg("Press " + controller.ControlHotkeys[0].Codes[4] + " to start next round",5f);
		roundHasStarted = false;
		readyBtn.SetActive (true);
        yield return new WaitForSeconds(timeBetweenRounds);
        startNextRound();
    }

	public bool RoundHasStarted {
		get {
			return roundHasStarted;
		}
	}

    public Transform CurrentRoom
    {
        get
        {
            return currentRoom;
        }

        set
        {
            if (value == null)
            {
                KillAllEnemies();
                if(spawningWaveCouroutine != null)
                    StopCoroutine(spawningWaveCouroutine);
            }

            if(currentRoom == null && value != null)
            {
                startWaveCouroutine = StartCoroutine(spawnWave(false));
            }

            currentRoom = value;
        }
    }
}