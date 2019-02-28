using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//離散的に動くRigitBody
//使わないかも
public class TileRigitBody2D : MonoBehaviour
{

    public Grid grid;

    Vector2Int position;
    Vector2Int speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        position += speed;
        transform.position = grid.CellToWorld(new Vector3Int(position.x, position.y,0));
    }
}
