using UnityEngine;

public class Door : MonoBehaviour
{

    private Animator animator;

    public int cost;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
	}


    public void Open(Player player)
    {
        animator.SetTrigger("Open");
    }
}
