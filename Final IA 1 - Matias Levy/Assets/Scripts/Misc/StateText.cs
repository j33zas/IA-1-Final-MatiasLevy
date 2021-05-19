using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateText : MonoBehaviour
{
    TextMesh TM;
    BaseUnit BU;
    public string text;

    void Start()
    {
        TM = GetComponent<TextMesh>();
    }
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        TM.text = text;
    }
}
