using UnityEngine;
using UnityEngine.AI;

public class RandomWalker : MonoBehaviour
{

    public Transform a;
    public Transform b;

    private NavMeshAgent nav;
    private Animator anim;
    private Vector3 target;

	// Use this for initialization
	void Start ()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        transform.position = RandomVector();
        target = RandomVector();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(Vector3.Distance(transform.position, target) < 1)
        {
            target = RandomVector();
        }
        else
        {
            nav.SetDestination(target);
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
            {               
                anim.SetTrigger("Walk");
            }

        }
	}

    private Vector3 RandomVector()
    {
        return new Vector3(Random.Range(a.position.x, b.position.x), Random.Range(a.position.y, b.position.y), Random.Range(a.position.z, b.position.z));
    }
}
