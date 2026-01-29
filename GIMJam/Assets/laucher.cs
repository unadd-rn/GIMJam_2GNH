using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class laucher : MonoBehaviour 
{
    private bool isPlayerOnLauncher = false;
    private RobotController.RobotController playerController;
    public TextMeshProUGUI coinText;

    void Start()
    {
        FinalLevelManager.instance.OnLaunch(false);
    }
    void Update() 
    {
        // Cek apakah player di atas launcher DAN menekan tombol jump
        if (isPlayerOnLauncher && playerController != null) 
        {
            if (coinText != null) coinText.text ="Jump";
            if (playerController.ExternalJumpDown || playerController.ExternalJumpHeld) 
            {
                // Panggil OnLaunch hanya sekali
                FinalLevelManager.instance.OnLaunch(true);
                MusicManager.Instance.StopMusic();
                
                // Matikan isPlayerOnLauncher biar gak kepanggil berkali-kali tiap frame
                isPlayerOnLauncher = false; 
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerOnLauncher = true;
            playerController = other.GetComponent<RobotController.RobotController>();
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerOnLauncher = false;
            playerController = null;
        }
    }
}
