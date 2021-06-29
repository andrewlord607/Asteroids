using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Spaceship : MonoBehaviour
{
    public bool moveToRight = true;

    public float speed = 10f;

    public float shootCooldown = 1f;

    public GameObject bullet;
    public Transform shootingPoint;

    public float bulletSpeed = 400f;

    [Range(1, 10)]
    public int accuracy = 1;

    public float changeLineCooldown = 10f;
    public float timeToChangeLine = 0.5f;

    private GameController _gameController;
    private float _shootCooldown = 0f;
    private float _changeLineCooldown = 0f;
    private float _changeLineTimer = 0f;
    private Vector3 _changeLineDirection = Vector3.up;
    // Start is called before the first frame update
    void Start()
    {
        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            Debug.LogError("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        var direction = (moveToRight ? Vector3.right : Vector3.left);
        if (_changeLineTimer > 0)
            direction += _changeLineDirection;
        transform.position += direction * speed * Time.deltaTime;

        _shootCooldown += Time.deltaTime;
        if(_shootCooldown >= shootCooldown)
		{
            _shootCooldown = 0;
            Shoot();
		}

        _changeLineCooldown += Time.deltaTime;
        if(_changeLineCooldown >= changeLineCooldown)
		{
            _changeLineCooldown = 0;
            _changeLineTimer = timeToChangeLine;
            _changeLineDirection *= (Random.value < 0.5f ? -1f : 1f);
        }

        if (_changeLineTimer > 0f)
            _changeLineTimer -= Time.deltaTime;


        if (transform.position.x > 9 || transform.position.x < -9)
        {
            gameObject.SetActive(false);
            moveToRight = !moveToRight;
            Invoke(nameof(Reactivate), 5);
        }
    }

    void Reactivate()
	{
        gameObject.SetActive(true);
	}

    void Shoot()
	{
        var currentAccuracy = Random.Range(0, 10);
        if(currentAccuracy < accuracy)
		{
            var playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if(playerGameObject != null)
			{
                // Spawn a bullet
                var goBullet = Instantiate(bullet,
                                           shootingPoint.position,
                                           transform.rotation);

                // Push the bullet in the direction it is facing
                goBullet.GetComponent<Rigidbody2D>().AddForce( (playerGameObject.transform.position - shootingPoint.position).normalized * bulletSpeed);
            }
		}
        else
		{
            // Spawn a bullet
            var goBullet = Instantiate(bullet,
                                       shootingPoint.position,
                                       transform.rotation);

            // Push the bullet in the direction it is facing
            goBullet.GetComponent<Rigidbody2D>().AddForce(Vector2.down * bulletSpeed);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _gameController.IncrementScore(gameObject.tag);

        Destroy(gameObject);
    }
}
