using System.Collections;
using UnityEngine;

public class EnemyLeftRight : MonoBehaviour
 
{
    public Rigidbody myRB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      

        myRB.linearVelocity = transform.right;
        StartCoroutine(MovingCorot());
    }

    IEnumerator MovingCorot()
    { 
    yield return new WaitForSeconds(2);


        myRB.linearVelocity = -1 * transform.right;
    }
    
}
