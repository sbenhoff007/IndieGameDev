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
    bool left = false;

    bool isFishing = false;

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
        //isFishing = animator.GetBool("Fishing");

        if (!isFishing)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Vector2 move = new Vector2(horizontal, vertical);

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                left = move.x < 0f ? true : false;
                Debug.Log("Left = " + left);

                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Fishing");
            isFishing = isFishing ? false : true;
            Debug.Log("F key pressed, isFishing = " + isFishing);
        }

        if (isFishing)
        {
            Debug.Log("Fishing = true!");            

            if (left)
            {
                Debug.Log("Left = true");
                //fishing.size = new Vector2(-1, 1);
                fishing.flipX = true;
            }
            else
            {
                //fishing.size = new Vector2(1, 1);
                fishing.flipX = false;
            }
        }
    }

    void FixedUpdate()
    {
        //if (animator.GetBool("Fishing") == true)
        //{
        //    //Stop all movement
        //}
        //else
        //{
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * horizontal * Time.deltaTime;
            position.y = position.y + speed * vertical * Time.deltaTime;

            rigidbody2d.MovePosition(position);
        //}
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
