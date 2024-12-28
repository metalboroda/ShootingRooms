using UnityEngine;

public class PlayerRaycastProviderComponent
{
    private readonly LayerMask _aimLayer;
    private readonly LayerMask _ignoreLayer;
    private readonly float _defaultRayDistance = 100f;

    private readonly Camera _mainCamera;

    public PlayerRaycastProviderComponent(Camera mainCamera, LayerMask aimLayer, LayerMask ignoreLayer, float defaultRayDistance = 100f)
    {
        _mainCamera = mainCamera;
        _aimLayer = aimLayer;
        _ignoreLayer = ignoreLayer;
        _defaultRayDistance = defaultRayDistance;
    }

    public Vector3 GetTargetPoint()
    {
        Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, _aimLayer))
        {
            if (IsIgnoredLayer(hit.collider.gameObject.layer) == false)
            {
                return hit.point;
            }
        }
        return ray.GetPoint(_defaultRayDistance);
    }

    private bool IsIgnoredLayer(int layer)
    {
        return (_ignoreLayer.value & (1 << layer)) != 0;
    }
}