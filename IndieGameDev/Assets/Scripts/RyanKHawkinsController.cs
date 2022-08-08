using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RyanKHawkinsController : MonoBehaviour
{
    public float speed = 3.0f;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    SpriteRenderer fishing;
    Vector2 lookDirection = new Vector2(1, 0);
    
    bool isFishing = false;
    bool isCatching = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        fishing = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFishing)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Vector2 move = new Vector2(horizontal, vertical);

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isCatching)
            {
                animator.SetTrigger("Fishing");
                isFishing = animator.GetBool("Fishing");
                Debug.Log("F key pressed, isFishing = " + isFishing);
            }
        }

        if (isFishing && Input.GetKeyDown(KeyCode.C))
        {
           if (isCatching)
            {
                animator.ResetTrigger("Fishing");
                isFishing = animator.GetBool("Fishing"); 
                animator.ResetTrigger("Catching");
                isCatching = animator.GetBool("Catching"); 
                Debug.Log("Catching mode exit: " + isCatching + " Fishing = " + isFishing);
            }
            else
            {
                animator.SetTrigger("Catching");
                isCatching = animator.GetBool("Catching"); 
                Debug.Log("Setting the Catching trigger: " + isCatching);
            }

            Debug.Log("C key pressed, isCatching = " + isCatching);
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
