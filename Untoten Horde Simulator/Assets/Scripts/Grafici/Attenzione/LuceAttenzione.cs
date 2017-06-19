using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LuceAttenzione : MonoBehaviour {

	public TGCConnectionController controller;
	private int attention1;
	private float t;
	private float start, finish,start1, finish1;
	public Light luceAtt;
	public Slider AttBar;

	void Start () {
		t = 0;
		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
		controller.UpdateAttentionEvent += OnUpdateAttention;
	}
	void Update(){
		
		float valueAtt,valueAttBar;
		if (Demo.demo == false) {
			valueAtt = (29.5f * attention1 / 100f)+0.5f;
			valueAttBar = 1f * attention1 / 100f;
		} else {    // valueMed-0.5 / 2.5 = 0 / 1
			valueAtt = Random.Range (0.5f, 30f);
			valueAttBar = (valueAtt-0.5f)/29.5f;
		}

		t += Time.deltaTime;
		if (t >= 1) {
			t = 0;
			start = luceAtt.intensity;
			finish = valueAtt;
			start1 = AttBar.value;
			finish1 = valueAttBar;
		}

		float newScale= Mathf.Lerp(start, finish, t);
		luceAtt.intensity = newScale;
		float newScale1= Mathf.Lerp(start1, finish1, t);
		AttBar.value = newScale1;
	}
	void OnUpdateAttention(int value){
		attention1 = value;
	}
}
