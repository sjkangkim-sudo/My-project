using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float MaxHP = 3;
    float HP;

    void Start()
    {
        HP = MaxHP;
    }


    void Update()
    {
        
    }
    public bool Hit(float damage)
    {
        HP -=damage;
        if (HP < 0)
        {
            HP=0;
        }
        return HP>0;
    }
}
