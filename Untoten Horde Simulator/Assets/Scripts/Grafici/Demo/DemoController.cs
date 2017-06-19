using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void change(){
		if (Demo.demo) {
			Demo.demo = false;
		} else
			Demo.demo = true;
	}
}
