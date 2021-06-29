using UnityEngine;

namespace Utils
{
	static class AsteroidsUtils
	{
		public enum Border
		{
			top,
			bottom,
			left,
			right
		}
		public static float RandomSign() => Random.value < 0.5 ? -1f : 1f;

		public static Vector3 GetWorldPositionOfBorder(Border border)
		{
			var camera = Camera.main;

			return border switch
			{
				Border.top => camera.ViewportToWorldPoint(Vector3.up),
				Border.bottom => camera.ViewportToWorldPoint(Vector3.down),
				Border.left => camera.ViewportToWorldPoint(Vector3.forward),
				Border.right => camera.ViewportToWorldPoint(Vector3.right),
				_ => Vector3.zero,
			};
		}

		public static Vector3 GetRandomPointInsideBorder()
		{
			var camera = Camera.main;
			return camera.ViewportToWorldPoint(new Vector3(Random.value, Random.value, 0));
		}
	}
}
