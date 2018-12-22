using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoGeneratorScript : MonoBehaviour {

    public GameObject[] minoPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject GetMino()
    {
        return Instantiate(minoPrefab[Random.Range(0, minoPrefab.Length)], new Vector3(0, 0, 0), Quaternion.identity);
    }

}
