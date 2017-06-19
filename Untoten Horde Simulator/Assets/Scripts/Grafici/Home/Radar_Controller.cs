using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Radar_Controller : MonoBehaviour {


	public TGCConnectionController controller;
	public UIPolygon[] radar;
	private int lowalpha;
	private int highalpha;
	private int lowbeta;
	private int highbeta;
	private int lowgamma;
	private int highgamma;
	private int theta;
	private int delta;
	private  float[] startAtt, finishAtt,value;
	private float t,perc;
	private float delay = 0.15f;
	private float nextFarme;

	// Use this for initialization
	void Start () {
		nextFarme = delay;
		t = 0;
		startAtt = new float[9];
		finishAtt = new float[9];
		value = new float[9];
		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
		Debug.Log ("Controller Found!");
	
		controller.UpdateLowAlphaEvent += OnUpdateLowAlpha;
		controller.UpdateHighAlphaEvent += OnUpdateHighAlpha;

		controller.UpdateLowBetaEvent += OnUpdateLowBeta;
		controller.UpdateHighBetaEvent += OnUpdateHighBeta;

		controller.UpdateLowGammaEvent += OnUpdateLowGamma;
		controller.UpdateHighGammaEvent += OnUpdateHighGamma;

		controller.UpdateThetaEvent += OnUpdateTheta;

		controller.UpdateDeltaEvent += OnUpdateDelta;
		for (int j = 0; j < radar.Length; j++)
		{
			for (int i = 0; i < 9; i++)
			{
				radar [j].VerticesDistances [i] = 0.3f;
			}
			radar[j].SetVerticesDirty ();
		}
		Debug.Log ("Start() Done!");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Demo.demo == false) {

			if (PropLowGamma (lowgamma) <= 1f) {
				value [0] = PropLowGamma (lowgamma);
			} else
				value [0] = 1f;

			if (PropHighBeta (highbeta) <= 1f) {
				value [1] = PropHighBeta (highbeta);
			} else
				value [1] = 1f;
		
			if (PropLowBeta (lowbeta) <= 1f) {
				value [2] = PropLowBeta (lowbeta);
			} else
				value [2] = 1f;

			if (PropHighAlpha (highalpha) <= 1f) {
				value [3] = PropHighAlpha (highalpha);
			} else
				value [3] = 1f;

			if (PropLowAlpha (lowalpha) <= 1f) {
				value [4] = PropLowAlpha (lowalpha);
			} else
				value [4] = 1f;

			if (PropTheta (theta) <= 1f) {
				value [5] = PropTheta (theta);
			} else
				value [5] = 1f;

			if (PropDelta (delta) <= 1f) {
				value [6] = PropDelta (delta);
			} else
				value [6] = 1f;

			if (PropHighGamma (highgamma) <= 1f) {
				value [7] = PropHighGamma (highgamma);
			} else
				value [7] = 1f;

			if (PropLowGamma (lowgamma) <= 1f) {
				value [8] = PropLowGamma (lowgamma);
			} else
				value [8] = 1f;

		} else {
		
			value [0] = Random.Range (0.3f,1f);
			value [1] = Random.Range (0.3f,1f);
			value [2] = Random.Range (0.3f,1f);
			value [3] = Random.Range (0.3f,1f);
			value [4] = Random.Range (0.3f,1f);
			value [5] = Random.Range (0.3f,1f);
			value [6] = Random.Range (0.3f,1f);
			value [7] = Random.Range (0.3f,1f);
			value [8] = Random.Range (0.3f,1f);
		
		}
		t += Time.deltaTime;
		if (t >= 1.2) {
			t = 0;
			for (int i = 0; i < 9; i++) {
				startAtt[i] = radar[radar.Length-1].VerticesDistances [i];
				finishAtt[i] = value[i];
			}
		}
			

		perc = t/1.2f;

	

		if (nextFarme > 0f) {
			nextFarme -= Time.deltaTime;
		} else {
			nextFarme = delay;
			for (int j = 0; j < radar.Length - 1; j++) {
				for (int i = 0; i < 9; i++) {
					radar [j].VerticesDistances [i] = radar [j + 1].VerticesDistances [i];
				}

			}
		}

		for (int i = 0; i < 9; i++)
		{
			float newScale = Mathf.Lerp (startAtt [i], finishAtt [i], perc);
			radar[radar.Length-1].VerticesDistances [i] = newScale;
			//radar[0].SetVerticesDirty();
		}

		for (int i = 0; i < radar.Length; i++)
		{
			radar[i].SetVerticesDirty();
		}

	}

	void OnUpdateLowAlpha(int value){
		lowalpha = value;
		Debug.Log ("OnUpdateLowAlpha() Done!");
	}
	void OnUpdateHighAlpha(int value){
		highalpha = value;
		Debug.Log ("OnUpdateHighAlpha() Done!");
	}
	void OnUpdateLowBeta(int value){
		lowbeta = value;
		Debug.Log ("OnUpdateLowBeta() Done!");
	}
	void OnUpdateHighBeta(int value){
		highbeta = value;
		Debug.Log ("OnUpdateLowBeta() Done!");
	}
	void OnUpdateLowGamma(int value){
		lowgamma = value;
		Debug.Log ("OnUpdateLowAlpha() Done!");
	}
	void OnUpdateHighGamma(int value){
		highgamma = value;
		Debug.Log ("OnUpdateLowAlpha() Done!");
	}
	void OnUpdateDelta(int value){
		delta = value;
		Debug.Log ("OnUpdateDelta() Done!");
	}
	void OnUpdateTheta(int value){
		theta = value;
		Debug.Log ("OnUpdateTheta() Done!");
	}

	public float PropLowAlpha(int value){
		return (0.7f*(float)value / 230054f)+0.3f;
	}

	public float PropHighAlpha(int value){
		return (0.7f*(float)value / 416442f)+0.3f;
	}

	public float PropLowBeta(int value){
		return (0.7f*(float)value / 453050f)+0.3f;
	}

	public float PropHighBeta(int value){
		return (0.7f*(float)value / 304638f)+0.3f;
	}

	public float PropLowGamma(int value){
		return (0.7f*(float)value / 72310f)+0.3f;
	}

	public float PropHighGamma(int value){
		return (0.7f*(float)value / 36068f)+0.3f;
	}

	public float PropDelta(int value){
		return (0.7f*(float)value / 3381218f)+0.3f;
	}

	public float PropTheta(int value){
		return (0.7f* (float)value /1996463f)+0.3f;
	}
}
