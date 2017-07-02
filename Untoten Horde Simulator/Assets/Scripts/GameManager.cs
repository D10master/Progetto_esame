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
	public Transform environment;

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
	private int maxSpawnedZombies;
    public int startingZombiesHealth;
    public int zombiesHealthIncrement;
    private int zombiesHealth;
    private int zombiesAttack;
    private int zombiesSpeed;

    //UI elements
    public Text roundText;
    public GameObject eegPanel;

    public Animator gameOverAnimator;

    //neurosky data
    public bool isAnalyzingBrain;
	public TGCConnectionController connectionController;
	private int attention;
	public float attentionPickTime;
	private float nextPick;
	private List<float> attentionLevels;
	public int attentionListLength = 150;
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

		maxSpawnedZombies = Consts.MAX_ZOMBIES_IN_SCENE;
        round = 0;
		timeToNextRound = roundPauseTime;
		roundInProgress = false;

        nextSpawn = Random.Range(Consts.MIN_ZOMBIE_SPAWN_TIME, Consts.MAX_ZOMBIE_SPAWN_TIME);

		connectionController.UpdateAttentionEvent += OnUpdateAttention;
		attentionLevels = new List<float>();
		nextPick = attentionPickTime;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (!eegPanel.activeSelf)
            {
                eegPanel.SetActive(true);
            }
            else
            {
                eegPanel.SetActive(false);
            }
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(!Demo.demo)
            {
                Demo.demo = true;
            }
            else
            {
                Demo.demo = false;
            }
        }
		else if (Input.GetButtonDown ("Escape"))
		{
			Application.Quit ();
		}
		else if (Input.GetKeyDown (KeyCode.N))
		{
			Debug.Log (AverageAttention ());
		}

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
                    nextSpawn = Random.Range(Consts.MIN_ZOMBIE_SPAWN_TIME, Consts.MAX_ZOMBIE_SPAWN_TIME);
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
		player.transform.SetParent (environment);
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
        float attentionPerc = AverageAttention() / 100f;
        zombiesAttack = Mathf.RoundToInt(Mathf.Lerp(Consts.ZOMBIE_MIN_ATTACK, Consts.ZOMBIE_MAX_ATTACK, attentionPerc));
        zombiesSpeed = Mathf.RoundToInt(Mathf.Lerp(Consts.ZOMBIE_MIN_SPEED, Consts.ZOMBIE_MAX_SPEED, attentionPerc));
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
		attentionLevels.Add (attention);

		if (attentionLevels.Count > attentionListLength) {
			attentionLevels.Remove (0);
		}
	}

	public float AverageAttention()
	{
		int total=0;

		foreach (int val in attentionLevels)
		{
			total += val;
		}

		return total / attentionLevels.Count;
	}

	private void OnUpdateAttention(int value)
	{
		attention = value;
	}

    #endregion

    #endregion

}
