using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    List<BaseUnit> redTeam = new List<BaseUnit>();
    List<BaseUnit> BlueTeam = new List<BaseUnit>();
    int _redSoldierAmount;
    int _blueSoldierAmount;
    public NewSoldier redSoldierPF;
    public NewGeneral redGeneralPF;
    public NewSoldier blueSoldierPF;
    public NewGeneral blueGeneralPF;

    public Transform redGeneralLocation;
    public Transform blueGeneralLocation;

    //UI
    public Button StartGame;

    public Text currentRedSoldierAmount;
    public Text currentBlueSoldierAmount;

    public Text redSliderAmount;
    public Text blueSliderAmount;

    public Slider redSoldierAmountSlider;
    public Slider blueSoldierAmountSlider;
    
    public GameObject information;

    private void Update()
    {
        _redSoldierAmount = (int)redSoldierAmountSlider.value;
        _blueSoldierAmount = (int)blueSoldierAmountSlider.value;

        currentBlueSoldierAmount.text = BlueTeam.Count.ToString() + " Amount of blue soldiers";
        currentRedSoldierAmount.text = redTeam.Count.ToString() + " Amount of red soldiers";

        redSliderAmount.text = redSoldierAmountSlider.value + " Red Soldiers";
        blueSliderAmount.text = blueSoldierAmountSlider.value + " Blue Soldiers";
    }

    public void PlaySimulation()
    {
        information.gameObject.SetActive(false);
        //instance red general
        var red = Instantiate(redGeneralPF, redGeneralLocation.transform.position, redGeneralLocation.transform.rotation);
        redTeam.Add(red);
        //instance blue general
        var blue = Instantiate(blueGeneralPF, blueGeneralLocation.transform.position, blueGeneralLocation.transform.rotation);
        BlueTeam.Add(blue);
        //instance blue team
        for (int i = 0; i < _redSoldierAmount; i++)
        {
            var s = Instantiate(redSoldierPF, new Vector3(Random.Range(redGeneralPF.obsAvoidanceRadious, redGeneralPF.AttackDistance), 0, Random.Range(redGeneralPF.obsAvoidanceRadious, redGeneralPF.AttackDistance)) + red.transform.position, Quaternion.identity);
            redTeam.Add(s);
        }
        //instance red team
        for (int i = 0; i < _blueSoldierAmount; i++)
        {
            var s = Instantiate(blueSoldierPF, new Vector3(Random.Range(blueGeneralPF.obsAvoidanceRadious, blueGeneralPF.AttackDistance), 0, Random.Range(blueGeneralPF.obsAvoidanceRadious, blueGeneralPF.AttackDistance)) + blue.transform.position, Quaternion.identity);
            BlueTeam.Add(s);
        }
    }
}
