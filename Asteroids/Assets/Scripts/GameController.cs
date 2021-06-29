using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utils.AsteroidsUtils;

public class GameController : MonoBehaviour
{
    public GameObject largeAsteroid;
    public PlayerController player;

    public int asteroidsToSpawn = 5;
    

    private int score;
    private int hiscore;
    private int lives;

    public Text scoreText;
    public Text livesText;
    public Text hiscoreText;

    public List<GameObject> spaceShips;

    private int remainingAsteroids = 5;
    private readonly Dictionary<string, int> scoreTable = new Dictionary<string, int>()
    {
        { "Large Asteroid", 20},
        { "Medium Asteroid", 50},
        { "Small Asteroid", 100},
        { "Spaceship", 200},
        { "Small Spaceship", 500}
    };

    private void Start()
    {
        hiscore = PlayerPrefs.GetInt("Hiscore", 0);

        BeginGame();
    }

    private void Update()
    {
        if (Input.GetButton("Cancel"))
            Application.Quit();
    }

	public void BeginGame()
    {
        score = 0;
        lives = 3;

        // Prepare the HUD
        scoreText.text = "SCORE: " + score;
        hiscoreText.text = "HISCORE: " + hiscore;
        livesText.text = "LIVES: " + lives;

        SpawnAsteroids();
        InvokeRepeating(nameof(SpawnSpaceShip), 50, 50);
        PlayerRespawn();
    }

    private void SpawnSpaceShip()
	{
        var spaceShip = spaceShips[Random.Range(0, spaceShips.Count)];

        var randomPosX = GetWorldPositionOfBorder(Border.left).x * RandomSign();
        var randomPosY = Random.Range(GetWorldPositionOfBorder(Border.bottom).y, GetWorldPositionOfBorder(Border.top).y);

        var goSpaceShip = Instantiate(spaceShip, new Vector3(randomPosX, randomPosY, 0), Quaternion.identity);
        goSpaceShip.GetComponent<Spaceship>().moveToRight = (randomPosX < 0);
	}

    private void SpawnAsteroids()
    {
        DestroyExistingAsteroids();

        remainingAsteroids = asteroidsToSpawn;
        for (int i = 0; i < remainingAsteroids; ++i)
        {
            // Spawn an asteroid
            Instantiate(largeAsteroid,
						new Vector3(
							Random.Range(-9.0f, -2.0f) * RandomSign(),
							Random.Range(-6.0f, -2.0f) * RandomSign(),
							0),
                        Quaternion.Euler(0, 0, Random.Range(0.0f, 359.0f)));
        }
    }

    public void IncrementScore(string asteroidTag)
    {
        score += scoreTable[asteroidTag];
        scoreText.text = "SCORE: " + score;

        if (score > hiscore)
        {
            hiscore = score;
            hiscoreText.text = "HISCORE: " + hiscore;

            // Save the new hiscore
            PlayerPrefs.SetInt("Hiscore", hiscore);
        }

        // Has player destroyed all asteroids?
        if (remainingAsteroids < 1)
            SpawnAsteroids();
    }

    public void DecrementLives()
    {
        lives--;
        livesText.text = "LIVES: " + lives;

        // Has player run out of lives?
        if (lives < 1)
            BeginGame(); // Restart the game
        else
            Invoke(nameof(PlayerRespawn), 2);
    }

    private void PlayerRespawn()
	{
        player.Respawn();
    }

    public void DecrementAsteroids()
    {
        remainingAsteroids--;
    }

    public void SplitAsteroid()
    {
        remainingAsteroids++;
    }

    private void DestroyExistingAsteroids()
    {
        DestroyAsteroidsByTag("Large Asteroid");
        DestroyAsteroidsByTag("Medium Asteroid");
        DestroyAsteroidsByTag("Small Asteroid");
    }

    private void DestroyAsteroidsByTag(string tag)
	{
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject asteroid in asteroids)
            Destroy(asteroid);
    }
}
