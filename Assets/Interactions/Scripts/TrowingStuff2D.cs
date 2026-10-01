using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrowingStuff2D : MonoBehaviour
{
    public GameObject throwablePrefab;

    public float throwForce = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            ThrowObject();
        }
    }

    void ThrowObject()
    {
        GameObject thrownObject = Instantiate(throwablePrefab, gameObject.transform.position, gameObject.transform.rotation);
        Rigidbody2D rb = thrownObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(gameObject.transform.right * throwForce, ForceMode2D.Impulse);
        }
    }
}
