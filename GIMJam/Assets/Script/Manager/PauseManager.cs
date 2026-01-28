using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PauseManager
{
    public static void ToggleEntities(bool state)
    {
        foreach (var p in Object.FindObjectsOfType<MonoBehaviour>())
        {
            if (p is IPausable pausable)
            {
                pausable.SetPaused(!state);
            }
        }
    }
}
