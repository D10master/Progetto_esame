using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    #region Fields

    private NavMeshAgent nav;
    private Transform target;
    private Animator animator;

    private AudioSource audioSrc;
    public AudioClip[] speakClips;
    public AudioClip[] screamClips;
    public AudioClip[] attackClips;
    public AudioClip[] dieClips;

    private float nextSound;

    public float speed;
    public int meleeDamage;
    public float meleeRange;
    public float attackTime;
    private float nextAttack;
    public int hp;

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

    public void Attack()
    {
        if(Vector3.Distance(target.transform.position, transform.position) <= meleeRange)
            target.GetComponent<Player>().TakeDamage(meleeDamage);
    }

    public void Attack(Player player)
    {
        player.TakeDamage(meleeDamage);
    }

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
        PlaySound(dieClips,0);
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for(int i=0; i<colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        gm.SpawnedZombies.Remove(this);
        gm.CheckZombies();
        Destroy(gameObject, 20);
        enabled = false;
    }

    public void PlaySound(AudioClip[] audioClips, int i)
    {
        audioSrc.clip = audioClips[i];
        audioSrc.Play();
    }

    #endregion

}
