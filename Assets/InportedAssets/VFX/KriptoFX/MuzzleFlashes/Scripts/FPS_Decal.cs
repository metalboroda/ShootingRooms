using UnityEngine;

[ExecuteInEditMode]
public class FPS_Decal : MonoBehaviour
{
    [SerializeField] private bool screenSpaceDecals = true;
    [SerializeField] private float randomScalePercent = 50;

    private Vector3 _startScale;

    void Awake()
    {
        _startScale = transform.localScale;
    }

    private void OnEnable()
    {

        if (Application.isPlaying)
        {
            transform.localRotation = Quaternion.Euler(Random.Range(0, 360), 90, 90);

            float randomScaleRange = Random.Range(_startScale.x - _startScale.x * randomScalePercent * 0.01f,
                _startScale.x + _startScale.x * randomScalePercent * 0.01f);

            transform.localScale = new Vector3(randomScaleRange, screenSpaceDecals ? _startScale.y : 0.001f, randomScaleRange);
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(this.transform.TransformPoint(Vector3.zero), this.transform.rotation, this.transform.lossyScale);
        Gizmos.color = new Color(1, 1, 1, 1);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
