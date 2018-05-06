using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public List<Tile> map = new List<Tile>();
    private GameObject mapObject;
    public string str;

    private Tile prevTile = null;
    private Tile currTile = null;
    private GameObject cursor;
    private GameObject cursorTop;
    private SpriteRenderer cursorSprite;

    private Camera mainCamera;

	// Use this for initialization
	private void Awake ()
    {
        Instance = this;
        mainCamera = Camera.main;
        mapObject = transform.Find("lutia1").gameObject;
	}
	
    private void Start()
    {
        XMLManager.LoadMap(map, mapObject, str);
        currTile = map[0];
        cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
        cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
        cursorTop.transform.position += new Vector3(0, 2f, 0);
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
            cursorTop.transform.position = cursor.transform.position + new Vector3(0, 2f, 0);
            moveCamera(cursor.transform.position);
            if (currTile.hasObj)
                cursorSprite.sortingLayerName = "3";
            else
                cursorSprite.sortingLayerName = "1";
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

    private void moveCamera(Vector3 target)
    {
        if (target.x - mainCamera.transform.position.x > 5.5f)
            mainCamera.transform.position += Vector3.right;
        else if (target.x - mainCamera.transform.position.x < -5)
            mainCamera.transform.position += Vector3.left;

        if (target.y - mainCamera.transform.position.y > 3.5f)
            mainCamera.transform.position += Vector3.up;
        else if (target.y - mainCamera.transform.position.y < -2.5f)
            mainCamera.transform.position += Vector3.down;
    }

}
