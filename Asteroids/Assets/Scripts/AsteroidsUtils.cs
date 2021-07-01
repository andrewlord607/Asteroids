using UnityEngine;

namespace Utils
{
	// Класс, содержащий вспомогательные методы
	static class AsteroidsUtils
	{
		// 4 вида границы экрана для GetWorldPositionOfBorder
		public enum Border
		{
			top,
			bottom,
			left,
			right
		}
		/// <summary>
		/// Получить случайный знак
		/// </summary>
		public static float RandomSign() => Random.value < 0.5 ? -1f : 1f;

		/// <summary>
		/// Получить границу камеры(зависит от текущего разрешения) в мировых координатах
		/// </summary>
		public static Vector3 GetWorldPositionOfBorder(Border border)
		{
			var camera = Camera.main;

			// На основе переданного типа границы экрана вычисляется мировые координаты
			// Вычисление происходит путём перевода координат Viewport, которые всегда определяются промежутком между 0 и 1
			return border switch
			{
				Border.top => camera.ViewportToWorldPoint(Vector3.up),
				Border.bottom => camera.ViewportToWorldPoint(Vector3.down),
				Border.left => camera.ViewportToWorldPoint(Vector3.zero),
				Border.right => camera.ViewportToWorldPoint(Vector3.right),
				_ => Vector3.zero,
			};
		}

		/// <summary>
		/// Получить случайную координату внутри границ экрана
		/// </summary>
		public static Vector3 GetRandomPointInsideBorder()
		{
			var camera = Camera.main;
			// Вычисление происходит путём перевода координат Viewport, которые всегда определяются промежутком между 0 и 1
			return camera.ViewportToWorldPoint(new Vector3(Random.value, Random.value, 0));
		}
	}
}
