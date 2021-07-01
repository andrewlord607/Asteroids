using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utils.AsteroidsUtils;

// �����, ������� ��������� ���������� ����
public class GameController : MonoBehaviour
{
    // ������ �� ������ ������ �������� ���������
    public GameObject largeAsteroid;
    // ������ �� ������, ����������� ���������� ������
    // �� ������ ��������� �� �����
    public PlayerController player;
    // ���������� ����������, ������� ����� ���������� ��� ������ ����
    // ��� ����� ��� ������������ ����� ����������
    public int asteroidsToSpawn = 5;

    // ���������� ��� �������� �������� ��������
    // score � ������� ����
    // hiscore � ��������� ����
    // lives � ���������� ���������� ������
    private int score;
    private int hiscore;
    private int lives;

    // ������ �� ��������� �������� UI
    // � ��� ���� ����������� �������� �������� ����
    public Text scoreText;
    public Text livesText;
    public Text hiscoreText;

    // ������ �� �������� ������������ ��������, ������� ��������� ������� ���������� �� ����� ����
    public List<GameObject> spaceShips;

    // ���������� ���������� ����������
    private int _remainingAsteroids = 5;
    // �������, ������������ ��������� ����������� ������ �������� � �����
    // �������� ������������� Tag
    private readonly Dictionary<string, int> scoreTable = new Dictionary<string, int>()
    {
        { "Large Asteroid", 20},
        { "Medium Asteroid", 50},
        { "Small Asteroid", 100},
        { "Spaceship", 200},
        { "Small Spaceship", 500}
    };
    // ������ ��� ������� ��������� ������������ ��������
    private float _spaceShipSpawnCooldown = 0;

    private void Start()
    {
        // �������� ������, ������� �������� � �������
        // ���� ������ � ������� �����������, �� 0
        hiscore = PlayerPrefs.GetInt("Hiscore", 0);

        // �������� ����
        BeginGame();
    }

    private void Update()
    {
        // ���� ������ ������ ������(Esc �� ����������), �� ������� �� ����
        if (Input.GetButton("Cancel"))
            Application.Quit();

        // ����������� �������� ������� 
        _spaceShipSpawnCooldown += Time.deltaTime;
        // ���� ��� �������� ������������ ��������, �� ���������� ������ � ������ ������������ �������
        if (_spaceShipSpawnCooldown >= 50f)
        {
            _spaceShipSpawnCooldown = 0f;
            SpawnSpaceShip();
        }
    }

    /// <summary>
    /// ����� �������� ���������� ����� ������� ����
    /// </summary>
    public void BeginGame()
    {
        // ��������������� �������� �� ���������
        score = 0;
        lives = 3;

        // ��������������� �������� �������� �� ���������
        _spaceShipSpawnCooldown = 0;

        // �������������� UI
        scoreText.text = "SCORE: " + score;
        hiscoreText.text = "HISCORE: " + hiscore;
        livesText.text = "LIVES: " + lives;

        // ������ ���������
        SpawnAsteroids();
        // ���������� ������
        PlayerRespawn();
    }

    /// <summary>
    /// �����, ������� ������ ������������ ������� � ��������� ������� �� ������� ������
    /// </summary>
    private void SpawnSpaceShip()
	{
        // ������ ��������� ������������ ������� �� ������ ������������
        var spaceShip = spaceShips[Random.Range(0, spaceShips.Count)];

        // �������� ���������� � ����� �������( � ����� ��� ������) �������� �������
        var randomPosX = GetWorldPositionOfBorder(Border.left).x * RandomSign();
        // �������� ���������� �� ����� ������ �������� �������
        var randomPosY = Random.Range(GetWorldPositionOfBorder(Border.bottom).y, GetWorldPositionOfBorder(Border.top).y);

        // ������ ������ ������� � ����������� ���� �����
        var goSpaceShip = Instantiate(spaceShip, new Vector3(randomPosX, randomPosY, 0), Quaternion.identity);
        // ���� ��� � ����� �������, �� ������� ����� ��������� ������ � ��������
        goSpaceShip.GetComponent<Spaceship>().moveToRight = (randomPosX < 0);
    }

