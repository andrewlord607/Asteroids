using UnityEngine;
using static Utils.AsteroidsUtils;

// �����, ������� ��������� ���������� ������������ ��������
[RequireComponent(typeof(Rigidbody2D))]
public class Spaceship : MonoBehaviour
{
    // �������� �������
    public float speed = 10f;
    // ����� ����������� ��������
    public float shootCooldown = 100f;
    // �������� ��������
    public float bulletSpeed = 400f;
    // �������� ������������
    // 1 � ����� ����� ������������� � ������
    // 10 � ������ ������������� � ������
    [Range(1, 10)]
    public int accuracy = 1;

    // � ����� ������� �������� �������
    public bool moveToRight = true;
    // �����, ������ ������� ������� ������ ���� �����(���������� y)
    public float changeLineCooldown = 10f;
    // �����, ������� ������ �� ����� �����
    public float timeToChangeLine = 0.5f;
    // ����� ������ �� ������� ������� �������������� � ����� ������� ������ ����� ����� ������������
    public float reactivateCooldown = 5f;

    // ������ ���� ������������� �������
    public GameObject bullet;
    // ������� �� ������� ���������� �������
    public Transform shootingPointDown;

    // ������ �� GameController, ������� ��������� ���������� ����
    private GameController _gameController;
    // ������� ��� ������� ������� ����� �����, �������� � �.�.
    private float _changeLineTimer = 0f;
    private float _shootCooldown = 0f;
    private float _changeLineCooldown = 0f;
    // � ����� ������� �������� �����(����� ��� ����)
    private Vector3 _changeLineDirection = Vector3.up;

    private void Start()
    {
        // ������� � �������� �������� GameController � ������� Tag � ���������� ������ �� ����
        // ���� �� ������, �� ����������� ������
        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            throw new System.Exception("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    private void Update()
    {
        // ���� ������ ����� ��������, �� ���������� ����� Shoot
        _shootCooldown += Time.deltaTime;
        if (_shootCooldown >= shootCooldown)
        {
            _shootCooldown = 0;
            Shoot();
        }

        // ���� ������ ����� ����� ������, �� ���������� ����� ChangeLine
        _changeLineCooldown += Time.deltaTime;
        if (_changeLineCooldown >= changeLineCooldown)
        {
            _changeLineCooldown = 0;
            ChangeLine();
        }


        // ���� ������� ������� �� ����� ��� ������ ������� ������, �� �� ��������������
        // ������ ��������� ����� ����� ������������, �� ������ �������� � ��������������� �������
        if (transform.position.x > GetWorldPositionOfBorder(Border.right).x || transform.position.x < GetWorldPositionOfBorder(Border.left).x)
        {
            gameObject.SetActive(false);
            moveToRight = !moveToRight;
            // ������� ������� � ��������������� �������, ����� �� ������ �� ��������� �� ��������
            transform.Translate((moveToRight? Vector2.right: Vector2.left) * 0.2f);
            Invoke(nameof(Reactivate), reactivateCooldown);
        }

        // ����������� �������� ������������ �� ������ ����� moveToRight
        var direction = moveToRight ? Vector3.right : Vector3.left;
        // _changeLineTimer > 0f �� ������ � ���� ������ ���������� ����� �����
        // � ���� ������ ����������� �������� ����� ������ ��������� ����������� ����� �����
        if (_changeLineTimer > 0f)
        {
            direction += _changeLineDirection;
            _changeLineTimer -= Time.deltaTime;
        }
        // ����� ������� ����������� �� ������ ����������� �������� � ��������
        transform.position += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// �����, ������� �������� �� ����� ����� � ������������� �������
    /// </summary>
    private void ChangeLine()
	{
        // ��������� ������ ����� �����
        // ���� �� ������ ���� ����� ����������� ����� �����
        _changeLineTimer = timeToChangeLine;
        // ���������� ����� ����� ������������ ��������� �������
        _changeLineDirection *= RandomSign();
    }

    /// <summary>
    /// �����, ������� ������ ���������� �������
    /// </summary>
    private void Reactivate()
	{
        gameObject.SetActive(true);
	}

    /// <summary>
    /// �����, ������� ���������� �������(������ ����)
    /// </summary>
    private void Shoot()
	{
        // ������ ��������� �������� �� 0 �� 10
        // ���� ��� ������ �������� ������� ������ ���������� ������� �� ������ (playerGameObject �������� ������ �� ������)
        // ���� ������ �� � ��������� ����� �� ������ (playerGameObject ����� null)
        var currentAccuracy = Random.Range(0, 10);
        var playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (currentAccuracy < accuracy)
            playerGameObject = null;

        // ������������ ������� ����(���� �����, ���� ��������� �����)
        var targetPosition = playerGameObject != null ? playerGameObject.transform.position : GetRandomPointInsideBorder();
        // ��������� ������� ������ ����� ������ ��������� �������
        var startPosition = shootingPointDown.position;
        // �������� ���� � ��������� �������
        var goBullet = Instantiate(bullet,
                                       startPosition,
                                       transform.rotation);

        // ���� ����� ��������� � ����������� � ������� ����
        goBullet.GetComponent<Rigidbody2D>().AddForce((targetPosition - startPosition).normalized * bulletSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���� ���������� ������� �� ����� �� ����� �� ����������
        if (collision.gameObject.CompareTag("Enemy Bullet"))
            return;

        // ���� ���, �� ����������� ���� � ���������� ������� ������������ �������
        _gameController.IncrementScore(gameObject.tag);

        Destroy(gameObject);
    }
}
