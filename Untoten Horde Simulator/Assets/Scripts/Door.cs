using UnityEngine;

public class Door : MonoBehaviour
{

    private Animator animator;

    public int openCost;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
	}


    public void Open()
    {
        animator.SetTrigger("Open");
    }
}
