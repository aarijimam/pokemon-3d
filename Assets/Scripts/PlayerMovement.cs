using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = 10f;
    [SerializeField] private float time;
    Animator animator;
    public float sensitivity = 8;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        
        float horizontalInput = Input.GetAxis("Horizontal");
         float verticalInput = Input.GetAxis("Vertical");
         Vector3 movementDirection = new Vector3(horizontalInput , 0 , verticalInput);
         transform.Translate(movementDirection *speed*Time.deltaTime,Space.Self);
         //rb.velocity = movementDirection;
         //movementDirection.Normalize();
        bool isWalking = animator.GetBool("isWalking");

    

        if (Input.GetKey("w") && !isWalking)
        {
            animator.SetBool("isWalking", true);
        
            //transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (!Input.GetKey("w") && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
        if (Input.GetKey("a") && !isWalking)
        {
            animator.SetBool("isWalking", true);
           
            //transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        if (!Input.GetKey("a") && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
        if (Input.GetKey("d") && !isWalking)
        {
            animator.SetBool("isWalking", true);
            
           // transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (!Input.GetKey("d") && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
        if (Input.GetKey("s") && !isWalking)
        {
            animator.SetBool("isWalking", true);
           
            //transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (!Input.GetKey("s") && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("StartMenu");
        }


    }
    void FixedUpdate()
    {
        float rotateHorizontal = Input.GetAxis("Mouse X");
        transform.RotateAround(transform.position, -Vector3.up, -rotateHorizontal * sensitivity); 
        transform.RotateAround(Vector3.zero, transform.right, 0); 
    }


    //[SerializeField] Animator grassAnimator;
    public event Action onEncountered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Grass") && other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            other.gameObject.GetComponent<Animator>().SetBool("isMoving", true);
        }
        if (UnityEngine.Random.Range(1, 101) <= 5)
        {
            Debug.Log("Wild Pokemon booo");
            animator.SetBool("isWalking", false);
            onEncountered();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Grass"))
        {
            other.gameObject.GetComponent<Animator>().SetBool("isMoving", false);

        }
    }

}


