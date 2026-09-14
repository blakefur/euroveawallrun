using UnityEngine;

public class logetk : MonoBehaviour
    

{
    public int maxAmount = 10000;
    public int unlimited = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (unlimited <= maxAmount)
        {
            unlimited += 1;
            Debug.Log(unlimited);
        }
    }
}
