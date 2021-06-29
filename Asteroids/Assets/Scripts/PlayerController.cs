using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float impulseForce = 5f;
    public float rotationSpeed = 200.0f;
    public float bulletSpeed = 400f;

    public float friction = 0.98f;
    public float maxVelocity = 3.0f;

    public GameObject bullet;
    public Transform shootingPoint;
    public GameObject thrustFire;

    public AudioClip shoot;
    public AudioClip explode;

    private GameController _gameController;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    private void Start()
    {
        var objectGameController = GameObject.FindGameObjectWithTag("GameController");
        if (objectGameController == null)
            Debug.LogError("Game Controller not found");
        _gameController = objectGameController.GetComponent<GameController>();
    }

    private void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Mathf.Clamp01(Input.GetAxisRaw("Vertical"));

        // update rotation.
        transform.Rotate(new Vector3(0, 0, -inputX), rotationSpeed * Time.deltaTime);

        // update velocity.
        velocity += (inputY * (transform.up * impulseForce)) * Time.deltaTime;

        // apply friction if np input.
        if (inputY == 0.0f)
            velocity *= friction;
        
        thrustFire.SetActive(inputY != 0.0f);

        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        transform.Translate(velocity * Time.deltaTime, Space.World);

        // Has a bullet been fired
        if (Input.GetButtonDown("Fire"))
            Shoot();
    }
    private void Shoot()
    {
        // Spawn a bullet
        var goBullet = Instantiate(bullet,
								   shootingPoint.position,
								   transform.rotation);

        // Push the bullet in the direction it is facing
        goBullet.GetComponent<Rigidbody2D>().AddForce(goBullet.transform.up * bulletSpeed);

        // Play a shoot sound
        AudioSource.PlayClipAtPoint(shoot, Camera.main.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        AudioSource.PlayClipAtPoint(explode, Camera.main.transform.position);
        gameObject.SetActive(false);

        _gameController.DecrementLives();
    }

    public void Respawn()
    {
        transform.position = Vector3.zero;
        velocity = Vector3.zero;

        gameObject.SetActive(true);
    }
}
