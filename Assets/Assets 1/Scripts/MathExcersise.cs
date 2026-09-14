using Unity.VisualScripting;
using UnityEngine;

public class MathExcersise : MonoBehaviour
{
    public int bounce = 0;
    public int fizz = 3;
    public int buzz = 5;
    public int zvys = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        bounce++;
    }
        // Update is called once per frame
        void Update()
            {
            if (bounce % 3 == 0)
            {
                Debug.Log("Fizz");
            }
            if (bounce % 5 == 0)
            {
                Debug.Log("Buzz");

            }
            if (bounce % 3 == 0 && bounce % 5 == 0)
            {
                Debug.Log("FizzBuzz");
            }
            else
            {
                Debug.Log("badump" + bounce);
            }
        }
    }

