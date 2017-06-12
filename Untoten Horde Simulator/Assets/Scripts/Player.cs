﻿using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    #region Fields

	//riferimento allo script che cestisce il gioco
    private GameManager gm;
	//riferimento alla camera di gioco
	private Camera mainCamera;
	//riferimento al controller del giocatore che si occupa di prendere gli input da tastiera
	private FirstPersonController fpc;
	//riferimento all'animatore che si occupa di gestire le animazioni del giocatore
	private Animator animator;
	//private List<Weapon> weapons = new List<Weapon>();
	//oggetto che contiene l'arma impugnata
	public Transform weaponSlot;
	//layer che contiene tutti gli ogetti attivabili
	public LayerMask activableLayer;
	//riferimento all'oggetto arma
	public Transform weapon;
	//riferimento allo script arma
	private Weapon equippedWeapon;

	//velocità di camminata senza che l'arma sia tenuta in modalità mirino
    public float normalSpeed;
	//velocità di camminata con l'arma tenuta in modalità mirino
    public float aimingSpeed;



    //Elementi UI
    public Text ammoCounter;
    public Text pointsCounter;
    public Text actionHint;
    public Image sight;
    public Image hitMarker;
	public Slider hpSlider;
	public Slider armorSlider;

    //hp e rigenerazione
    public int maxHp;
	private int hp;
	public int maxArmor;
	private int armor;
    public float regenerationStartTime;
	public float regenerateEvery;
    private float nextRegeneration;
	public float regenerationAmount;

	//flag che dice se il giocatore sta mirando
    private bool isAiming;
	//punti accumulati dal giocatore con le uccisioni
    private int points;

	//parametri interpolazione arma
    private Vector3 weaponStartingPosition;
    private Vector3 weaponInterpolatePosition;
    public float lerpTime;
    private float currentLerpTime;

    #endregion


    #region UnityMethods

    // Use this for initialization
    void Start ()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        mainCamera = Camera.main;
        fpc = GetComponent<FirstPersonController>();
        animator = GetComponent<Animator>();
        equippedWeapon = weaponSlot.GetComponentInChildren<Weapon>();
        weapon = equippedWeapon.transform;
		armorSlider.minValue = 0;
		armorSlider.maxValue = maxArmor;
		hpSlider.minValue = 0;
		hpSlider.maxValue = maxHp;

        hitMarker.GetComponent<HitMarker>().playerTransform = transform;

        hp = maxHp;
        points = 1000;

        Unaim();

        UpdatePointsCounter();
        UpdateAmmoCounter();
		UpdateArmorUI ();
		UpdateHpUI ();
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
                    GameManager gm = GameObject.Find("Game Manager").GetComponent<GameManager>();

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
		hp -= Consts.CalculateDamage(damage, armor);
		UpdateHpUI ();
        //Debug.Log("Damage:" + damage);
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
		UpdateHpUI ();

        fpc.enabled = false;
        this.enabled = false;
        gm.GameOver();
		animator.SetTrigger("Die");
    }

    public void ModifyPoints(int delta)
    {
        points += delta;
        UpdatePointsCounter();
    }

	public void ModifyArmor(int delta)
	{
		armor += delta;
		armor = (int)Mathf.Clamp (armor, 0, maxArmor);
		UpdateArmorUI ();
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

	private void UpdateArmorUI()
	{
		armorSlider.value = armor;
	}

	private void UpdateHpUI()
	{
		hpSlider.value = hp;
	}

    #endregion

    #endregion

}
