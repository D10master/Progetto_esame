using UnityEngine;

public class Weapon : MonoBehaviour
{

    #region Fields

    public FireType fireType;
    public LayerMask layer;
    private Animator animator;
    public ParticleSystem muzzleFlash;
    private AudioSource sound;
    public Player player;

    //parametri del danno
    public int maxDamage;
    public int maxDamageDistance;
    public int minDamage;
    public int minDamageDistance;

    //parametri delle munizioni
    public int maxAmmo;
    public int magazineSize;

    //parametri del tempo tra uno spare e l'altro
    public float shotTime;
    private float nextShot;
    public int shootsPerBlast;
    private int blastQueue;
    public float blastDelay;
    private float nextBlast;

    public Transform barrel;
    public GameObject[] bulletHoles;


    //weapon placeholders
    public Vector3 normalPosition;
    public Vector3 aimingPosition;
    
    //souds
    public AudioClip shootClip;
    public AudioClip reloadClip;

    private int ammoReserve;
    private int ammoInMagazine;

    private bool canShoot;

    #endregion


    #region Getters and Setters

    public Animator Animator
    {
        get
        {
            return animator;
        }

        set
        {
            animator = value;
        }
    }

    public AudioSource Audio
    {
        get
        {
            return sound;
        }

        set
        {
            sound = value;
        }
    }

    public int AmmoReserve
    {
        get
        {
            return ammoReserve;
        }

        set
        {
            ammoReserve = value;
        }
    }

    public int AmmoInMagazine
    {
        get
        {
            return ammoInMagazine;
        }

        set
        {
            ammoInMagazine = value;
        }
    }

    #endregion


    #region Unity methods

    // Use this for initialization
    void Start ()
    {
        if(!player) player = transform.parent.parent.parent.GetComponent<Player>();
        player.UpdateAmmoCounter();
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
        nextShot = 0.0f;
        ammoReserve = maxAmmo;
        ammoInMagazine = magazineSize;
        canShoot = true;

		player.UpdateAmmoCounter ();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(nextShot > 0)
        {
            nextShot -= Time.deltaTime;
        }
        else if(fireType == FireType.BLAST && blastQueue > 0)
        {
            Shoot();
            blastQueue--;
        }

        if(nextBlast > 0)
        {
            nextBlast -= Time.deltaTime;
        }
	}

    #endregion


    #region Methods

    public void Shoot()
    {
        
            if (ammoInMagazine > 0 && animator.GetBool("Is Reloading") == false)
            {
                if (nextShot <= 0 && nextBlast <= 0 && canShoot)
                {
                Ray ray = new Ray(barrel.position, transform.rotation * Vector3.left);
                Debug.DrawRay(barrel.position, transform.rotation * Vector3.left  * 20, Color.red, 2);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, layer))
                {
                    Zombie zombie = hit.collider.GetComponentInParent<Zombie>();
                    if (zombie/*hit.collider.tag == "Zombie"*/)
                    {
                        if (hit.collider.tag == "Zombie")
                        {
                            if (hit.distance <= maxDamageDistance)
                            {
                                zombie.TakeDamage(maxDamage, player);
                            }
                            else if (hit.distance > maxDamageDistance && hit.distance < minDamageDistance)
                            {
                                float deltaDistance = minDamageDistance - maxDamageDistance;
                                float distancePerc = deltaDistance / 100f * hit.distance;
                                zombie.TakeDamage(Mathf.RoundToInt(Mathf.Lerp(maxDamage, minDamage, distancePerc)), player);
                            }
                            else
                            {
                                zombie.TakeDamage(minDamage, player);
                            }
                        }
                        else if(hit.collider.tag == "Zombie Head")
                        {
                            zombie.TakeDamage(maxDamage * 3, player);
                        }
                    }
                    else if(hit.collider.tag == "Wall")
                    {
                        //Instantiate(bulletHoles[UnityEngine.Random.Range(0, bulletHoles.Length - 1)], hit.point, hit.collider.transform.rotation);
                    }
                    //Debug.Log("Hit " + hit.transform.name);
                }

                ammoInMagazine--;
                nextShot = shotTime;

                animator.SetTrigger("Shot");
                muzzleFlash.Play();
                sound.clip = shootClip;
                sound.Play();
            }
        }
    }

    public void Reload()
    {
        int neededAmmo = magazineSize - ammoInMagazine;

        if(ammoReserve >= neededAmmo)
        {
            ammoReserve -= neededAmmo;
            ammoInMagazine += neededAmmo;
        }
        else if(ammoReserve < neededAmmo)
        {
            neededAmmo = ammoReserve;
            ammoReserve = 0;
            ammoInMagazine += neededAmmo;
        }
        player.UpdateAmmoCounter();
    }

    public void IsReloading()
    {
        animator.SetBool("Is Reloading", true);
    }

    public void IsNotReloading()
    {
        animator.SetBool("Is Reloading", false);
    }

    public void EnableShoot()
    {
        canShoot = true;
    }

    public void DisableShoot()
    {
        canShoot = false;
    }

    #endregion

}
