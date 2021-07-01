using UnityEngine;

//�����, ����������� ���������� ����������
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class AsteroidController : MonoBehaviour
{
    // ������ �� ������ ���������, ������� ����� ���������� ��� ��������� ��������
    // ���� ������� �������� Small Asteroid, �� ��� ���� ������������
    public GameObject childAsteroid; 
    // �������� ������ ��� ��������� ���������
    public AudioClip destroy;
    // ��� ��������� �������� ������� �������� � ���������� ����� minSpeed � maxSpeed
    public float minSpeed = 50f;
    public float maxSpeed = 150f;
    // ������ �� GameController, ������� ��������� ���������� ����
    private GameController _gameController;

    private void Start()
    {
        // ������� �������� � �����������, � ������� �� �������, �� ��������� � ���������� ����� minSpeed � maxSpeed
        // ����������� ������� ��� �������� ������� ���������
        // ��������� � ���� �������� Gravity scale ����� 0, �� �� ��� �� ������ ������ � ��� �� ����� �����������
        GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range( minSpeed, maxSpeed));

        // ������� � �������� �������� GameController � ������� Tag � ���������� ������ �� ����
        // ���� �� ������, �� ����������� ������
        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            throw new System.Exception("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���� ������������ � ���-�� ����� �������� ���� (Bullet), �� ��������� ����������
        // ������ ����������� � ������� ��������� ��������� � ��������������� �������
        if (!collision.gameObject.CompareTag("Bullet") && !collision.gameObject.CompareTag("Enemy Bullet"))
            return;
        // ���������� ������ ����
        Destroy(collision.gameObject);
        // ��������� �������� �� �����
        Split();
        // ������������� ���� �����������
        AudioSource.PlayClipAtPoint(destroy, Camera.main.transform.position);
        // �������� GameController'� ��� ���������� ��������� ���� � ������������ � ������� ����� ���������
        _gameController.IncrementScore(gameObject.tag);
        // ���������� ������� ��������
        Destroy(gameObject);
    }

    /// <summary>
    /// �������� �������� ��������� �� ��� ����� ������� ��������� �� ������ �������, ����������� � childAsteroid
    /// </summary>
    private void Split()
	{
        // ���� ��� Small Asteroid, �� ��������� ���������� ���������� ���������� � ��
        if (CompareTag("Small Asteroid"))
		{
            _gameController.DecrementAsteroids();
            return;
		}

        // ������� ��� ��������� ��������, � ��������� ����� ��������
        // �� ������ ���� �������� ����� ���������� �� ������� �����������
        Instantiate(childAsteroid,
					new Vector3(transform.position.x - .5f, transform.position.y - .5f, 0),
					Quaternion.Euler(0, 0, Random.Range(0,90)));

        Instantiate(childAsteroid,
                    new Vector3(transform.position.x + .5f,transform.position.y - .5f, 0),
                    Quaternion.Euler(0, 0, Random.Range(180, 270)));

        // ��� ��� ���������� ����� ������ ��� ����, �� ����������� ���������� ����������
        _gameController.IncrementAsteroids();
    }
}
