using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AsteroidController : MonoBehaviour
{
    public GameObject childAsteroid;

    public AudioClip destroy;

    public float minSpeed = 50f;
    public float maxSpeed = 150f;

    private GameController _gameController;

    private void Start()
    {
        // Push the asteroid in the direction it is facing
        GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range( minSpeed, maxSpeed));


        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            Debug.LogError("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Bullet") && !collision.gameObject.CompareTag("Enemy Bullet"))
            return;

        Destroy(collision.gameObject);

        Split();

        AudioSource.PlayClipAtPoint(destroy, Camera.main.transform.position);

        _gameController.IncrementScore(gameObject.tag);

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
