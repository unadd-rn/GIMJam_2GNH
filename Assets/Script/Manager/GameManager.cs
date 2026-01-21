using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // List untuk menyimpan nama-nama trigger yang sudah diselesaikan pemain
    // Menggunakan HashSet lebih efisien untuk pengecekan Contains() jika daftar trigger sangat banyak
    public HashSet<string> HaveDone = new HashSet<string>(); 
    // Atau bisa juga pakai List<string> HaveDone = new List<string>(); kalau jumlahnya tidak terlalu banyak

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Penting: Agar GameManager tetap ada antar scene
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Jika ada instance lain dari GameManager (misalnya dari scene berbeda), hancurkan yang baru
            Destroy(gameObject);
        }
    }

    // Metode untuk menambahkan trigger ke daftar HaveDone
    public void AddTrigger(string triggerName)
    {
        if (!string.IsNullOrEmpty(triggerName) && !HaveDone.Contains(triggerName))
        {
            HaveDone.Add(triggerName);
            Debug.Log($"GameManager: Trigger '{triggerName}' ditambahkan ke HaveDone.");
            // Di sini kamu bisa memicu event jika ada objek lain yang perlu bereaksi secara langsung
        }
    }

    // Metode untuk memeriksa apakah sebuah trigger sudah ada di daftar HaveDone
    public bool HasTrigger(string triggerName)
    {
        return HaveDone.Contains(triggerName);
    }

    // Metode untuk memeriksa apakah semua trigger dalam sebuah daftar sudah ada di HaveDone
    public bool HasAllTriggers(List<string> requiredTriggers)
    {
        if (requiredTriggers == null || requiredTriggers.Count == 0)
        {
            return true; // Jika tidak ada trigger yang diperlukan, anggap kondisinya terpenuhi
        }

        foreach (string trigger in requiredTriggers)
        {
            if (!HaveDone.Contains(trigger))
            {
                return false; // Ada satu trigger yang belum terpenuhi
            }
        }
        return true; // Semua trigger yang diperlukan sudah terpenuhi
    }

    // --- Optional: Untuk debugging atau menyimpan/memuat game ---
    public void PrintHaveDoneTriggers()
    {
        Debug.Log("--- Current HaveDone Triggers ---");
        if (HaveDone.Count == 0)
        {
            Debug.Log("No triggers completed yet.");
        }
        foreach (string trigger in HaveDone)
        {
            Debug.Log($"- {trigger}");
        }
        Debug.Log("----------------------------------");
    }
}