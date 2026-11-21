using System;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    private GameObject gm;
    private bool scored;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 pos = GetComponent<Transform>().position;
        pos.x -= speed * Time.fixedDeltaTime;
        GetComponent<Transform>().position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!scored && other.CompareTag("Player") && gm && gm.GetComponent<GameManager>().IsPlaying)
        {
            scored = true;
            gm.GetComponent<GameManager>().AddScore(1);
        }
    }
}
