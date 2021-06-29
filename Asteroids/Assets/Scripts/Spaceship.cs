using UnityEngine;
using static Utils.AsteroidsUtils;

[RequireComponent(typeof(Rigidbody2D))]
public class Spaceship : MonoBehaviour
{
    public float speed = 10f;
    public float shootCooldown = 100f;
    public float bulletSpeed = 400f;
    [Range(1, 10)]
    public int accuracy = 1;

    public bool moveToRight = true;
    public float changeLineCooldown = 10f;
    public float timeToChangeLine = 0.5f;
    public float reactivateCooldown = 5f;

    public GameObject bullet;
    public Transform shootingPointDown;

    private GameController _gameController;
    private float _changeLineTimer = 0f;
    private float _shootCooldown = 0f;
    private float _changeLineCooldown = 0f;
    private Vector3 _changeLineDirection = Vector3.up;

    private void Start()
    {
        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            Debug.LogError("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    private void Update()
    {
        _shootCooldown += Time.deltaTime;
        if (_shootCooldown >= shootCooldown)
        {
            _shootCooldown = 0;
            Shoot();
        }

        _changeLineCooldown += Time.deltaTime;
        if (_changeLineCooldown >= changeLineCooldown)
        {
            _changeLineCooldown = 0;
            ChangeLine();
        }


        if (transform.position.x > GetWorldPositionOfBorder(Border.right).x || transform.position.x < GetWorldPositionOfBorder(Border.left).x)
        {
            gameObject.SetActive(false);
            moveToRight = !moveToRight;
            Invoke(nameof(Reactivate), reactivateCooldown);
        }

        var direction = (moveToRight ? Vector3.right : Vector3.left);
        if (_changeLineTimer > 0f)
        {
            direction += _changeLineDirection;
            _changeLineTimer -= Time.deltaTime;
        }
        transform.position += direction * speed * Time.deltaTime;
    }

    private void ChangeLine()
	{
        _changeLineTimer = timeToChangeLine;
        _changeLineDirection *= RandomSign();
    }

    private void Reactivate()
	{
        gameObject.SetActive(true);
	}

    private void Shoot()
	{
        var currentAccuracy = Random.Range(0, 10);
        var playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (currentAccuracy < accuracy)
            playerGameObject = null;

        var targetPosition = playerGameObject != null ? playerGameObject.transform.position : GetRandomPointInsideBorder();
        var startPosition = shootingPointDown.position;
        var goBullet = Instantiate(bullet,
                                       startPosition,
                                       transform.rotation);

        goBullet.GetComponent<Rigidbody2D>().AddForce((targetPosition - startPosition).normalized * bulletSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy Bullet"))
            return;

        _gameController.IncrementScore(gameObject.tag);

        Destroy(gameObject);
    }
}
