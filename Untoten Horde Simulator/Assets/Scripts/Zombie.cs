using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    #region Fields

	//RIFERIMENTI A COMPONENTI
	/*riferimento al nav mesh agent che permette allo zombie
	di camminare nelle aree dove gli è consentito*/
    private NavMeshAgent nav;
	//riferimento al giocatore da inseguire
    private Transform target;
	//riferimento al componente che gestisce le animazioni
    private Animator animator;

	//COMPONENTI AUDIO
	//riferimento al componente che si occupa di emettere suoni
    private AudioSource audioSrc;
	//gruppo di array per diverse tipologie di clip audio
    public AudioClip[] speakClips;
    public AudioClip[] screamClips;
    public AudioClip[] attackClips;
    public AudioClip[] dieClips;
	//contatore del tempo restante per l'emissione del suono successivo
    private float nextSound;


	//CARATTERISTICHE FISICHE DELLO ZOMBIE
	//velocità di movimento
    public float speed;
	//danno inflitto dagli attacchi corpo a corpo
    public int meleeDamage;
	//distanza degli attacchi corpo a orpo
    public float meleeRange;
	//tempo impiegato per sferrare un attacco
    public float attackTime;
	//contatore del tempo per il prossimo attacco
    private float nextAttack;
	//punti vita dello zombie
    public int hp;


	//ATTRIBUTI DROP DI OGGETTI
	public float armorDropRate;
	public GameObject armorPickUp;
	public float ammoDropRate;
	public GameObject ammoPickUp;

    #endregion


    #region UnityMethods

    // Use this for initialization
    void Start ()
    {
        audioSrc = GetComponent<AudioSource>();
        nav = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();

        nav.stoppingDistance = meleeRange - 0.25f;
        nav.speed = speed;
        if(speed > Consts.ZOMBIE_RUNNING_SPEED)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
        animator.speed = speed * Consts.ZOMBIE_ANIMATION_SPEED_SCALE;

        nextAttack = attackTime;
        nextSound = Random.Range(Consts.MIN_ZOMBIE_SOUND_TIME, Consts.MIN_ZOMBIE_SOUND_TIME);
    }

    // Update is called once per frame
    void Update()
    {
        if (nav.isActiveAndEnabled && target)
        {
            //transform.LookAt(target);
            if (Vector3.Distance(transform.position, target.position) > meleeRange)
            {
                nav.SetDestination(target.position);
                nextAttack = attackTime;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walking") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                {
                    if (animator.GetBool("IsRunning"))
                    {
                        animator.SetTrigger("Run");
                    }
                    else
                    {
                        animator.SetTrigger("Walk");
                    }
                }
            }
            else
            {
                //Debug.Log("In attack range");
                if (nextAttack > 0)
                {
                    nextAttack -= Time.deltaTime;
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {
                        animator.SetTrigger("Idle");
                    }
                }
                else
                {
                    //Attack(target.GetComponent<Player>());
					Attack();
                    animator.SetTrigger("Attack");
                    PlaySound(attackClips, Random.Range(0, attackClips.Length));
                    nextAttack = attackTime;
                }
            }

            if (nav.isOnOffMeshLink)
            {
                animator.SetTrigger("Jump");
            }
        }

        if(nextSound > 0)
        {
            nextSound -= Time.deltaTime;
        }
        else
        {
            PlaySound(speakClips, Random.Range(0,speakClips.Length));
            nextSound = Random.Range(Consts.MIN_ZOMBIE_SOUND_TIME, Consts.MIN_ZOMBIE_SOUND_TIME);
        }
    }

    #endregion


    #region Methods

    public void TakeDamage(int damage, Player player)
    {
        hp -= damage;
        player.ModifyPoints(10); 
        //Debug.Log("Damage:" + damage);
        if(hp <= 0)
        {
            //Debug.Log("Dead");
            Die(player);
        }
    }

    private void Die(Player player)
    {
        GameManager gm = GameObject.Find("Game Manager").GetComponent<GameManager>();

        hp = 0;
        player.ModifyPoints(50);
        nav.enabled = false;
        animator.SetBool("IsDead", true);
        animator.speed = 1f;
        animator.SetTrigger("Die");
		PlaySound(dieClips,Random.Range(0,dieClips.Length));
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for(int i=0; i<colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

		AttemptAmmoGeneration ();
		AttemptArmorGeneration ();


        gm.SpawnedZombies.Remove(this);
        gm.CheckZombies();
        Destroy(gameObject, 20);
        enabled = false;
    }

	public void AttemptArmorGeneration()
	{
		if (Random.value < armorDropRate)
		{
			Instantiate (armorPickUp, transform.position, Quaternion.identity);
		}
	}

	public void AttemptAmmoGeneration()
	{
		if (Random.value < ammoDropRate)
		{
			Instantiate (ammoPickUp, transform.position, Quaternion.identity);
		}
	}



	public void Attack()
	{
		if (Vector3.Distance (target.transform.position, transform.position) <= meleeRange)
		{
			float deltaX = target.position.x - transform.position.x;
			float deltaZ = target.position.z - transform.position.z;
			float angle = Mathf.Atan2 (deltaZ, deltaX);
			target.GetComponent<Player> ().TakeDamage (meleeDamage, angle);
		}
	}

	public void Attack(Player player)
	{
		player.TakeDamage(meleeDamage);
	}

    public void PlaySound(AudioClip[] audioClips, int i)
    {
        audioSrc.clip = audioClips[i];
        audioSrc.Play();
    }

    #endregion

}
