using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarsCreativita : MonoBehaviour {

	public TGCConnectionController controller;
	private int theta1;
	public  Image thetaFill, medFill;
	private int meditation1;
	public Slider thetaBar;
	public Slider MedBar;
	private float t;
	private float startMed, finishMed;
	private float startTheta, finishTheta;

	void Start () {
		t = 0;
		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
		controller.UpdateThetaEvent += OnUpdateTheta;
		controller.UpdateMeditationEvent += OnUpdateMeditation;
	}



	void Update(){
		float valueTheta;
		float valueMed;
		
		if (Demo.demo == false) {
			 valueTheta= PropTheta(theta1);
			 valueMed = 1f * meditation1 / 100f;
		} else {
			 valueTheta = Random.Range (0f, 1f);
			 valueMed = Random.Range (0f, 1f);
		}

		t += Time.deltaTime;
		if (t >= 1) {
			t = 0;
			startTheta = thetaBar.value;
			startMed = MedBar.value;
			finishTheta = valueTheta;
			finishMed = valueMed;
		}


		medFill.color = Color.HSVToRGB(MedBar.value,0.7f,0.85f);
		thetaFill.color = Color.HSVToRGB(thetaBar.value,0.7f,0.85f);


		float newScale= Mathf.Lerp(startTheta, finishTheta, t);
		thetaBar.value = newScale;

		float newScale1= Mathf.Lerp(startMed, finishMed, t);
		MedBar.value = newScale1;

	}

	void OnUpdateTheta(int value){
		theta1 = value;
	}
	void OnUpdateMeditation(int value){
		meditation1 = value;
	}
	public float PropTheta(int value){
		return (1f* (float)value /1996463f);
	}

}
