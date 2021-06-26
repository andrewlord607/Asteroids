using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float impulseForce = 3f;
    public float rotationSpeed = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);

        float movement = Input.GetAxisRaw("Vertical");
        if (movement > 0)
            GetComponent<Rigidbody2D>().AddForce(transform.up * impulseForce * movement);

    }
}
