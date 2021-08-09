using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerCheck : MonoBehaviour
{
    public LayerMask blueteam;
    public LayerMask redteam;

    Collider[] blueTeamarray;
    Collider[] redTeamarray;

    public Text BlueWon;
    public Text RedWon;

    public Text blueCounter;
    public Text redCounter;

    void Start()
    {
        RedWon.gameObject.SetActive(false);
        BlueWon.gameObject.SetActive(false);
    }

    void Update()
    {
        blueTeamarray = Physics.OverlapSphere(transform.position, 20, blueteam);

        redTeamarray = Physics.OverlapSphere(transform.position, 20, redteam);

        blueCounter.text = "Blue Team: " + blueTeamarray.Length;

        redCounter.text = "Red Team: " + redTeamarray.Length;

        if(blueTeamarray.Length <= 0)
            RedWon.gameObject.SetActive(true);

        if (redTeamarray.Length <= 0)
            BlueWon.gameObject.SetActive(true);
    }
}
