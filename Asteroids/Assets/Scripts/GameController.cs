using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject asteroid;
    private int asteroidsRemaining;
    // Start is called before the first frame update
    void Start()
    {
        BeginGame();
    }

    void BeginGame()
    {
        SpawnAsteroids();
    }

    void SpawnAsteroids()
    {
        // Decide how many asteroids to spawn
        // If any asteroids left over from previous game, subtract them
        asteroidsRemaining = 4;

        for (int i = 0; i < asteroidsRemaining; i++)
        {

            // Spawn an asteroid
            Instantiate(asteroid,
                new Vector3(Random.Range(-9.0f, 9.0f),
                    Random.Range(-6.0f, 6.0f), 0),
                Quaternion.Euler(0, 0, Random.Range(-0.0f, 359.0f)));

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
