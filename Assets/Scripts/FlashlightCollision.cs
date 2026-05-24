using UnityEngine;

/// <summary>
/// FlashlightCollision — The Deadliest 20th's Floor
/// Mencegah mesh & cahaya senter menembus tembok dengan
/// melakukan SphereCast dari pivot kamera ke posisi senter.
/// Jika terhalang geometry, senter ditarik mendekati kamera.
///
/// Setup:
///   1. Pasang script ini ke GameObject Camera (child dari Player)
///   2. flashlightTransform → drag child object senter (FlashlightColor)
///   3. Isi collisionLayers → layer tembok/Default
/// </summary>
public class FlashlightCollision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform flashlightTransform;

    [Header("Position Settings")]
    // Posisi normal senter relatif ke kamera (saat tidak ada halangan)
    [SerializeField] private Vector3 defaultLocalPos  = new Vector3(0.3f, -0.25f, 0.5f);
    // Posisi saat terhalang tembok (ditarik mendekati kamera)
    [SerializeField] private Vector3 blockedLocalPos  = new Vector3(0.15f, -0.15f, 0.1f);

    [Header("Collision Settings")]
    [SerializeField] private float   sphereRadius     = 0.08f;
    [SerializeField] private LayerMask collisionLayers;

    [Header("Smoothing")]
    [SerializeField] private float   pullSpeed        = 25f;
    [SerializeField] private float   returnSpeed      = 8f;

    private Vector3 _targetLocalPos;
    private bool    _isBlocked;

    private void Start()
    {
        _targetLocalPos = defaultLocalPos;

        if (flashlightTransform != null)
            flashlightTransform.localPosition = defaultLocalPos;
    }

    private void LateUpdate()
    {
        if (flashlightTransform == null) return;

        // Titik asal: posisi kamera (this.transform)
        // Titik tujuan: posisi world senter saat di defaultLocalPos
        Vector3 origin      = transform.position;
        Vector3 worldTarget = transform.TransformPoint(defaultLocalPos);
        Vector3 direction   = (worldTarget - origin).normalized;
        float   distance    = Vector3.Distance(origin, worldTarget);

        // SphereCast dari kamera ke posisi senter default
        _isBlocked = Physics.SphereCast(origin, sphereRadius,
                                         direction, out RaycastHit hit,
                                         distance, collisionLayers);

        if (_isBlocked)
        {
            // Hitung posisi aman: sebelum titik tabrakan
            float   safeDist     = Mathf.Max(0f, hit.distance - sphereRadius);
            Vector3 safeWorld    = origin + direction * safeDist;
            Vector3 safeLocal    = transform.InverseTransformPoint(safeWorld);

            // Pertahankan offset X & Y senter, hanya Z yang dipengaruhi halangan
            safeLocal.x = Mathf.Lerp(defaultLocalPos.x, blockedLocalPos.x,
                          1f - (safeDist / distance));
            safeLocal.y = Mathf.Lerp(defaultLocalPos.y, blockedLocalPos.y,
                          1f - (safeDist / distance));

            _targetLocalPos = safeLocal;
        }
        else
        {
            _targetLocalPos = defaultLocalPos;
        }

        // Smooth lerp ke target
        float speed = _isBlocked ? pullSpeed : returnSpeed;
        flashlightTransform.localPosition = Vector3.Lerp(
            flashlightTransform.localPosition,
            _targetLocalPos,
            Time.deltaTime * speed
        );
    }

    // ── Gizmos ───────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Vector3 worldTarget = transform.TransformPoint(defaultLocalPos);

        Gizmos.color = _isBlocked ? Color.red : Color.yellow;
        Gizmos.DrawLine(transform.position, worldTarget);
        Gizmos.DrawWireSphere(worldTarget, sphereRadius);
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
