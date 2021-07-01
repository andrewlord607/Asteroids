using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utils.AsteroidsUtils;

// Класс, который управляет поведением игры
public class GameController : MonoBehaviour
{
    // Ссылка на префаб самого большого астероида
    public GameObject largeAsteroid;
    // Ссылка на скрипт, управляющий поведением игрока
    // Он должен находится на сцене
    public PlayerController player;
    // Количество астероидов, которые будут появляться при старте игры
    // или когда все существующие будут уничтожены
    public int asteroidsToSpawn = 5;

    // Переменные для подсчёта числовых значений
    // score — текущий счёт
    // hiscore — рекордный счёт
    // lives — количество оставшихся жизней
    private int score;
    private int hiscore;
    private int lives;

    // Ссылки на текстовые элементы UI
    // В них буду отображатья числовые значения выше
    public Text scoreText;
    public Text livesText;
    public Text hiscoreText;

    // Список из префабов инопланетных кораблей, которые случайным образом появляются во время игры
    public List<GameObject> spaceShips;

    // Количество оставшихся астероидов
    private int _remainingAsteroids = 5;
    // Словарь, определяющий стоимость уничтожение разных объектов в очках
    // Названия соответствуют Tag
    private readonly Dictionary<string, int> scoreTable = new Dictionary<string, int>()
    {
        { "Large Asteroid", 20},
        { "Medium Asteroid", 50},
        { "Small Asteroid", 100},
        { "Spaceship", 200},
        { "Small Spaceship", 500}
    };
    // Таймер для времени появления инопланетных кораблей
    private float _spaceShipSpawnCooldown = 0;

    private void Start()
    {
        // Получаем рекорд, который хранится в реестре
        // Если запись о рекорде отсутствует, то 0
        hiscore = PlayerPrefs.GetInt("Hiscore", 0);

        // Начинаем игру
        BeginGame();
    }

    private void Update()
    {
        // Если нажата кнопка отмены(Esc на клавиатуре), то выходим из игры
        if (Input.GetButton("Cancel"))
            Application.Quit();

        // Увеличиваем значение таймера 
        _spaceShipSpawnCooldown += Time.deltaTime;
        // Если оно достигло определённого значения, то сбрасываем таймер и создаём инопланетный корабль
        if (_spaceShipSpawnCooldown >= 50f)
        {
            _spaceShipSpawnCooldown = 0f;
            SpawnSpaceShip();
        }
    }

    /// <summary>
    /// Метод делающий подготовку перед началом игры
    /// </summary>
    public void BeginGame()
    {
        // Восстанавливаем значения по умолчания
        score = 0;
        lives = 3;

        // Восстанавливаем значение таймеров по умолчанию
        _spaceShipSpawnCooldown = 0;

        // Подготавливаем UI
        scoreText.text = "SCORE: " + score;
        hiscoreText.text = "HISCORE: " + hiscore;
        livesText.text = "LIVES: " + lives;

        // Создаём астероиды
        SpawnAsteroids();
        // Возрождаем игрока
        PlayerRespawn();
    }

    /// <summary>
    /// Метод, который создаёт инопланетные корабли в случайной позиции на границе экрана
    /// </summary>
    private void SpawnSpaceShip()
	{
        // Достаём случайный инопланетный корабль из списка существующих
        var spaceShip = spaceShips[Random.Range(0, spaceShips.Count)];

        // Случайно определяем с какой стороны( с левой или правой) появится корабль
        var randomPosX = GetWorldPositionOfBorder(Border.left).x * RandomSign();
        // Случайно определяем на какой высоте появится корабль
        var randomPosY = Random.Range(GetWorldPositionOfBorder(Border.bottom).y, GetWorldPositionOfBorder(Border.top).y);

        // Создаём объект корабля в определённой выше точке
        var goSpaceShip = Instantiate(spaceShip, new Vector3(randomPosX, randomPosY, 0), Quaternion.identity);
        // Если это с левой стороны, то корабль будет двигаться вправо и наоборот
        goSpaceShip.GetComponent<Spaceship>().moveToRight = (randomPosX < 0);
    }

