using UnityEngine;

// �����, ������� ���������� ��������� ����
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private void Start()
    {
        // ������ ������ ����������� ������ 1 �������
        Destroy(gameObject, 1f);
    }
}
