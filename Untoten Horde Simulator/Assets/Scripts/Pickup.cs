using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	//riferimento al mesh renderer
	public MeshRenderer[] mesh;

	//velocità di rotazione
	public float rotationSpeed;

	//durata sul campo di gioco
	public float durationtime;
	//tempo rimanente prima che scompaia
	private float remainingTime;

	//quando inizia a lampeggiare
	public float blinkStartTime;
	//frequenza minima di lamppeggiamento
	public float blinkMinFrequecy;
	//frequenza massima di lamppeggiamento
	public float blinkMaxfrequency;
	private float nextblink;

	private float lerpPerc;

	// Use this for initialization
	void Start ()
	{
		remainingTime = durationtime;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//rotazione dell'oggetto
		transform.Rotate (new Vector3 (0, rotationSpeed, 0) * Time.deltaTime);

		remainingTime -= Time.deltaTime;

		if (remainingTime <= 0)
		{
			Destroy (gameObject);
		}
		else if (remainingTime <= blinkStartTime)
		{
			/*if (nextblink > 0)
			{
				nextblink -= Time.deltaTime;
			}
			else
			{
				lerpPerc = 0 
				nextblink = Mathf.Lerp(1f/blinkMaxfrequency, 1f/blinkMinFrequecy, lerpPerc);
				SwitchMesh();
			}*/
		}
	}

	private void SwitchMesh()
	{
		for (int i = 0; i < mesh.Length; i++)
		{
			if (mesh[i].enabled)
				mesh[i].enabled = false;
			else
				mesh[i].enabled = true;
		}
	}
}
