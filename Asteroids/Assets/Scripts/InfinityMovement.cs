using UnityEngine;

public class InfinityMovement : MonoBehaviour
{
    void Update()
    {
        // Teleport the game object
        if (transform.position.x > 9)
        {

            transform.position = new Vector3(-9, transform.position.y, 0);

        }
        else if (transform.position.x < -9)
        {
            transform.position = new Vector3(9, transform.position.y, 0);
        }

        else if (transform.position.y > 6)
        {
            transform.position = new Vector3(transform.position.x, -6, 0);
        }

        else if (transform.position.y < -6)
        {
            transform.position = new Vector3(transform.position.x, 6, 0);
        }
    }
}
