using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    #region Fields

    public float normalSpeed;
    public float aimingSpeed;

    private Camera mainCamera;
    private FirstPersonController fpc;
    private Animator animator;
    private List<Weapon> weapons = new List<Weapon>();
    public Transform weaponSlot;
    public LayerMask activableLayer;
    public Transform weapon;
    public Transform normalWeaponPlaceholder;
    public Transform aimingWeaponPlaceholder;
    private Weapon equippedWeapon;

    //Elementi UI
    public Text ammoCounter;
    public Text pointsCounter;
    public Text actionHint;
    public Image sight;
    public Image hitMarker;

    public int maxHp;
    private int hp;
    private bool isAiming;
    private int points;

    private Vector3 weaponStartingPosition;
    private Vector3 weaponInterpolatePosition;
    public float lerpTime;
    private float currentLerpTime;

    #endregion


    #region UnityMethods

    // Use this for initialization
    void Start ()
    {
        mainCamera = Camera.main;
        fpc = GetComponent<FirstPersonController>();
        animator = GetComponent<Animator>();
        equippedWeapon = weaponSlot.GetComponentInChildren<Weapon>();
        weapon = equippedWeapon.transform;

        hp = maxHp;
        points = 1000;

        Unaim();

        UpdatePointsCounter();
        UpdateAmmoCounter();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0) && (equippedWeapon.fireType == FireType.SEMI_AUTO || equippedWeapon.fireType == FireType.BLAST))
        {
            Shot();
            UpdateAmmoCounter();
        }
        else if (Input.GetMouseButton(0) && equippedWeapon.fireType == FireType.AUTO)
        {
            Shot();
            UpdateAmmoCounter();
        }
        else if (Input.GetButtonDown("Reload") && equippedWeapon.Animator.GetBool("Is Reloading") == false)
        {
            Reload();
        }
        else if(Input.GetButtonDown("Aim"))
        {
            if (isAiming)
            {
                Unaim();
            }
            else
            {
                Aim();
            }
        }
        else if(Input.GetButtonDown("Run") && isAiming)
        {
            if(isAiming)
                Unaim();
        }
        else if(Input.GetButtonDown("Action"))
        {
            Action();
        }
        else if(Input.GetKeyDown(KeyCode.H))
        {
            ShowHitMarker(new Vector2(Random.Range(-3.14f, 3.14f), Random.Range(-3.14f, 3.14f)));
        }

        CheckAction();

        if(currentLerpTime < lerpTime)
        {
            float perc = currentLerpTime / lerpTime;
            weapon.localPosition = Vector3.Lerp(weaponStartingPosition, weaponInterpolatePosition, perc);
            currentLerpTime += Time.deltaTime;
           // Debug.Log("Perc: " + perc);
        }
        else if(currentLerpTime > lerpTime)
        {  
            currentLerpTime = lerpTime;
            float perc = currentLerpTime / lerpTime;
            weapon.localPosition = Vector3.Lerp(weaponStartingPosition, weaponInterpolatePosition, perc);
           // Debug.Log("Perc: " + perc);
        }
	}

    #endregion


    #region Methods

    private void Shot()
    {
        equippedWeapon.Shot();
    }

    private void Reload()
    {
        if (equippedWeapon.AmmoReserve > 0 && equippedWeapon.AmmoInMagazine < equippedWeapon.magazineSize)
        {
            Unaim();
            equippedWeapon.Audio.clip = equippedWeapon.reloadClip;
            equippedWeapon.Audio.Play();
            equippedWeapon.IsReloading();
            equippedWeapon.Animator.SetTrigger("Reload");
        }
    }

    private void Aim()
    {
        if (fpc.IsWalking)
        {
            isAiming = true;
            //weapon.localPosition = equippedWeapon.aimingPosition;
            currentLerpTime = 0;
            weaponStartingPosition = weapon.localPosition;
            weaponInterpolatePosition = equippedWeapon.aimingPosition;
            fpc.WalkSpeed = aimingSpeed;
            sight.enabled = false;
        }
    }

    private void Unaim()
    {
        isAiming = false;
        //weapon.localPosition = equippedWeapon.normalPosition;
        currentLerpTime = 0;
        weaponStartingPosition = weapon.localPosition;
        weaponInterpolatePosition = equippedWeapon.normalPosition;
        fpc.WalkSpeed = normalSpeed;
        sight.enabled = true;
    }

    private void CheckAction()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.rotation * Vector3.forward);
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.rotation * Vector3.forward * Consts.ACTION_DISTANCE, Color.blue, 0.05f);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Consts.ACTION_DISTANCE, activableLayer))
        {
            if (hit.collider.tag == "Door")
            {
                Door door = hit.collider.GetComponent<Door>();
                ShowActionHint("Premi F per aprire la porta [<color=#ffa500ff>" + door.cost + " punti</color>]");
            }
            else if (hit.collider.tag == "BuyableWeapon")
            {
                BuyableWeapon weapon = hit.collider.GetComponent<BuyableWeapon>();
                ShowActionHint("Premi F per comprare [<color=#ffa500ff>" + weapon.cost + " punti</color>]");
            }
        }
        else
        {
            HideActionHint();
        }
    }

    private void Action()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.rotation * Vector3.forward);
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.rotation * Vector3.forward * Consts.ACTION_DISTANCE, Color.green, 3);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Consts.ACTION_DISTANCE, activableLayer))
        {
            if (hit.collider.tag == "Door")
            {
                Door door = hit.collider.GetComponent<Door>();
                if(points >= door.cost)
                {
                    Transform[] newSpawnPoints = door.transform.parent.GetComponent<Room>().spawnPoints;
                    PlayManager gm = GameObject.Find("Game Manager").GetComponent<PlayManager>();

                    ModifyPoints(-door.cost);
                    door.GetComponent<Collider>().enabled = false;

                    for (int i=0; i<newSpawnPoints.Length; i++)
                    {
                        gm.SpawnPoints.Add(newSpawnPoints[i]);
                    }

                    door.enabled = false;
                    door.transform.parent.GetComponent<Animator>().SetTrigger("Open");
                }
            }
            Debug.Log("Attempting action on: " + hit.collider.name);
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Damage:" + damage);
        if (hp <= damage)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector2 direction)
    {
        TakeDamage(damage);
    }

    private void Die()
    {
        hp = 0;
        animator.SetTrigger("Die");
        fpc.enabled = false;
        this.enabled = false;
    }

    public void ModifyPoints(int delta)
    {
        points += delta;
        UpdatePointsCounter();
    }

    #region UI Methods

    public void UpdateAmmoCounter()
    {
        ammoCounter.text = equippedWeapon.AmmoInMagazine + " | " + equippedWeapon.AmmoReserve;
    }

    public void UpdatePointsCounter()
    {
        pointsCounter.text = points.ToString();
    }

    private void ShowActionHint(string hint)
    {
        actionHint.text = hint;
        actionHint.enabled = true;
    }

    private void HideActionHint()
    {
        actionHint.enabled = false;
    }

    public void ShowHitMarker(Vector2 direction)
    {
        hitMarker.enabled = true;
        hitMarker.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x));
    }

    #endregion

    #endregion

}
