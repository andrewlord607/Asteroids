using UnityEngine;

// Класс, который позволяет объекту перемещаться в зацикленном на себя пространстве
// Всё что ушло за границу слева появится на границу справа и т.д.
public class InfinityMovement : MonoBehaviour
{
	// Отступ от границы
	// Необходим, чтобы контролировать когда именно объект переместится в противоположную сторону
	public float padding = 0.2f;

	// Поле, в котором хранятся координаты объекта в координатах Viewport
	// Если происходит выход за границы, то значение этого поля тоже меняется
	private Vector3 viewportPos;

	// Границы экрана в координатах Viewport с учётом отступа
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
		// Конвертируем координаты объекта из мировых в координаты Viewport
		viewportPos = Camera.main.WorldToViewportPoint(transform.position);

		// Если объект вышел за границы, то перемещаем его в новые координаты
		if (!CheckInBounds())
			transform.position = Camera.main.ViewportToWorldPoint(viewportPos);
	}

	/// <summary>
	/// Метод проверяющий находится ли текущий объект в границах экрана
	/// </summary>
	private bool CheckInBounds()
	{
		// Если объект вышел хотя бы за одну из границ, то вернётся false
		if (!CheckXBound() || !CheckYBound())
			return false;

		return true;
	}

	/// <summary>
	/// Метод проверяющий находится ли текущий объект между левой и правой границей экрана
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
	/// Метод проверяющий находится ли текущий объект между верхней и нижней границей экрана
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
