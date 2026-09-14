using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Shop : MonoBehaviour
{
    public int priceflame = 10;
    public int priceskyro = 25000;
    public int pricesnail = 5;
    public int pricepapear = 25;
    public int peniaze = 26000;
    public int itemskyro = 0;
    public int itemflame = 0;
    public int itemsnail = 0;
    public int itempapear = 0;
    public TextMeshProUGUI peniazeGUI;
    public TextMeshProUGUI ItemsGUI;

    void Start()
    {

    

    }

    // Update is called once per frame
    void Update()
    {
     Debug.Log(peniaze);
    peniazeGUI.text = "Money: " + peniaze;
        ItemsGUI.text = "Snail: " + itemsnail + " Wisdom: " + itemflame + " Skyro: " + itemskyro + " Papear: " + itempapear;
    }

    public void flame()
    {
        if (peniaze >= priceflame)
        {
            itemflame += 1;
            peniaze -= priceflame;
        }
    }

    public void snail()
    {
        if (peniaze >= pricesnail)
        {
            itemsnail += 1;
            peniaze -= pricesnail;
        }
    }

    public void skyro()
    {
        if  (peniaze >= priceskyro)
            {
            itemskyro += 1;
            peniaze -= priceskyro;
        }
        
    }
    public void papear()
    {
        if (peniaze >= pricepapear)
        {
            itempapear += 1;
            peniaze -= pricepapear;
        }
    }
}



