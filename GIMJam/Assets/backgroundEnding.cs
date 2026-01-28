using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundEnding : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform[] backgrounds; // Masukkan 2 Image tadi di sini
    public float scrollSpeed = 50f;
    public float offset = 0.5f;
    
    private float _textureHeight;

    void Start()
    {
        // Ambil tinggi gambar (asumsi kedua gambar ukurannya sama)
        _textureHeight = backgrounds[0].rect.height; //itu 0.35 yang buat aku scale imagenya biar pas di cam
        
        // Posisikan gambar kedua tepat di atas gambar pertama
        ResetPositions();
    }

    void Update()
    {
        // Gerakkan semua gambar ke atas
        foreach (RectTransform bg in backgrounds)
        {
            bg.anchoredPosition += Vector2.down * scrollSpeed * Time.deltaTime;
            
            // Cek jika gambar sudah benar-benar keluar dari atas kamera
            // Karena pivot di bawah (0), maka jika pos > tinggi gambar, berarti sudah lewat
            if (bg.anchoredPosition.y <= _textureHeight-offset)
            {
                RepositionBackground(bg);
            }
        }
    }

    void ResetPositions()
    {
        backgrounds[0].anchoredPosition = Vector2.zero;
        backgrounds[1].anchoredPosition = new Vector2(0, -_textureHeight-offset);
    }

    void RepositionBackground(RectTransform bg)
    {
        // Cari gambar yang satunya lagi
        RectTransform otherBG = (bg == backgrounds[0]) ? backgrounds[1] : backgrounds[0];
        
        // Pindahkan gambar yang sudah lewat ke belakang (bawah) gambar yang sedang tampil
        // Dikurang _textureHeight agar pas menyambung di bawahnya
        bg.anchoredPosition = new Vector2(0, otherBG.anchoredPosition.y + _textureHeight);

        
    }
}
