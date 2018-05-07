using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public List<Tile> map = new List<Tile>();
    private GameObject mapObject;

    private Tile prevTile = null;
    private Tile currTile = null;
    private GameObject cursor;
    private GameObject cursorTop;
    private Vector3 cursorOffset = new Vector3(0, 2.5f, 0);
    private SpriteRenderer cursorSprite;

    private Character player;
    private List<Character> playerList = new List<Character>();
    private Tile playerTile = null;

    private Camera mainCamera;

	// Use this for initialization
	private void Awake ()
    {
        Instance = this;
        mainCamera = Camera.main;
        mapObject = transform.Find("mapObject").gameObject;
	}
	
    private void Start()
    {
        loadLevel(1);
    }

    // Update is called once per frame
    void Update () {
        Controls();
    }

    private void loadLevel(int level)
    {
        if (level == 1)
        {
            XMLManager.LoadMap(1, map, mapObject);

            currTile = map[0];
            cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
            cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
            cursorTop.transform.position += cursorOffset;
            cursorSprite = cursor.GetComponent<SpriteRenderer>();

            setEnems(0, 19, 0);
            setEnems(1, 47, 0);
            setEnems(1, 77, 3);
            setEnems(1, 108, 3);
            setEnems(2, 80, 3);

            setChars(1, 12, 2);
            setChars(2, 21, 2);
            setChars(3, 11, 2);
            setChars(4, 1, 2);
            setChars(5, 10, 2);
            setChars(0, 22, 2);
        }
        else if (level == 2)
        {
            XMLManager.LoadMap(2, map, mapObject);

            currTile = map[120];
            cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
            cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
            cursorTop.transform.position += cursorOffset;
            cursorSprite = cursor.GetComponent<SpriteRenderer>();

            setEnems(1, 125, 2);
            setEnems(3, 159, 2);
            setEnems(3, 113, 2);

            setChars(1, 122, 0);
            setChars(2, 151, 0);
            setChars(3, 136, 0);
            setChars(4, 152, 0);
            setChars(5, 137, 0);
            setChars(0, 121, 0);        
        }
        else
        {
            XMLManager.LoadMap(3, map, mapObject);

            currTile = map[57];
            cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
            cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
            cursorTop.transform.position += cursorOffset;
            cursorSprite = cursor.GetComponent<SpriteRenderer>();

            setEnems(4, 39, 2);
            setEnems(5, 78, 2);
            setEnems(6, 61, 2);
            setEnems(7, 105, 2);
            setEnems(5, 31, 2);
            setEnems(4, 120, 2);

            setChars(1, 59, 0);
            setChars(2, 71, 0);
            setChars(3, 84, 0);
            setChars(4, 85, 0);
            setChars(5, 72, 0);
            setChars(0, 58, 0);
        }

        mainCamera.transform.position = new Vector3(currTile.transform.position.x, currTile.transform.position.y, -10);
    }

    private void setChars(int playIndex, int mapIndex, int face)
    {
        playerTile = map[mapIndex];
        player = ((GameObject)Instantiate(PrefabHolder.Instance.Player[playIndex])).GetComponent<Character>();
        player.transform.position = playerTile.transform.position + player.playerOffset;
        player.faceDir(face);
        player.tileLoc = playerTile;
        player.charSprite.sortingOrder = playerTile.sort + 3;
        playerList.Add(player);
    }

    private void setEnems(int playIndex, int mapIndex, int face)
    {
        playerTile = map[mapIndex];
        player = ((GameObject)Instantiate(PrefabHolder.Instance.Enemies[playIndex])).GetComponent<Character>();
        player.transform.position = playerTile.transform.position + player.playerOffset;
        player.faceDir(face);
        player.tileLoc = playerTile;
        player.charSprite.sortingOrder = playerTile.sort + 3;
        playerList.Add(player);
    }

    private void selectChar(Tile currTile)
    {
        foreach (Character chara in playerList)
        {
            if (chara.tileLoc == currTile)
            {
                player = chara;
                playerTile = chara.tileLoc;
            }
        }
    }

    private void moveCursor(int direction)
    {
        prevTile = currTile;
        currTile = currTile.neighbors[direction];
        if (currTile != null)
        {
            cursor.transform.position = currTile.transform.position;
            cursorTop.transform.position = cursor.transform.position + cursorOffset;
            cursorSprite.sortingOrder = currTile.sort + 1;

            moveCamera(currTile.transform.position);
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (player.Move(map, playerTile, currTile))
                playerTile = currTile;
        }

        if (Input.GetKeyDown(KeyCode.S))
            selectChar(currTile);
    }

    private void moveCamera(Vector3 target)
    {
        if (target.x - mainCamera.transform.position.x > 3f)
            mainCamera.transform.position += Vector3.right;
        else if (target.x - mainCamera.transform.position.x > 5f)
            mainCamera.transform.position += 2 * Vector3.right;
        else if (target.x - mainCamera.transform.position.x < -3f)
            mainCamera.transform.position += Vector3.left;

        if (target.y - mainCamera.transform.position.y > 1.5f)
            mainCamera.transform.position += Vector3.up;
        else if (target.y - mainCamera.transform.position.y > 2.5f)
            mainCamera.transform.position += 2 * Vector3.up;
        else if (target.y - mainCamera.transform.position.y < -1.5f)
            mainCamera.transform.position += Vector3.down;
    }

}
