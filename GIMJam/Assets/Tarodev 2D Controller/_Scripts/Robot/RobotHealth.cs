using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHealth : MonoBehaviour
{
    public float Health=3;
    public GameObject healthbar;
    public GameObject bar1;
    public GameObject bar2;
    public GameObject bar3;
    // Start is called before the first frame update
    void Start()
    {
        Health = 3;
        if (healthbar != null){
        healthbar.SetActive(true);}
        if (bar1 != null){
        bar1.SetActive(true);}
        if (bar2 != null){
        bar2.SetActive(true);}
        if (bar3 != null){
        bar3.SetActive(true);}
        Debug.Log("Your Health = 3");
    }

    // Update is called once per frame
    void Update()
    {
        if (Health == 2)
        {
            if (bar3 != null){
            bar3.SetActive(false);}
        }

        if (Health == 1)
        {
            if (bar2 != null){
            bar3.SetActive(false);}
        }

        if (Health == 0)
        {
            if (bar1 != null){
            bar3.SetActive(false);}
        }
    }
}
