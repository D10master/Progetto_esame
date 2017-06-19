using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttMed : MonoBehaviour {

	public TGCConnectionController controller;
	private int attention1;
	private int meditation1;
	public GameObject circleAtt;
	public GameObject circleMed;
	private float t;
	private float startMed, finishMed;
	private float startAtt, finishAtt,perc;

	void Start () {
		t = 0;

		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();

		controller.UpdateAttentionEvent += OnUpdateAttention;
		controller.UpdateMeditationEvent += OnUpdateMeditation;
	}



	void Update(){
		float valueAtt;
		float valueMed;
		if (Demo.demo == false) {
			 valueAtt = 1f * attention1 / 100f;
			 valueMed = 1f * meditation1 / 100f;
		} else {
			valueAtt = Random.Range (0f,1f);
			valueMed = Random.Range (0f,1f);
		}
		t += Time.deltaTime;
		if (t >= 1.2) {
			t = 0;
			startAtt = circleAtt.transform.localScale.x;
			finishAtt = valueAtt;
			startMed = circleMed.transform.localScale.x;
			finishMed = valueMed;
		}
		perc = t / 1.2f;



		float newScale= Mathf.Lerp(startAtt, finishAtt, perc);
		circleAtt.transform.localScale=new Vector3(newScale, newScale, 1);

		float newScale1= Mathf.Lerp(startMed, finishMed, t);
		circleMed.transform.localScale=new Vector3(newScale1, newScale1, 1);

	}

	void OnUpdateAttention(int value){
		attention1 = value;
	}
	void OnUpdateMeditation(int value){
		meditation1 = value;
	}

}
