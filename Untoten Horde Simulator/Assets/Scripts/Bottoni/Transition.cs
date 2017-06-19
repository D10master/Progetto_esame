using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour {

	//public List<GameObject> targets;
	public GameObject container;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

	}

	public void transition(int i){
		if (gameObject.name.Equals ("Welcome_Panel")) {
			gameObject.SetActive (false);
			container.transform.GetChild (8).gameObject.SetActive(true);
		}

		for (int j = 0; j < container.transform.childCount-1; j++) {
			if(!container.transform.GetChild(j).name.Equals("Bar"))
				container.transform.GetChild (j).gameObject.SetActive(false);
		}
		container.transform.GetChild (i).gameObject.SetActive(true);
	}
}
