using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarsStudy : MonoBehaviour {

	public TGCConnectionController controller;
	private int gammaL1;
	public Image fillGammaL, fillGammaH, fillAttention;
	private int gammaH1;
	private int attention1;
	public Slider gammaLBar;
	public Slider gammaHBar;
	public Slider AttBar;
	private float t;
	private float startAtt, finishAtt;
	private float startGammaL, finishGammaL;
	private float startGammaH, finishGammaH;

	void Start () {
		t = 0;

		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();

		controller.UpdateLowGammaEvent += OnUpdateLowGamma;
		controller.UpdateLowGammaEvent += OnUpdateHighGamma;

		controller.UpdateAttentionEvent += OnUpdateAttention;
	}



	void Update(){
		float valueGammaL ;
		float valueGammaH ;
		float valueAtt;

		if (Demo.demo == false) {
			valueGammaL = PropLowGamma(gammaL1);
			valueGammaH = PropHighGamma(gammaH1);
			valueAtt = 1f * attention1 / 100f;
		} else {
			valueGammaL = Random.Range (0f, 1f);
			valueGammaH = Random.Range (0f, 1f);
			valueAtt = Random.Range (0f, 1f);
		}

		t += Time.deltaTime;
		if (t >= 1) {
			t = 0;
			startGammaL = gammaLBar.value;
			startGammaH = gammaHBar.value;
			startAtt = AttBar.value;
			finishGammaL = valueGammaL;
			finishGammaH = valueGammaH;
			finishAtt = valueAtt;
		}

		fillGammaL.color = Color.HSVToRGB(gammaLBar.value,0.7f,1f);
		fillGammaH.color = Color.HSVToRGB(gammaHBar.value,0.7f,1f);
		fillAttention.color = Color.HSVToRGB(AttBar.value,0.7f,1f);


		float newScale= Mathf.Lerp(startGammaL, finishGammaL, t);
		gammaLBar.value = newScale;

		float newScale1= Mathf.Lerp(startGammaH, finishGammaH, t);
		gammaHBar.value = newScale1;

		float newScale2= Mathf.Lerp(startAtt, finishAtt, t);
		AttBar.value = newScale2;

	}

	void OnUpdateLowGamma(int value){
		gammaL1 = value;
	}

	void OnUpdateHighGamma(int value){
		gammaH1 = value;
	}

	void OnUpdateAttention(int value){
		attention1 = value;
	}

	public float PropLowGamma(int value){
		return (1f*(float)value / 72310f);
	}

	public float PropHighGamma(int value){
		return (1f*(float)value / 36068f);
	}

}
