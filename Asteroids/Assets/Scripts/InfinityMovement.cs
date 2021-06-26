using UnityEngine;

public class InfinityMovement : MonoBehaviour
{
    private bool _isWrapping;
    private Renderer[] _renderers;

    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    bool CheckRenderers()
    {
        foreach (var renderer in _renderers)
            // If at least one render is visible, return true
            if (renderer.isVisible)
                return true;

        // Otherwise, the object is invisible
        return false;
    }

    void Update()
    {
        var isVisible = CheckRenderers();

        if (isVisible)
        {
            _isWrapping = false;
            return;
        }

        if (_isWrapping)
            return;

        var cam = Camera.main;
        var viewportPosition = cam.WorldToViewportPoint(transform.position);
        var newPosition = transform.position;

        if (viewportPosition.x > 1 || viewportPosition.x < 0)
        {
            newPosition.x = -newPosition.x;
            _isWrapping = true;
        }

        if (viewportPosition.y > 1 || viewportPosition.y < 0)
        {
            newPosition.y = -newPosition.y;
            _isWrapping = true;
        }

        transform.position = newPosition;
    }
}
