using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaColor : MonoBehaviour
{
    public Color[] arrayColor = new Color[3];
    private Renderer miRenderer;

    void Start()
    {
        miRenderer = GetComponent<Renderer>();
        StartCoroutine(ChangeColor());
    }

    IEnumerator ChangeColor()
    {
        while(true)
        {
            foreach(Color defaultColor in arrayColor)
            {
                miRenderer.material.color = defaultColor;
                yield return new WaitForSeconds(0.5f);
            }   
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("PickUp"))
        {
            //collision.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}