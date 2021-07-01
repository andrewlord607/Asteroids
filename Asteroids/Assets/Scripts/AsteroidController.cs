using UnityEngine;

//Класс, управляющий поведением астероидов
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class AsteroidController : MonoBehaviour
{
    // Ссылка на префаб астероида, который будет появляться при разбиение текущего
    // Если текущий астероид Small Asteroid, то это поле игнорируется
    public GameObject childAsteroid; 
    // Звуковой эффект при разбиение астероида
    public AudioClip destroy;
    // При появлении астероид получит скорость в промежутке между minSpeed и maxSpeed
    public float minSpeed = 50f;
    public float maxSpeed = 150f;
    // Ссылка на GameController, который управляет поведением игры
    private GameController _gameController;

    private void Start()
    {
        // Толкает астероид в направление, в которое он повёрнут, со скоростью в промежутке между minSpeed и maxSpeed
        // Направление задаётся при создании объекта астероида
        // Поскольку у всех объектов Gravity scale равен 0, то на них не влияет физика и они не будут замедляться
        GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range( minSpeed, maxSpeed));

        // Находим в иерархии объектов GameController с помощью Tag и запоминаем ссылку на него
        // Если не найден, то выбрасываем ошибку
        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            throw new System.Exception("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если сталкиваемся с чем-то кроме объектов пуль (Bullet), то прерываем выполнение
        // Логика сталкивания с другими объектами находится в соответствующих классах
        if (!collision.gameObject.CompareTag("Bullet") && !collision.gameObject.CompareTag("Enemy Bullet"))
            return;
        // Уничтожаем объект пули
        Destroy(collision.gameObject);
        // Разбиваем астероид на части
        Split();
        // Воспроизводим звук уничтожения
        AudioSource.PlayClipAtPoint(destroy, Camera.main.transform.position);
        // Сообщаем GameController'у что необходимо увеличить счёт в соответствии с текущим типом астероида
        _gameController.IncrementScore(gameObject.tag);
        // Уничтожаем текущий астероид
        Destroy(gameObject);
    }

    /// <summary>
    /// Разбитие текущего астероида на два более меньших астероида на основе префаба, хранящегося в childAsteroid
    /// </summary>
    private void Split()
	{
        // Если это Small Asteroid, то уменьшаем количество оставшихся астероидов и всё
        if (CompareTag("Small Asteroid"))
		{
            _gameController.DecrementAsteroids();
            return;
		}

        // Создаем два астероида поменьше, с случайным углом поворота
        // На основе угла поворота будет определено их будущее направление
        Instantiate(childAsteroid,
					new Vector3(transform.position.x - .5f, transform.position.y - .5f, 0),
					Quaternion.Euler(0, 0, Random.Range(0,90)));

        Instantiate(childAsteroid,
                    new Vector3(transform.position.x + .5f,transform.position.y - .5f, 0),
                    Quaternion.Euler(0, 0, Random.Range(180, 270)));

        // Так как астероидов стало больше чем было, то увеличиваем количество оставшихся
        _gameController.IncrementAsteroids();
    }
}
