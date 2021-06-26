using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float impulseForce = 3f;
    public float rotationSpeed = 100.0f;
    public float bulletSpeed = 400f;

    public GameObject bullet;
	
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        transform.Rotate(0, 0, -horizontalMovement * rotationSpeed * Time.deltaTime);

        float verticalMovement = Input.GetAxisRaw("Vertical");
        if (verticalMovement > 0 && horizontalMovement == 0)
            GetComponent<Rigidbody2D>().AddForce(transform.up * impulseForce * verticalMovement);

        // Has a bullet been fired
        if (Input.GetButtonDown("Fire"))
            Shoot();
    }
    void Shoot()
    {
        // Spawn a bullet
        var goBullet = Instantiate(bullet,
					new Vector3(transform.position.x, transform.position.y, 0),
					transform.rotation);

        // Push the bullet in the direction it is facing
        goBullet.GetComponent<Rigidbody2D>().AddForce(goBullet.transform.up * bulletSpeed);

        // Play a shoot sound
        //AudioSource.PlayClipAtPoint(shoot, Camera.main.transform.position);
    }
}
