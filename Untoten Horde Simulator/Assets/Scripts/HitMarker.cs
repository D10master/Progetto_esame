using UnityEngine;

public class HitMarker : MonoBehaviour
{

    public Transform playerTransform;
    public float rotation;

    //private RectTransform rect;

	// Use this for initialization
	void Start ()
    {
        //rect = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		transform.localRotation = Quaternion.Euler(0f, 0f, playerTransform.rotation.eulerAngles.y - rotation);
	}
}
