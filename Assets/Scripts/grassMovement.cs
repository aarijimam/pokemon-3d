using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class grassMovement : MonoBehaviour
{
    Animator animator;
    public event Action onEncountered;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1){
            animator.SetBool("isMoving", true);
        }
        if (UnityEngine.Random.Range(1,101) <= 5)
        {
            Debug.Log("Wild Pokemon booo");
            onEncountered();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            animator.SetBool("isMoving", false);

        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
