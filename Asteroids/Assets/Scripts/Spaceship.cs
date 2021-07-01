using UnityEngine;
using static Utils.AsteroidsUtils;

// Класс, который управляет поведением инопланетных кораблей
[RequireComponent(typeof(Rigidbody2D))]
public class Spaceship : MonoBehaviour
{
    // Скорость корабля
    public float speed = 10f;
    // Время перезарядки выстрела
    public float shootCooldown = 100f;
    // Скорость выстрела
    public float bulletSpeed = 400f;
    // Точность прицеливания
    // 1 — очень редко прицеливается в игрока
    // 10 — всегда прицеливается в игрока
    [Range(1, 10)]
    public int accuracy = 1;

    // В какую сторону движется корабль
    public bool moveToRight = true;
    // Время, спустя которое корабль меняет свою линию(координату y)
    public float changeLineCooldown = 10f;
    // Время, которое уходит на смену линии
    public float timeToChangeLine = 0.5f;
    // После выхода за границу корабль деактивируется и затем спустся данное время снова активируется
    public float reactivateCooldown = 5f;

    // Префаб пули инопланетного корабля
    public GameObject bullet;
    // Позиция из которой происходит выстрел
    public Transform shootingPointDown;

    // Ссылка на GameController, который управляет поведением игры
    private GameController _gameController;
    // Таймеры для отсчёта времени смены линии, выстрела и т.д.
    private float _changeLineTimer = 0f;
    private float _shootCooldown = 0f;
    private float _changeLineCooldown = 0f;
    // В какую сторону меняется линия(вверх или вниз)
    private Vector3 _changeLineDirection = Vector3.up;

    private void Start()
    {
        // Находим в иерархии объектов GameController с помощью Tag и запоминаем ссылку на него
        // Если не найден, то выбрасываем ошибку
        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            throw new System.Exception("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    private void Update()
    {
        // Если пришло время выстрела, то вызывается метод Shoot
        _shootCooldown += Time.deltaTime;
        if (_shootCooldown >= shootCooldown)
        {
            _shootCooldown = 0;
            Shoot();
        }

        // Если пришло время смены линииц, то вызывается метод ChangeLine
        _changeLineCooldown += Time.deltaTime;
        if (_changeLineCooldown >= changeLineCooldown)
        {
            _changeLineCooldown = 0;
            ChangeLine();
        }


        // Если корабль выходит за левую или правую границу экрана, то он деактивируется
        // Спустя некоторое время снова активируется, но теперь движется в противоположную сторону
        if (transform.position.x > GetWorldPositionOfBorder(Border.right).x || transform.position.x < GetWorldPositionOfBorder(Border.left).x)
        {
            gameObject.SetActive(false);
            moveToRight = !moveToRight;
            // Смещаем корабль в противоположную сторону, чтобы он больше не находился за границей
            transform.Translate((moveToRight? Vector2.right: Vector2.left) * 0.2f);
            Invoke(nameof(Reactivate), reactivateCooldown);
        }

        // Направление движение определяется на основе флака moveToRight
        var direction = moveToRight ? Vector3.right : Vector3.left;
        // _changeLineTimer > 0f то значит в этот момент происходит смена линии
        // В этом случае направление движение также должно учитывать направление смены линии
        if (_changeLineTimer > 0f)
        {
            direction += _changeLineDirection;
            _changeLineTimer -= Time.deltaTime;
        }
        // Новая позиция вычисляется на основе направления движения и скорости
        transform.position += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// Метод, который отвечает за смену линии у инопланетного корабля
    /// </summary>
    private void ChangeLine()
	{
        // Запускаем таймер смены линии
        // Пока он больше нуля будет происходить смена линии
        _changeLineTimer = timeToChangeLine;
        // Напрвление смены линии определяется случайным образом
        _changeLineDirection *= RandomSign();
    }

    /// <summary>
    /// Метод, который заново активирует корабль
    /// </summary>
    private void Reactivate()
	{
        gameObject.SetActive(true);
	}

    /// <summary>
    /// Метод, который производит выстрел(создаёт пули)
    /// </summary>
    private void Shoot()
	{
        // Берётся случайное значение от 0 до 10
        // Если оно больше точности корабля значит происходит выстрел по игроку (playerGameObject содержит ссылку на игрока)
        // Если меньше то в случайную точку на экране (playerGameObject равен null)
        var currentAccuracy = Random.Range(0, 10);
        var playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (currentAccuracy < accuracy)
            playerGameObject = null;

        // Определяется позиция цели(либо игрок, либо случайная точка)
        var targetPosition = playerGameObject != null ? playerGameObject.transform.position : GetRandomPointInsideBorder();
        // Начальная позиция всегда точка откуда просходит выстрел
        var startPosition = shootingPointDown.position;
        // Создаётся пуля в стартовой позиции
        var goBullet = Instantiate(bullet,
                                       startPosition,
                                       transform.rotation);

        // Пуля будет двигаться в направлении к позиции цели
        goBullet.GetComponent<Rigidbody2D>().AddForce((targetPosition - startPosition).normalized * bulletSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если происходит контакт со своей же пулей то игнорируем
        if (collision.gameObject.CompareTag("Enemy Bullet"))
            return;

        // Если нет, то увеличиваем счёт и уничтожаем текущий инопланетный корабль
        _gameController.IncrementScore(gameObject.tag);

        Destroy(gameObject);
    }
}
