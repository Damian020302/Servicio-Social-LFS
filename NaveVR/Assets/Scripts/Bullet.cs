using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //public Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        rb = GetComponent<Rigidbody>();
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //GameManager.Instance.ScorePoints(10);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
