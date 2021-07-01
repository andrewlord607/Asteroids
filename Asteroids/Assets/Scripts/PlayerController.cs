using UnityEngine;

// �����, ����������� ���������� ������
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // ���� �������� ��� �������� �����(�������� ������)
    public float impulseForce = 5f;
    // �������� � ������� ���������� �������� ������� ������
    public float rotationSpeed = 200.0f;
    // �������� ��������
    public float bulletSpeed = 400f;
    // ��� ������ ���������� ���������� ������ ����� ���� ��� ����������� ������ ��������
    // ( 1 � �� ���������� �����, 0 � ���������)
    public float friction = 0.98f;
    // ����������� ������������ ��������
    public float maxVelocity = 3.0f;

    // ������ ����, ������� �������� �����
    public GameObject bullet;
    // ������� �� ������� ���������� �������
    public Transform shootingPoint;
    // ������, ������� ������������ ����� ���������� ����������� ����������� �������
    public GameObject thrustFire;

    // �����, ������� ��������������� ��� �������� � ������
    public AudioClip shoot;
    public AudioClip explode;

    // ������ �� GameController, ������� ��������� ���������� ����
    private GameController _gameController;
    // �������� ������ ��� ������� ���������� ���������
    private Vector3 velocity = Vector3.zero;

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
        // ��������� �������� ��� �������� ������
        float inputX = Input.GetAxis("Horizontal");
        // ��������� �������� �������� ������ ����� � ����� 
        // ����������� �������� ������� ���� ����, �������� ��������� �����
        float inputY = Mathf.Clamp01(Input.GetAxisRaw("Vertical"));

        // ��������� ������� ������, �������� �������� ��������.
        transform.Rotate(new Vector3(0, 0, -inputX), rotationSpeed * Time.deltaTime);

        // ��������� �������� ������ �� ������ ��������� ��������
        velocity += (inputY * (transform.up * impulseForce)) * Time.deltaTime;

        // ��������� ����������, ���� ����� �� ��������� �����
        if (inputY == 0.0f)
            velocity *= friction;
        
        // �������� ��� ��������� ���������� ����������� ����������� �������
        thrustFire.SetActive(inputY != 0.0f);

        // ������������ �������� ������������ ���������
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        // ���������� ������ �� ������ ����������� ��������
        transform.Translate(velocity * Time.deltaTime, Space.World);

        // ���� ������ ������ ��������(spacebar, left mouse), �� �������� ������ ��������
        if (Input.GetButtonDown("Fire"))
            Shoot();
    }

    /// <summary>
    /// �����, ������� ���������� �������(������ ����)
    /// </summary>
    private void Shoot()
    {
        // ������ ���� � shootingPoint.position � ������������ ������� ����� �� ��� � � ������
        var goBullet = Instantiate(bullet,
								   shootingPoint.position,
								   transform.rotation);

        // ������� ���� � �����������, � ������� ��� ��������
        goBullet.GetComponent<Rigidbody2D>().AddForce(goBullet.transform.up * bulletSpeed);

        // ������������� ���� ��������
        AudioSource.PlayClipAtPoint(shoot, Camera.main.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // ��� ����������� � ����� �������� ���������� ������ (��������������� ���� � ������������ ������ ������� ������)
        AudioSource.PlayClipAtPoint(explode, Camera.main.transform.position);
        gameObject.SetActive(false);

        // �������� GameController'�, ��� ���������� ��������� ���������� ��������� ������
        _gameController.DecrementLives();
    }

    /// <summary>
    /// �����, ������� ���������� ��� �������� ������� ������ � ��������
    /// </summary>
    public void Respawn()
    {
        // ������� ������ ���������� �� ������ ������
        transform.position = Vector3.zero;
        // �������� ������������
        velocity = Vector3.zero;
        // ������ ������� ������ ������������ 
        gameObject.SetActive(true);
    }
}
