using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : DestroyableEntity
{
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    protected override void InitializeStats()
    {
        health = 100;
        maxHealth = 100;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
