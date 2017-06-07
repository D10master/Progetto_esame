using UnityEngine;

public class HitMarker : MonoBehaviour
{

    public Transform playerTransform;
    public float rotation;

    private RectTransform rect;

	// Use this for initialization
	void Start ()
    {
        rect = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        rect.localRotation.SetEulerRotation(0, 0, playerTransform.rotation.y - rotation);
	}
}
