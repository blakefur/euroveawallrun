using TMPro;
using UnityEngine;

public class BotScript : MonoBehaviour
{
    public int botNumber = 0;
    public TextMeshProUGUI playernumGUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            botNumber = Random.Range(0, 7);
            string newstring = "PC's throw is: " + botNumber;
            playernumGUI.text = newstring;
            Debug.Log(botNumber);
        }
    }

}
