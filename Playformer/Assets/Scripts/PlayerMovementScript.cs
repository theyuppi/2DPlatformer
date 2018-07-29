using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementScript : MonoBehaviour
{
    public float Speed = 6.0f;
    private Rigidbody2D m_Rigidbody2D;
    public bool grounded = false;
    public bool climbing = false;
    public int player = 1;
    private const float jump_time_delay = 0.5f;
    public float jump_timer = jump_time_delay;
    private bool key_jump_trig = false;
    public bool face_right = true;
    private const float climb_time_delay = 0.5f;
    public float climb_timer = climb_time_delay;

    public float debug_jump_impulse = 5.0f;
    public float debug_jump_force = 10.0f;
    public float debug_jump_time = 0.17f;
    public float debug_move_speed = 1.0f;

    public ParticleSystem pSystem;
    public GameObject trailPrefab;

    private float trailInterval = 4.0f;
    private float currTrailInterval = 4.0f;

    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        //SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        //renderer.material.SetTextureOffset("_MainTex", new Vector2(10, 10));


        switch (player)
        {
            case 1:
                //renderer.material.color = new Color(255, 0, 0);
                break;
            case 2:
                //renderer.material.color = new Color(0, 0, 255);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        float valX = Input.GetAxis("Horizontal" + player);

        if (valX == 0)
        {
            if (player == 1)
            {
                if (Input.GetKey(KeyCode.A))
                    valX = -1.0f;
                else if (Input.GetKey(KeyCode.D))
                    valX = 1.0f;
            }
            if (player == 2)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    valX = -1.0f;
                else if (Input.GetKey(KeyCode.RightArrow))
                    valX = 1.0f;
            }

        }
        m_Rigidbody2D.velocity = new Vector2(Speed * valX * debug_move_speed, m_Rigidbody2D.velocity.y);



        if (valX > 0 && !face_right)
        {
            face_right = true;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        if (valX < 0 && face_right)
        {
            face_right = false;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        //jump trigger
        if (!Input.GetButton("Jump" + player))
            key_jump_trig = false;
    }

    void FixedUpdate()
    {
        //jump timer delay
        if (jump_timer > 0) jump_timer -= Time.fixedDeltaTime;
        if (climb_timer > 0) climb_timer -= Time.fixedDeltaTime;

        bool jumping = Input.GetButton("Jump" + player);
        if (!jumping)
        {
            if (player == 1 && Input.GetKey(KeyCode.LeftControl)) jumping = true;
            if (player == 2 && Input.GetKey(KeyCode.Keypad0)) jumping = true;
        }

        if (climbing) jumping = false;

        //continue jump force
        if (jumping && jump_timer >= 0)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, debug_jump_force);
        }

        //start jump from ground
        if (jumping && grounded && !key_jump_trig)
        {
            m_Rigidbody2D.AddForce(new Vector2(0.0f, debug_jump_impulse), ForceMode2D.Impulse);
            //m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Mathf.Clamp(m_Rigidbody2D.velocity.y, -5.0f, 5.0f));
            grounded = false;
            SpawnParticles();
            //reset timer for jump control
            jump_timer = debug_jump_time;
        }

        //set key trigger flag
        if (jumping) key_jump_trig = true;



        if (currTrailInterval <= 0)
        {
            //AddTrail();
            currTrailInterval = trailInterval;
        }
        currTrailInterval -= 1;
    }

    public void End_climb()
    {
        climb_timer = climb_time_delay;
    }

    private void SpawnParticles()
    {
        //Vector2 position = new Vector2(transform.position.x, transform.position.y - 0.3f);

        //ParticleSystem ps = GameObject.Instantiate(pSystem, position, pSystem.transform.rotation) as ParticleSystem;
        //ps.Play();
    }

    void AddTrail()
    {
        GameObject trailInstance = GameObject.Instantiate(trailPrefab, transform.position, transform.rotation) as GameObject;
        //trailInstance.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        trailInstance.transform.localScale = transform.localScale;
        switch (player)
        {
            case 1:
                trailInstance.GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.blue);
                trailInstance.GetComponent<SpriteRenderer>().material.SetFloat("_HueShift", 752.9f);
                break;
            case 2:
                trailInstance.GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.red);
                trailInstance.GetComponent<SpriteRenderer>().material.SetFloat("_HueShift", 674.2f);
                break;
            default:
                break;
        }
    }
}