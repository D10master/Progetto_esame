using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private Animator anim;

	void Start () {
		anim = GetComponent<Animator> ();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		anim.SetTrigger ("Su");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		anim.SetTrigger ("Giu");
	}
}
