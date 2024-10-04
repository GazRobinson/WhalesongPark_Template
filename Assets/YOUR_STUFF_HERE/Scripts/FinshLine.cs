using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinshLine : MonoBehaviour
{
    [SerializeField ]BikeGame bikeGame;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
            bikeGame.OnFinishReached();

            print("aaaaaaaaaaaaaaa");
        }
    }

}
