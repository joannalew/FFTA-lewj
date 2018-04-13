using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public List<Tile> map = new List<Tile>();
    private GameObject mapObject;

    private Tile prevTile = null;
    private Tile currTile = null;
    private GameObject cursor;
    private SpriteRenderer cursorSprite;

	// Use this for initialization
	private void Awake ()
    {
        Instance = this;
        mapObject = transform.Find("giza1").gameObject;
	}
	
    private void Start()
    {
        XMLManager.LoadMap(map, mapObject);
        currTile = map[0];
        cursor = (GameObject)Instantiate(PrefabHolder.Instance.Cursor, currTile.transform.position, Quaternion.identity);
        cursorSprite = cursor.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {
        Controls();
	}

    private void moveCursor(int direction)
    {
        prevTile = currTile;
        currTile = currTile.neighbors[direction];
        if (currTile != null)
        {
            cursor.transform.position = currTile.transform.position;
            /*
            if (currTile.hasObj)
                cursorSprite.sortingLayerName = "3";
            else
                cursorSprite.sortingLayerName = "1";
            */
        }
        else
            currTile = prevTile;
    }

    public void Controls()
    { 
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            moveCursor(0);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            moveCursor(1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            moveCursor(2);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            moveCursor(3);
    }


}
