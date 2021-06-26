using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float minSpeed = 50f;
    public float maxSpeed = 150f;

    public float minAngularVelocity = 0f;
    public float maxAngularVelocity = 90f;

    public GameObject childAsteroid;

    private GameController _gameController;

    // Start is called before the first frame update
    void Start()
    {
        // Push the asteroid in the direction it is facing
        GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range( minSpeed, maxSpeed));

        // Give a random angular velocity/rotation
        GetComponent<Rigidbody2D>().angularVelocity = Random.Range(minAngularVelocity, maxAngularVelocity);

        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            Debug.LogError("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Bullet"))
            return;

        Destroy(collision.gameObject);

        Split();

        //playsound
        _gameController.IncrementScore();

        Destroy(gameObject);
    }

    private void Split()
	{
        if (CompareTag("Small Asteroid"))
		{
            _gameController.DecrementAsteroids();
            return;
		}

        // Spawn small asteroids
        Instantiate(childAsteroid,
					new Vector3(transform.position.x - .5f, transform.position.y - .5f, 0),
					Quaternion.Euler(0, 0, Random.Range(0,90)));

        // Spawn small asteroids
        Instantiate(childAsteroid,
                    new Vector3(transform.position.x + .5f,transform.position.y - .5f, 0),
                    Quaternion.Euler(0, 0, Random.Range(180, 270)));

        _gameController.SplitAsteroid();
    }
}
