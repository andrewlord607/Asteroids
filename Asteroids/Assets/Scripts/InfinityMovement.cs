using UnityEngine;

// �����, ������� ��������� ������� ������������ � ����������� �� ���� ������������
// �� ��� ���� �� ������� ����� �������� �� ������� ������ � �.�.
public class InfinityMovement : MonoBehaviour
{
	// ������ �� �������
	// ���������, ����� �������������� ����� ������ ������ ������������ � ��������������� �������
	public float padding = 0.2f;

	// ����, � ������� �������� ���������� ������� � ����������� Viewport
	// ���� ���������� ����� �� �������, �� �������� ����� ���� ���� ��������
	private Vector3 viewportPos;

	// ������� ������ � ����������� Viewport � ������ �������
	private float top;
	private float bottom;
	private float left;
	private float right;

	public virtual void Awake()
	{
		top = 0.0f - padding;
		bottom = 1.0f + padding;
		left = 0.0f - padding;
		right = 1.0f + padding;
	}

	public virtual void Update()
	{
		// ������������ ���������� ������� �� ������� � ���������� Viewport
		viewportPos = Camera.main.WorldToViewportPoint(transform.position);

		// ���� ������ ����� �� �������, �� ���������� ��� � ����� ����������
		if (!CheckInBounds())
			transform.position = Camera.main.ViewportToWorldPoint(viewportPos);
	}

	/// <summary>
	/// ����� ����������� ��������� �� ������� ������ � �������� ������
	/// </summary>
	private bool CheckInBounds()
	{
		// ���� ������ ����� ���� �� �� ���� �� ������, �� ������� false
		if (!CheckXBound() || !CheckYBound())
			return false;

		return true;
	}

	/// <summary>
	/// ����� ����������� ��������� �� ������� ������ ����� ����� � ������ �������� ������
	/// </summary>
	private bool CheckXBound()
	{
		if (viewportPos.x < left)
		{
			viewportPos.x = right;
			return false;
		}

		if (viewportPos.x > right)
		{
			viewportPos.x = left;
			return false;
		}

		return true;
	}

	/// <summary>
	/// ����� ����������� ��������� �� ������� ������ ����� ������� � ������ �������� ������
	/// </summary>
	bool CheckYBound()
	{
		if (viewportPos.y < top)
		{
			viewportPos.y = bottom;
			return false;
		}

		if (viewportPos.y > bottom)
		{
			viewportPos.y = top;
			return false;
		}

		return true;
	}
}
