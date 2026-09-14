using UnityEngine;

public class PlayerGooning : MonoBehaviour
{

    public int bulletsCount = 100;
    public GameObject bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



        Vector3 spawnpos = transform.position + transform.forward;

        Instantiate(bullet, transform.position, transform.rotation);
    }
}
