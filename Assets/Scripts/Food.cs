using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private SpawnObjects SpawnObs;
    
    void Start()
    {
        SpawnObs = GameObject.FindGameObjectWithTag("GameController").GetComponent<SpawnObjects>();
    }
    
    private void OnTriggerEnter2D(Collider2D Col)        // This method is called whenever a collision is detected, and passes the Component which Food collided with
    {
        if (Col.gameObject.tag == "Player" || Col.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            
            if (SpawnObs != null)
            {
                SpawnObs.FoodCount--;
            }
        }
    }
}
