using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class barsMemory : MonoBehaviour {

	public TGCConnectionController controller;
	private int att,med;
	public  Image attFill, medFill;
	public Slider attBar;
	public Slider MedBar;
	private float t;
	private float startMed, finishMed;
	private float startAtt, finishAtt;

	void Start () {
		t = 0;

		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();

		controller.UpdateAttentionEvent += OnUpdateAttention;
		controller.UpdateMeditationEvent += OnUpdateMeditation;
	}



	void Update(){
		float valueAtt ;
		float valueMed;

		if (Demo.demo == false) {
			valueAtt = 1f * att / 100f;
			valueMed = 1f * med / 100f;
		} else {
			valueAtt = Random.Range (0f, 1f);
			valueMed = Random.Range (0f, 1f);
		}


		t += Time.deltaTime;
		if (t >= 1) {
			t = 0;
			startAtt = attBar.value;
			startMed = MedBar.value;
			finishAtt = valueAtt;
			finishMed = valueMed;
		}


		medFill.color = Color.HSVToRGB(MedBar.value,0.7f,1f);
		attFill.color = Color.HSVToRGB(attBar.value,0.7f,1f);


		float newScale= Mathf.Lerp(startAtt, finishAtt, t);
		attBar.value = newScale;

		float newScale1= Mathf.Lerp(startMed, finishMed, t);
		MedBar.value = newScale1;

	}

	void OnUpdateAttention(int value){
		att = value;
	}
	void OnUpdateMeditation(int value){
		med = value;
	}

}
