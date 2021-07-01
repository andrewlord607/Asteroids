using UnityEngine;

// Класс, который определяет поведение пули
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private void Start()
    {
        // Данный объект уничтожится спустя 1 секунду
        Destroy(gameObject, 1f);
    }
}
