using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverButtonBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private Animator anim;
	public GameObject text;

	void Start () {
		anim = GetComponent<Animator> ();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		anim.SetTrigger ("JumpButton");
		text.SetActive (true);

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		anim.SetTrigger ("DownButton");
		text.SetActive (false);

	}
}

