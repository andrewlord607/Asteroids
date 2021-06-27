using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject largeAsteroid;
    public GameObject ship;

    private int score;
    private int hiscore;
    private int asteroidsRemaining;
    private int lives;

    public Text scoreText;
    public Text livesText;
    public Text hiscoreText;

    private Dictionary<string, int> scoreTable = new Dictionary<string, int>()
    {
        { "Large Asteroid", 20},
        { "Medium Asteroid", 50},
        { "Small Asteroid", 100}
    };

    // Start is called before the first frame update
    void Start()
    {
        hiscore = PlayerPrefs.GetInt("Hiscore", 0);
        BeginGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
            Application.Quit();
    }

    void BeginGame()
    {
        score = 0;
        lives = 3;

        // Prepare the HUD
        scoreText.text = "SCORE:" + score;
        hiscoreText.text = "HISCORE: " + hiscore;
        livesText.text = "LIVES: " + lives;

        SpawnAsteroids();
        Instantiate(ship);
    }

    void SpawnAsteroids()
    {
        DestroyExistingAsteroids();

        // Decide how many asteroids to spawn
        // If any asteroids left over from previous game, subtract them
        asteroidsRemaining = 5;

        for (int i = 0; i < asteroidsRemaining; ++i)
        {
            // Spawn an asteroid
            Instantiate(largeAsteroid,
						new Vector3(
							RandomValueInRangeWithExclude(-9.0f, -2.0f),
							RandomValueInRangeWithExclude(-6.0f, -2.0f),
							0),
                        Quaternion.Euler(0, 0, Random.Range(0.0f, 359.0f)));
        }
    }

    float RandomValueInRangeWithExclude(float min, float minExclude)
    {
        float value = Random.Range(min, minExclude); // value between min and max Inclusive
        float sign = Random.value < 0.5f ? -1f : 1f; // select a negative or positive value

        return sign * value;
    }

    public void IncrementScore(string asteroidTag)
    {
        score += scoreTable[asteroidTag];
        
        scoreText.text = "SCORE:" + score;

        if (score > hiscore)
        {
            hiscore = score;
            hiscoreText.text = "HISCORE: " + hiscore;

            // Save the new hiscore
            PlayerPrefs.SetInt("hiscore", hiscore);
        }

        // Has player destroyed all asteroids?
        if (asteroidsRemaining < 1)
        {
            // Start next wave
            SpawnAsteroids();
        }
    }

    public void DecrementLives()
    {
        lives--;
        livesText.text = "LIVES: " + lives;

        // Has player run out of lives?
        if (lives < 1)
            BeginGame(); // Restart the game
        else
            Invoke(nameof(Respawn), 2);
    }

    void Respawn()
	{
        Instantiate(ship);
    }

    public void DecrementAsteroids()
    {
        asteroidsRemaining--;
    }

    public void SplitAsteroid()
    {
        // Two extra asteroids
        // - big one
        // + 2 little ones
        // = 1
        asteroidsRemaining ++;
    }

    void DestroyExistingAsteroids()
    {
        DestroyAsteroidsByTag("Large Asteroid");
        DestroyAsteroidsByTag("Medium Asteroid");
        DestroyAsteroidsByTag("Small Asteroid");
    }

    void DestroyAsteroidsByTag(string tag)
	{
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject asteroid in asteroids)
            Destroy(asteroid);
    }
}
