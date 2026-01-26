using System.Collections;
using UnityEngine;

public class HtFlash : MonoBehaviour
{
    [SerializeField] private Material _flashMaterial;
    [SerializeField] private float _flashDuration = 0.1f;

    private SpriteRenderer _spriteRenderer;
    private Material _originalMaterial;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalMaterial = _spriteRenderer.material;
    }

    public void Flash()
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        _spriteRenderer.material = _flashMaterial;
        yield return new WaitForSecondsRealtime(_flashDuration); // Use Realtime because of HitStop!
        _spriteRenderer.material = _originalMaterial;
        _flashCoroutine = null;
    }
}