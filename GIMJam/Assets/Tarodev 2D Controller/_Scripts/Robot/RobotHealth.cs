using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHealth : MonoBehaviour
{
    public float health=3;
    public GameObject healthbar;
    public GameObject bar1;
    public GameObject bar2;
    public GameObject bar3;
    // Start is called before the first frame update
    void Start()
    {
        health = 3;
        if(healthbar != null){
            healthbar.SetActive(true);}
        if(bar1 != null){
            bar1.SetActive(true);}
        if(bar2 != null){
            bar2.SetActive(true);}
        if(bar3 != null){
            bar3.SetActive(true);}
    }

    // Update is called once per frame
    void Update()
    {//scriptnya yang ngurangin health dia ada di enemybullet
        if(health == 2)
        {
            if(bar3 != null){
            bar3.SetActive(false);}
        }

        if(health == 1)
        {
            if(bar2 != null){
            bar2.SetActive(false);}
        }

        if(health == 0)
        {
            if(bar1 != null){
            bar1.SetActive(false);}
        }
    }
}
