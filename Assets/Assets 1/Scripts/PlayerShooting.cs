using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    public int bulletsCount = 10;
    public GameObject bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log("Momentalne mas " + bulletsCount + "bulletov");   

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
                if (bulletsCount <= 0)
                {
                    Debug.Log("Nemozes strielat");
                }
                else
                { 
                    bulletsCount = bulletsCount - 1;

                    Vector3 spawnpos = transform.position + transform.forward;

                Instantiate(bullet, transform.position, transform.rotation );
            }
                if (bulletsCount <= 5)
                {
                    Debug.Log("I need more boolets i need more boolets");
            }
        }
    }
}
