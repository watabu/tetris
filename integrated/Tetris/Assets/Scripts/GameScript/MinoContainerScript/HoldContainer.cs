using UnityEngine;
using System.Collections;

public class HoldContainer : MonoBehaviour
{

    GameObject mino;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Register(GameObject mino_)
    {
        if (mino_ == null)
        {
            Debug.LogWarning("hold container mino is null");
            return;
        }
        mino = mino_;
        GenerateSprite();
    }

    public GameObject GetMino()
    {
        return mino;
    }

    public bool HasMino() { return mino != null; }

    void GenerateSprite()
    {
        //以前生成していたスプライトを削除
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        MinoScript minoS = mino.GetComponent<MinoScript>();
        GameObject sprite = minoS.AsSprite();
        sprite.transform.position += transform.position;
        sprite.transform.SetParent(transform);
        mino.transform.SetParent(transform);
    }

}
