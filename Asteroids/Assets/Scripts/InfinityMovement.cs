using UnityEngine;

public class InfinityMovement : MonoBehaviour
{
	[SerializeField]
	private float padding = 0.2f;

	private Vector3 viewportPos;

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
		// convert transform position to viewport position.
		viewportPos = Camera.main.WorldToViewportPoint(transform.position);

		if (!CheckInBounds())
			transform.position = Camera.main.ViewportToWorldPoint(viewportPos);
	}

	bool CheckInBounds()
	{
		if (!CheckXBound() || !CheckYBound())
			return false;

		return true;
	}

	bool CheckXBound()
	{
		// check x
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

	bool CheckYBound()
	{
		// check y
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
