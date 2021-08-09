using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : MonoBehaviour
{
    public float speed;

    private void Start()
    {
        StartCoroutine(ChangeDir());
    }
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    IEnumerator ChangeDir()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);
            transform.forward = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        }
    }
}