    /// <summary>
    /// �����, ������� ������� ��� ������������ ��������� � ������ �� ������
    /// </summary>
    private void SpawnAsteroids()
    {
        // ������� ������������ ���������
        DestroyExistingAsteroids();

        // ������������� ���������� ���������� ���������� ������ �����������, ������� ����� �������
        _remainingAsteroids = asteroidsToSpawn;

        for (int i = 0; i < _remainingAsteroids; ++i)
        {
            // ������ �������� � ��������� �����, �� ��� ���� ���������� �����, ��� ������ ��������� �����
            // ��� ���������� 1920 �� 1080
            // x = [-9; -2]&[2, 9]
            // y = [-6; -2]&[2, 6]
            // ���� �������� ���������, ��� ��� �� ����� ���������� ����������� ����� ���������
            Instantiate(largeAsteroid,
						new Vector3(
							Random.Range(GetWorldPositionOfBorder(Border.left).x, -2.0f) * RandomSign(),
							Random.Range(GetWorldPositionOfBorder(Border.bottom).y, -2.0f) * RandomSign(),
							0),
                        Quaternion.Euler(0, 0, Random.Range(0.0f, 359.0f)));
        }
    }

    /// <summary>
    /// �����, ������� ����������� ������� ���� � ����������� �� ������������� ������� ����������� � ��������� tag
    /// </summary>
    public void IncrementScore(string tag)
    {
        // ���������� ����� ������������ � ������� ������� scoreTable
        score += scoreTable[tag];
        // ���������� UI �������� �����
        scoreText.text = "SCORE: " + score;

        // ���� ������� ���� ������ �������, �� ��������� ������
        if (score > hiscore)
        {
            hiscore = score;
            // ���������� UI
            hiscoreText.text = "HISCORE: " + hiscore;

            // ��������� ��������� ���� � �������
            PlayerPrefs.SetInt("Hiscore", hiscore);
        }
    }

    /// <summary>
    /// �����, ������� ��������� ���������� ���������� ������ � ������
    /// </summary>
    public void DecrementLives()
    {
        // ���������� ���������� ������ � ���������� UI
        lives--;
        livesText.text = "LIVES: " + lives;

        // ���� � ������ �� �������� ������, �� ���� ���������� ������
        if (lives < 1)
        {
            BeginGame();
            return;
        }
        
        // ���� ����� ��� ����, �� ���������� ������ ����� ��������� �����
        Invoke(nameof(PlayerRespawn), 2);
    }

    /// <summary>
    /// �����, ������� �������� ������ PlayerController, ��� ���������� �������� ��� �������� � ��������
    /// </summary>
    private void PlayerRespawn()
	{
        player.Respawn();
    }

    /// <summary>
    /// ����� ����������� ���������� ���������� ����������
    /// �������� ��� ������ � ������ �������
    /// </summary>
    public void DecrementAsteroids()
    {
        _remainingAsteroids--;

        // ���� ���������� ��� ���������, �� ������ �� ������
        if (_remainingAsteroids < 1)
            SpawnAsteroids();
    }

    /// <summary>
    /// ����� ������������� ���������� ���������� ����������
    /// ��� ���������� ��-�� ��������� ���������� �� ����� ������
    /// �������� ��� ������ � ������ �������
    /// </summary>
    public void IncrementAsteroids()
    {
        _remainingAsteroids++;
    }

    /// <summary>
    /// ����� ������������ ��� ������������ ��������� �� ������ �� Tag'��
    /// </summary>
    private void DestroyExistingAsteroids()
    {
        DestroyObjectsByTag("Large Asteroid");
        DestroyObjectsByTag("Medium Asteroid");
        DestroyObjectsByTag("Small Asteroid");
    }

    /// <summary>
    /// ����� ������������ ��� ������������ ������� �� ������ �� Tag'��
    /// </summary>
    private void DestroyExistingSpaceships()
    {
        DestroyObjectsByTag("Spaceship");
        DestroyObjectsByTag("Small Spaceship");
    }

    /// <summary>
    /// ����� ������������ ��� ������������ ������� c �������� Tag'��, ������� ��������� � ��������� tag
    /// </summary>
    private void DestroyObjectsByTag(string tag)
	{
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
            Destroy(obj);
    }
}
