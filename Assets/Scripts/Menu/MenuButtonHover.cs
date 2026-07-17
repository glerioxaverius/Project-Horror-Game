using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Adds a hover highlight effect to menu buttons, matching the horror game aesthetic.
/// Attach this to each Button GameObject alongside its Button component.
/// </summary>
[RequireComponent(typeof(Button))]
public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Colors")]
    [Tooltip("Normal text color (off-white)")]
    public Color normalColor = new Color(0.85f, 0.82f, 0.75f, 1f);   // warm off-white

    [Tooltip("Hovered text color (bright white/cream)")]
    public Color hoverColor  = new Color(1f, 0.97f, 0.88f, 1f);       // bright cream

    [Tooltip("Clicked text color (muted amber)")]
    public Color clickColor  = new Color(0.9f, 0.75f, 0.45f, 1f);     // amber flash

    [Header("Scale")]
    [Tooltip("Scale multiplier when hovered (e.g. 1.05 = 5% larger)")]
    public float hoverScale = 1.06f;

    [Tooltip("Speed of the scale transition")]
    public float scaleSpeed = 8f;

    // ── Private State ────────────────────────────────────────────────────────

    private Text      _label;
    private Vector3   _originalScale;
    private Vector3   _targetScale;
    private bool      _isHovered = false;

    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    private void Awake()
    {
        _label = GetComponentInChildren<Text>();
        _originalScale = transform.localScale;
        _targetScale   = _originalScale;

        if (_label != null)
            _label.color = normalColor;
    }

    private void Update()
    {
        // Smooth scale lerp every frame
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            _targetScale,
            Time.unscaledDeltaTime * scaleSpeed   // unscaledDeltaTime so it works while paused
        );
    }

    // ── Pointer Events ───────────────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        _targetScale = _originalScale * hoverScale;

        if (_label != null)
            _label.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        _targetScale = _originalScale;

        if (_label != null)
            _label.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_label != null)
        {
            _label.color = clickColor;
            // Reset back to hover color after a short moment
            Invoke(nameof(ResetToHoverColor), 0.15f);
        }
    }

    private void ResetToHoverColor()
    {
        if (_label != null)
            _label.color = _isHovered ? hoverColor : normalColor;
    }
}
