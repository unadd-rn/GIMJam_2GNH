using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;

    void Start()
    {
        bool hasSave = PlayerPrefs.HasKey("SavedLevel");
        continueButton.SetActive(hasSave);
    }
}