using TMPro;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    public int playerNumber = 0;
    public int botNumber = 0;
    public TextMeshProUGUI playernumGUI;
    public TextMeshProUGUI botnumGUI;
    public TextMeshProUGUI winGUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerNumber = Random.Range(0, 7);
            string newstring = "your dice throw is: " + playerNumber;
            playernumGUI.text = newstring;
            Debug.Log(playerNumber);



            {
                botNumber = Random.Range(0, 7);
                string botstring = "PC's throw is: " + botNumber;
                botnumGUI.text = botstring;
                Debug.Log(botNumber);
                if (((botNumber)) == ((playerNumber))) 
                {
                    string win = "You Win";
                    winGUI.text = win;
                }
                if (botNumber > playerNumber) 
                {
                    string win = "You Lose";
                    winGUI.text = win;
                }
                if (botNumber < playerNumber) 
                {
                    string win = "You Lose";
                    winGUI.text = win;
                }

            }
        }

    }
}
