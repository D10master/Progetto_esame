using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuceMeditazione : MonoBehaviour {

	public TGCConnectionController controller;
	private int meditation1;
	private float t;
	private float start, finish,start1, finish1;
	public Light luceMed;
	public Slider MedBar;

	void Start () {
		t = 0;
		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
		controller.UpdateMeditationEvent += OnUpdateMeditation;

	}
	void Update(){
		float valueMed,valueMedBar;
		if (Demo.demo == false) {
			valueMed = (29.5f * meditation1 / 100f)+0.5f;
			valueMedBar = 1f * meditation1 / 100f;
		} else {    // valueMed-0.5 / 2.5 = 0 / 1
			valueMed = Random.Range (0.5f, 30f);
			valueMedBar = (valueMed-0.5f)/29.5f;
		}

		t += Time.deltaTime;
		if (t >= 1) {
			t = 0;
			start = luceMed.intensity;
			finish = valueMed;
			start1 = MedBar.value;
			finish1 = valueMedBar;
		}

		float newScale= Mathf.Lerp(start, finish, t);
		luceMed.intensity = newScale;
		float newScale1= Mathf.Lerp(start1, finish1, t);
		MedBar.value = newScale1;


	}
	void OnUpdateMeditation(int value){
		meditation1 = value;
	}
}
