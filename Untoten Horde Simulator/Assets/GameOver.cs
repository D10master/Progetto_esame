using UnityEngine;

public class GameOver : MonoBehaviour
{
	public float maxHeight;

	void Start()
	{
		GetComponent<Rigidbody> ().useGravity = false;
	}

	void Update ()
	{
		if(transform.position.y < maxHeight)
		transform.position += Vector3.up * Time.deltaTime * 1.5f;
	}
}
