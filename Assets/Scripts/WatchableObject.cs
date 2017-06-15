using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchableObject : MonoBehaviour
{
    public Transform watchObject;
    private Animator animator;

    void Start()
    {
        animator = watchObject.GetComponent<Animator>();
    }

    // Use this for initialization
    public void OnWatch()
    {
        animator.SetBool("Watched", true);
    }

    // Update is called once per frame
    public void UnWatch()
    {
        animator.SetBool("Watched", false);
    }
}
