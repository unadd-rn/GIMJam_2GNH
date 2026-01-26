using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Simpan material asli biar bisa dibalikin
        originalMaterial = spriteRenderer.material;
    }

    public void Flash()
    {
        // Kalau sedang flash, stop dulu yang lama biar nggak tabrakan
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Ganti ke material putih
        spriteRenderer.material = flashMaterial;

        // Tunggu sebentar (unscaledTime supaya tetap jalan pas HitStop)
        yield return new WaitForSeconds(flashDuration);

        // Balikin ke material awal
        spriteRenderer.material = originalMaterial;
        flashCoroutine = null;
    }
}
