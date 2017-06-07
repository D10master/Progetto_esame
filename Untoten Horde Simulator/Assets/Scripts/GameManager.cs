using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    #region Fields

    //audio component
    public AudioSource ambientSoundSource;
    private AudioSource audioSrc;
    public AudioClip ambientClip;
    public AudioClip roundStartClip;
    public AudioClip roundEndClip;
    public AudioClip gameOverClip;

    //placeholders
    public Transform spawnPoint;
    public GameObject player;
    public GameObject[] zombies;

    private MapGenerator mapGenerator;

    //round fields
	public float roundPauseTime;
	private float timeToNextRound;
    private int round;
    private bool roundInProgress;
    private float nextSpawn;
    private List<Transform> zombiesSpawnPoints;


    //zombies attributes
    public float walkerSpeed;
    public float runnerSpeed;
    public int startingZombiesCount;
    public int zombiesIncrement;
    private int zombiesReserve;
    private List<Zombie> spawnedZombies;
    public int maxSpawnedZombies;
    public int startingZombiesHealth;
    public int zombiesHealthIncrement;
    private int zombiesHealth;

    //UI elements
    public Text roundText;

    public Animator gameOverAnimator;

	//neurosky data
	public DisplayData neuroskyData;
	public float attentionPickTime;
	private float nextPick;
	private List<int> attentionLevels;
	public int attentionListLenght = 150;

    #endregion


    #region Getters and Setters

    public List<Zombie> SpawnedZombies
    {
        get
        {
            return spawnedZombies;
        }

        set
        {
            spawnedZombies = value;
        }
    }

    public List<Transform> SpawnPoints
    {
        get
        {
            return zombiesSpawnPoints;
        }

        set
        {
            zombiesSpawnPoints = value;
        }
    }



    #endregion


    #region UnityMethods

    // Use this for initialization
    void Start ()
    {
        audioSrc = GetComponent<AudioSource>();
        zombiesSpawnPoints = new List<Transform>();
        spawnedZombies = new List<Zombie>();
        mapGenerator = GetComponent<MapGenerator>();
        mapGenerator.GenerateMap();
        SpawnPlayer();

        round = 0;
		timeToNextRound = roundPauseTime;
		roundInProgress = false;

        nextSpawn = Random.Range(Consts.MIN_SPAWN_TIME, Consts.MAX_SPAWN_TIME);
		attentionLevels = new List<int>();
		nextPick = attentionPickTime;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(roundInProgress)
		{
			if(nextSpawn >  0)
			{
				nextSpawn -= Time.deltaTime;
			}
			else
			{
                if (SpawnedZombies.Count < Consts.MAX_ZOMBIES_IN_SCENE)
                {
                    SpawnZombie(Vector3.zero);
                    nextSpawn = Random.Range(Consts.MIN_SPAWN_TIME, Consts.MAX_SPAWN_TIME);
                }
			}
		}
        else if(timeToNextRound > 0)
        {
            timeToNextRound -= Time.deltaTime;
        }
        else
        {
            StartRound();
        }

        if (!ambientSoundSource.isPlaying)
        {
            ambientSoundSource.clip = ambientClip;
            ambientSoundSource.Play();
        }

		if (nextPick > 0)
		{
			nextPick -= Time.deltaTime;
		}
		else
		{
			AddAttentionLevel ();
			nextPick = attentionPickTime;
		}
	}

    #endregion


    #region Methods

    private void SpawnPlayer()
    {
        player = Instantiate(player, spawnPoint.position, spawnPoint.rotation);
        player.name = "Player";
    }

    public void CheckZombies()
    {
        if(zombiesReserve <= 0 && spawnedZombies.Count <= 0)
        {
            EndRound();
        }
    }

    public void StartRound()
    {
        round++;
        zombiesReserve = startingZombiesCount + (zombiesIncrement * round - 1);
        zombiesHealth = startingZombiesHealth + (zombiesHealthIncrement * round - 1);
        roundInProgress = true;
        UpdateRoundText();
        ambientSoundSource.clip = roundStartClip;
        ambientSoundSource.Play();
    }

    public void EndRound()
    {
        spawnedZombies.Clear();
        timeToNextRound = roundPauseTime;
        roundInProgress = false;
        ambientSoundSource.clip = roundEndClip;
        ambientSoundSource.Play();
    }

    public void UpdateRoundText()
    {
        roundText.text = "Round  " + round;
    }

    private void SpawnZombie()
    {
        SpawnZombie(zombiesSpawnPoints[Random.Range(0, zombiesSpawnPoints.Count)].position);
    }

	private void SpawnZombie(Vector3 position)
	{
        GameObject zombieGameObject = Instantiate(zombies[Random.Range(0, zombies.Length)], position, Quaternion.identity);
        Zombie zombieComponent = zombieGameObject.GetComponent<Zombie>();
        zombieComponent.hp = zombiesHealth;

        zombiesReserve--;
        spawnedZombies.Add(zombieComponent);
	}

    public void GameOver()
    {
        gameOverAnimator.SetTrigger("Game Over");
        ambientSoundSource.clip = gameOverClip;
        ambientSoundSource.Play();
    }

    #region Neurosky Methods

    private void AddAttentionLevel()
	{
		attentionLevels.Add (neuroskyData.Attention1);

		if (attentionLevels.Count > attentionListLenght) {
			attentionLevels.Remove (0);
		}
	}

	public int AverageAttention()
	{
		int total=0;

		foreach (int val in attentionLevels)
		{
			total += val;
		}

		return Mathf.RoundToInt((float)total / attentionLevels.Count);
	}

    #endregion

    #endregion

}
