using UnityEngine;
using UnityEngine.UI; // Perlu ini untuk Image
using UnityEngine.EventSystems;

public class ButtonUnderline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // Drag objek 'UnderlineImage' ke kolom ini di Inspector
    public GameObject underlineObject; 

    // Muncul pas mouse masuk (Highlight)
    public void OnPointerEnter(PointerEventData eventData)
    {
        underlineObject.SetActive(true);
    }

    // Hilang pas mouse keluar
    public void OnPointerExit(PointerEventData eventData)
    {
        underlineObject.SetActive(false);
    }

    // Tetap muncul/makin tegas pas ditekan (Optional)
    public void OnPointerDown(PointerEventData eventData)
    {
        underlineObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Tetap nyala kalau mouse masih di atas tombol
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
            underlineObject.SetActive(true);
    }
}