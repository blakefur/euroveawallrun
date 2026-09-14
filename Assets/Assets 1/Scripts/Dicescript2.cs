using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Dicescript2 : MonoBehaviour
{

        public int playerNumber = 0;
        public TextMeshProUGUI playernumGUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerNumber = Random.Range(1, 7);
            string newstring = "your dice throw is: " + playerNumber;
            playernumGUI.text = newstring;
            Debug.Log(playerNumber);
        }
    }

    }



