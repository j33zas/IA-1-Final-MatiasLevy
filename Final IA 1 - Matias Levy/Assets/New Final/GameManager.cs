using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    List<BaseUnit> _redTeam = new List<BaseUnit>();
    public List<BaseUnit> redTeam
    {
        get
        {
            return _redTeam;
        }
        set
        {
            _redTeam = value;
        }
    }
    List<BaseUnit> _blueTeam = new List<BaseUnit>();
    public List<BaseUnit> blueTeam
    {
        get
        {
            return _blueTeam;
        }
        set
        {
            _blueTeam = value;
        }
    }
    static GameManager GM;
    public static GameManager gameManager
    {
        get
        {
            return GM;
        }
    }
    int _redSoldierAmount;
    int _blueSoldierAmount;
    public NewSoldier redSoldierPF;
    public NewGeneral redGeneralPF;
    public NewSoldier blueSoldierPF;
    public NewGeneral blueGeneralPF;

    public Transform redGeneralLocation;
    public Transform blueGeneralLocation;

    bool started = false;

    //UI
    public Button StartGame;

    public Text currentRedSoldierAmount;
    public Text currentBlueSoldierAmount;

    public Text redSliderAmount;
    public Text blueSliderAmount;

    public Slider redSoldierAmountSlider;
    public Slider blueSoldierAmountSlider;
    
    public GameObject information;

    public Text VictoryText;
    private void Start()
    {
        GM = this;
        VictoryText.gameObject.SetActive(false);
    }
    private void Update()
    {
        _redSoldierAmount = (int)redSoldierAmountSlider.value;
        _blueSoldierAmount = (int)blueSoldierAmountSlider.value;

        currentBlueSoldierAmount.text = _blueTeam.Count.ToString() + " blue soldiers";
        currentRedSoldierAmount.text = _redTeam.Count.ToString() + " red soldiers";

        redSliderAmount.text = redSoldierAmountSlider.value + " Red Soldiers";
        blueSliderAmount.text = blueSoldierAmountSlider.value + " Blue Soldiers";
        if(started)
        {
            if (blueTeam.Count <= 0)
            {
                VictoryText.gameObject.SetActive(true);
                VictoryText.text = "Red Team WINS! Blu sux!";
                VictoryText.color = Color.red;
            }
            if (redTeam.Count <= 0)
            { 
                VictoryText.gameObject.SetActive(true);
                VictoryText.text = "Blue Team WINS! red sux!";
                VictoryText.color = Color.blue;
            }
        }
    }

    public void PlaySimulation()
    {
        information.gameObject.SetActive(false);
        //instance red general
        var red = Instantiate(redGeneralPF, redGeneralLocation.transform.position, redGeneralLocation.transform.rotation);
        _redTeam.Add(red);
        //instance blue general
        var blue = Instantiate(blueGeneralPF, blueGeneralLocation.transform.position, blueGeneralLocation.transform.rotation);
        _blueTeam.Add(blue);
        //instance blue team
        for (int i = 0; i < _redSoldierAmount; i++)
        {
            var s = Instantiate(redSoldierPF, new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4)) + red.transform.position, Quaternion.identity);
            _redTeam.Add(s);
        }
        //instance red team
        for (int i = 0; i < _blueSoldierAmount; i++)
        {
            var s = Instantiate(blueSoldierPF, new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4)) + blue.transform.position, Quaternion.identity);
            _blueTeam.Add(s);
        }
        started = true;
    }
    public IEnumerator RemoveUnit(BaseUnit unit, float time)
    {
        if (blueTeam.Contains(unit))
        {
            blueTeam.Remove(unit);
            yield return new WaitForSeconds(time);
            unit.gameObject.SetActive(false);
        }

        if (redTeam.Contains(unit))
        {
            redTeam.Remove(unit);
            yield return new WaitForSeconds(time);
            unit.gameObject.SetActive(false);
        }
    }

}
