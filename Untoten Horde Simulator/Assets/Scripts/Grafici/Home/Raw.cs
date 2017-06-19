using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raw : MonoBehaviour {


	public TGCConnectionController controller;
	private int raw;
	private float t;
	private float perc;
	private float start, finish;
	public Transform point;

	// Use this for initialization
	void Start () {
		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
		controller.UpdateRawdataEvent += OnUpdateRawdata;
	}
	
	// Update is called once per frame
	void Update () {
		t += Time.deltaTime;
	
		if (t >= 0.2f) {
			t = 0;
			if (Demo.demo == true) {
				raw=Random.Range(-2048, 2048);	
			}
			start = point.position.y;
			finish = propRaw (raw);
		}

		perc = t/0.2f;
		//float newPos= Mathf.Lerp(start, finish, t);

		point.position=new Vector3(point.position.x, Mathf.Lerp(start,finish,perc), point.position.z);

	}


	void OnUpdateRawdata(int value){
		raw = value;
	}

	private float propRaw(int value){
		return (((float)value + 2048f) * 320f / 4096f) + 600f;
	}
}
