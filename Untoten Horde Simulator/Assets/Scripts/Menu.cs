using UnityEngine;

public class Menu : MonoBehaviour
{

	void Update()
	{
		if (Input.GetButtonDown ("Escape"))
		{
			Application.Quit ();
		}
	}

    public void Play()
    {
        Application.LoadLevel(1);
    }
}