    /// <summary>
    /// Метод, который удаляет уже существующие астероиды и создаёт их заново
    /// </summary>
    private void SpawnAsteroids()
    {
        // Удаляем существующие астероиды
        DestroyExistingAsteroids();

        // Устанавливаем количество оставшихся астероидов равным количеством, которое будет создано
        _remainingAsteroids = asteroidsToSpawn;

        for (int i = 0; i < _remainingAsteroids; ++i)
        {
            // Создаём астероид в случайной точке, но при этом игнорируем центр, где должен находится игрок
            // Для разрешения 1920 на 1080
            // x = [-9; -2]&[2, 9]
            // y = [-6; -2]&[2, 6]
            // Угол поворота случайный, так как он будет определять направление полёта астероида
            Instantiate(largeAsteroid,
						new Vector3(
							Random.Range(GetWorldPositionOfBorder(Border.left).x, -2.0f) * RandomSign(),
							Random.Range(GetWorldPositionOfBorder(Border.bottom).y, -2.0f) * RandomSign(),
							0),
                        Quaternion.Euler(0, 0, Random.Range(0.0f, 359.0f)));
        }
    }

    /// <summary>
    /// Метод, который увеличивает текущий счёт в зависимости от уничтоженного объекта переданного в параметре tag
    /// </summary>
    public void IncrementScore(string tag)
    {
        // Количество очков определяется с помощью словаря scoreTable
        score += scoreTable[tag];
        // Обновление UI текущего счёта
        scoreText.text = "SCORE: " + score;

        // Если текущий счёт больше рекорда, то обновляем рекорд
        if (score > hiscore)
        {
            hiscore = score;
            // Обновление UI
            hiscoreText.text = "HISCORE: " + hiscore;

            // Сохраняем рекордный счёт в реестре
            PlayerPrefs.SetInt("Hiscore", hiscore);
        }
    }

    /// <summary>
    /// Метод, который уменьшает оставшееся количество жизней у игрока
    /// </summary>
    public void DecrementLives()
    {
        // Уменьшение количества жизней и обновление UI
        lives--;
        livesText.text = "LIVES: " + lives;

        // Если у игрока не осталось жизней, то игра начинается заново
        if (lives < 1)
        {
            BeginGame();
            return;
        }
        
        // Если жизни ещё есть, то возрождаем игрока через некоторое время
        Invoke(nameof(PlayerRespawn), 2);
    }

    /// <summary>
    /// Метод, который сообщает классу PlayerController, что необходимо сбросить все значения в исходные
    /// </summary>
    private void PlayerRespawn()
	{
        player.Respawn();
    }

    /// <summary>
    /// Метод уменьшающий количество оставшихся астероидов
    /// Доступен для вызова в других классах
    /// </summary>
    public void DecrementAsteroids()
    {
        _remainingAsteroids--;

        // Если уничтожены все астероиды, то создаём их заново
        if (_remainingAsteroids < 1)
            SpawnAsteroids();
    }

    /// <summary>
    /// Метод увеличивающий количество оставшихся астероидов
    /// Это происходит из-за разбиение астероидов на более мелкие
    /// Доступен для вызова в других классах
    /// </summary>
    public void IncrementAsteroids()
    {
        _remainingAsteroids++;
    }

    /// <summary>
    /// Метод уничтожающий все существующие астероиды на основе их Tag'ов
    /// </summary>
    private void DestroyExistingAsteroids()
    {
        DestroyObjectsByTag("Large Asteroid");
        DestroyObjectsByTag("Medium Asteroid");
        DestroyObjectsByTag("Small Asteroid");
    }

    /// <summary>
    /// Метод уничтожающий все инопланетные корабли на основе их Tag'ов
    /// </summary>
    private void DestroyExistingSpaceships()
    {
        DestroyObjectsByTag("Spaceship");
        DestroyObjectsByTag("Small Spaceship");
    }

    /// <summary>
    /// Метод уничтожающий все существующие объекты c заданным Tag'ом, который передаётся в параметре tag
    /// </summary>
    private void DestroyObjectsByTag(string tag)
	{
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
            Destroy(obj);
    }
}
