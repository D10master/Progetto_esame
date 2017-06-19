using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarsSonno : MonoBehaviour {

	public TGCConnectionController controller;
	public Image thetaFill, deltaFill; 
	private int theta1;
	private int delta1;
	public Slider thetaBar;
	public Slider deltaBar;
	private float t;
	private float startDelta, finishDelta;
	private float startTheta, finishTheta;

	void Start () {
		t = 0;
		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
		controller.UpdateThetaEvent += OnUpdateTheta;
		controller.UpdateDeltaEvent += OnUpdateDelta;
	}



	void Update(){
		float valueTheta ;
		float valueDelta;

		if (Demo.demo == false) {
			valueTheta = PropTheta(theta1);
			valueDelta = PropDelta(delta1);
		} else {
			valueTheta = Random.Range (0f, 1f);
			valueDelta = Random.Range (0f, 1f);
		}
		t += Time.deltaTime;
		if (t >= 1) {
			t = 0;
			startTheta = thetaBar.value;
			startDelta = deltaBar.value;
			finishTheta = valueTheta;
			finishDelta = valueDelta;
		}

		deltaFill.color = Color.HSVToRGB(deltaBar.value,0.7f,1f);
		thetaFill.color = Color.HSVToRGB(thetaBar.value,0.7f,1f);

		float newScale= Mathf.Lerp(startTheta, finishTheta, t);
		thetaBar.value = newScale;

		float newScale1= Mathf.Lerp(startDelta, finishDelta, t);
		deltaBar.value = newScale1;

	}

	void OnUpdateTheta(int value){
		theta1 = value;
	}
	void OnUpdateDelta(int value){
		delta1 = value;
	}
	public float PropDelta(int value){
		return (1f*(float)value / 3381218f);
	}

	public float PropTheta(int value){
		return (1f* (float)value /1996463f);
	}

}
