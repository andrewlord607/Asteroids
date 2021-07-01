using UnityEngine;

// Класс, управляющий поведением игрока
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Сила импульса при движение вперёд(скорость игрока)
    public float impulseForce = 5f;
    // Скорость с которой происходит вращение корабля игрока
    public float rotationSpeed = 200.0f;
    // Скорость выстрела
    public float bulletSpeed = 400f;
    // Как быстро происходит замедление игрока после того как отпускается кнопка движения
    // ( 1 — не происходит вовсе, 0 — мгновенно)
    public float friction = 0.98f;
    // Ограничение максимальной скорости
    public float maxVelocity = 3.0f;

    // Префаб пули, которой стреляет игрок
    public GameObject bullet;
    // Позиция из которой происходит выстрел
    public Transform shootingPoint;
    // Объект, который представляет собой визуальное отображение реактивного пламени
    public GameObject thrustFire;

    // Звуки, которые воспроизводятся при выстреле и смерти
    public AudioClip shoot;
    public AudioClip explode;

    // Ссылка на GameController, который управляет поведением игры
    private GameController _gameController;
    // Скорость игрока для ручного управления движением
    private Vector3 velocity = Vector3.zero;

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
        // Считываем значения для поворота игрока
        float inputX = Input.GetAxis("Horizontal");
        // Считываем значения движения игрока вперёд и назад 
        // Отбрасываем значения которые ниже нуля, запрещая двигаться назад
        float inputY = Mathf.Clamp01(Input.GetAxisRaw("Vertical"));

        // Обновляем поворот игрока, учитывая скорость поворота.
        transform.Rotate(new Vector3(0, 0, -inputX), rotationSpeed * Time.deltaTime);

        // Обновляем скорость игрока на основе считанных значений
        velocity += (inputY * (transform.up * impulseForce)) * Time.deltaTime;

        // Применяем замедление, если игрок не двигается вперёд
        if (inputY == 0.0f)
            velocity *= friction;
        
        // Включаем или выключаем визуальное отображения реактивного пламени
        thrustFire.SetActive(inputY != 0.0f);

        // Ограничиваем скорость максимальной скоростью
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        // Перемещаем игрока на основе вычесленных значений
        transform.Translate(velocity * Time.deltaTime, Space.World);

        // Если нажата кнопка выстрела(spacebar, left mouse), то вызываем метода выстрела
        if (Input.GetButtonDown("Fire"))
            Shoot();
    }

    /// <summary>
    /// Метод, который производит выстрел(создаёт пули)
    /// </summary>
    private void Shoot()
    {
        // Создаём пулю в shootingPoint.position и устанвливаем поворот такой же как и у игрока
        var goBullet = Instantiate(bullet,
								   shootingPoint.position,
								   transform.rotation);

        // Толкаем пулю в направление, в котором она повёрнута
        goBullet.GetComponent<Rigidbody2D>().AddForce(goBullet.transform.up * bulletSpeed);

        // Воспроизводим звук выстрела
        AudioSource.PlayClipAtPoint(shoot, Camera.main.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // При сталкивании с любым объектом происходит смерть (воспроизводится звук и деактивируем объект корабля игрока)
        AudioSource.PlayClipAtPoint(explode, Camera.main.transform.position);
        gameObject.SetActive(false);

        // Сообщаем GameController'у, что необходимо уменьшить количество доступных жизней
        _gameController.DecrementLives();
    }

    /// <summary>
    /// Метод, который сбрасывает все значения корабля игрока в исходные
    /// </summary>
    public void Respawn()
    {
        // Корабль игрока появляется по центру экрана
        transform.position = Vector3.zero;
        // Скорость сбрасывается
        velocity = Vector3.zero;
        // Объект корабля игрока активируется 
        gameObject.SetActive(true);
    }
}
