using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RyanKHawkinsController : MonoBehaviour
{
    public float speed = 3.0f;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    SpriteRenderer fishing;
    Vector2 lookDirection = new Vector2(1, 0);

    bool hasFishingRod = false;
    bool isWater = false;
    bool isFishing = false;
    bool isCatching = false;
    Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        fishing = GetComponent<SpriteRenderer>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFishing && !isCatching)
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

        for (int i = 0; i < inventory.slots.Length; i++)
        {
            //Iterate through the slots to see if the FishingRod tag is present on an object
            if (inventory.slots[i] != null && inventory.slots[i].transform != null)
            {
                if (inventory.slots[i].transform.childCount > 0 && inventory.slots[i].transform.GetChild(0).gameObject != null)
                {
                    var child = inventory.slots[i].transform.GetChild(0).gameObject.CompareTag("FishingRod");
                    hasFishingRod = true;
                    
                    break;
                }
            }
        }

        if (!isCatching && hasFishingRod && isWater && Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Fishing");
            isFishing = isFishing ? false : true;
        }

        if (isFishing && hasFishingRod && isWater && Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger("Catching");
            isCatching = isCatching ? false : true;
            isFishing = isCatching;

            if (!isCatching)
            {
                //Switch to Battle Scene
                SceneManager.LoadSceneAsync("BattleScene");
            }
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

    void OnCollisionEnter2D(Collision2D other)
    {
        isWater = other.gameObject.CompareTag("Water");
        Debug.Log("isWater = " + isWater);
    }

    void OnCollisionExit2D(Collision2D other)
    {
        isWater = false;
        Debug.Log("isWater = " + isWater);
    }
}
