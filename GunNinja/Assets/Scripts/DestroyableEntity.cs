using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableEntity : MonoBehaviour
{
    protected float health;
    protected float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        InitializeStats();
    }

    protected virtual void InitializeStats()
    {
        maxHealth = 1;
        health = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
