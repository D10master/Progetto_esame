using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarsHappy : MonoBehaviour {

	public TGCConnectionController controller;
	public Image gammaLFill, deltaFill; 
	private int gammaL1;
	private int delta1;
	public Slider gammaLBar;
	public Slider deltaBar;
	private float t;
	private float startDelta, finishDelta;
	private float startGammaL, finishGammaL;

	void Start () {
		t = 0;

		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();

		controller.UpdateLowGammaEvent += OnUpdateGammaL;
		controller.UpdateDeltaEvent += OnUpdateDelta;
	}



	void Update(){
		float valueGammaL;
		float valueDelta;

		if (Demo.demo == false) {
			valueGammaL = PropLowGamma(gammaL1);
			valueDelta = PropDelta(delta1);
		} else {
			valueGammaL = Random.Range (0f, 1f);
			valueDelta = Random.Range (0f, 1f);
		}


		t += Time.deltaTime;
		if (t >= 1) {
			t = 0;
			startGammaL = gammaLBar.value;
			startDelta = deltaBar.value;
			finishGammaL = valueGammaL;
			finishDelta = valueDelta;
		}

		deltaFill.color = Color.HSVToRGB(deltaBar.value,0.7f,1f);
		gammaLFill.color = Color.HSVToRGB(gammaLBar.value,0.7f,1f);

		float newScale= Mathf.Lerp(startGammaL, finishGammaL, t);
		gammaLBar.value = newScale;

		float newScale1= Mathf.Lerp(startDelta, finishDelta, t);
		deltaBar.value = newScale1;

	}

	void OnUpdateGammaL(int value){
		gammaL1 = value;
	}
	void OnUpdateDelta(int value){
		delta1 = value;
	}
	public float PropDelta(int value){
		return (1f*(float)value / 3381218f);
	}
	public float PropLowGamma(int value){
		return (1f*(float)value / 72310f);
	}
}
