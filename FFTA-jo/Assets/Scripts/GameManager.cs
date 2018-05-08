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

    private Enemy enemy;
    private List<Enemy> enemyList = new List<Enemy>();
    private Tile enemyTile = null;

    private Camera mainCamera;

	private void Awake ()
    {
        Instance = this;
        mainCamera = Camera.main;
        mapObject = transform.Find("mapObject").gameObject;
	}
	
    private void Start()
    {
        // 1 = giza, 2 = lutia #1, 3 = lutia #2
        loadLevel(1);
    }

    void Update () {
        Controls();
    }

    // Load map from XML; initiate cursor, enemies, and characters; set camera position
    // Character selected by default is Marche
    private void loadLevel(int level)
    {
        if (level == 1)
        {
            XMLManager.LoadMap(1, map, mapObject);

            // set cursor at (0, 0); down from viera
            currTile = map[0];
            cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
            cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
            cursorTop.transform.position += cursorOffset;
            cursorSprite = cursor.GetComponent<SpriteRenderer>();

            setEnems(0, 19, 0);         // fairy,       (1, 9), facing left
            setEnems(1, 47, 0);         // blue goblin, (4, 7)
            setEnems(1, 77, 3);         // blue goblin, (7, 5), facing down
            setEnems(1, 108, 3);        // blue goblin, (10, 10)
            setEnems(2, 80, 3);         // red goblin,  (7, 8)

            setChars(1, 12, 2);         // montblanc,   (1, 2), facing right
            setChars(2, 21, 2);         // soldier,     (2, 1)
            setChars(3, 11, 2);         // monk,        (1, 1)
            setChars(4, 1, 2);          // white mage,  (0, 1)
            setChars(5, 10, 2);         // archer,      (1, 0)
            setChars(0, 22, 2);         // marche,      (2, 2)
        }
        else if (level == 2)
        {
            XMLManager.LoadMap(2, map, mapObject);

            // set cursor at (8, 12); front of marche
            currTile = map[120];
            cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
            cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
            cursorTop.transform.position += cursorOffset;
            cursorSprite = cursor.GetComponent<SpriteRenderer>();

            setEnems(1, 125, 2);        // blue goblin, (9, 2), facing right
            setEnems(3, 159, 2);        // red panther, (11, 6)
            setEnems(3, 113, 2);        // red panther, (8, 5)

            setChars(1, 122, 0);        // montblanc,   (8, 14), facing left
            setChars(2, 151, 0);        // soldier,     (10, 13) 
            setChars(3, 136, 0);        // monk,        (9, 13)
            setChars(4, 152, 0);        // white mage,  (10, 14)
            setChars(5, 137, 0);        // archer,      (9, 14)
            setChars(0, 121, 0);        // marche,      (8, 13)
        }
        else
        {
            XMLManager.LoadMap(3, map, mapObject);

            // set cursor at (6, 10); front of marche
            currTile = map[57];
            cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
            cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
            cursorTop.transform.position += cursorOffset;
            cursorSprite = cursor.GetComponent<SpriteRenderer>();

            setEnems(4, 39, 2);         // soldier,     (5, 3), facing right
            setEnems(5, 78, 2);         // thief,       (8, 5)
            setEnems(6, 61, 2);         // archer,      (7, 1)
            setEnems(7, 105, 2);        // white mage,  (10, 6)
            setEnems(5, 31, 2);         // thief,       (4, 5)
            setEnems(4, 120, 2);        // soldier,     (11, 8)

            setChars(1, 59, 0);         // montblanc,   (6, 12), facing left
            setChars(2, 71, 0);         // soldier,     (7, 11)
            setChars(3, 84, 0);         // monk,        (8, 11)
            setChars(4, 85, 0);         // white mage,  (8, 12)
            setChars(5, 72, 0);         // archer,      (7, 12)
            setChars(0, 58, 0);         // marche,      (6, 11)
        }

        mainCamera.transform.position = new Vector3(currTile.transform.position.x, currTile.transform.position.y, -10);
    }

    // create character on map
    // Character created is PrefabHolder->Player[playindex]
    // At map[mapIndex]; sprite facing in "face" direction
    private void setChars(int playIndex, int mapIndex, int face)
    {
        playerTile = map[mapIndex];
        player = ((GameObject)Instantiate(PrefabHolder.Instance.Player[playIndex])).GetComponent<Character>();
        player.transform.position = playerTile.transform.position + player.charOffset;
        player.faceDir(face);
        player.tileLoc = playerTile;
        playerTile.occupied = player.group;
        player.charSprite.sortingOrder = playerTile.sort + 3;
        playerList.Add(player);
    }

    // create enemy on map
    // Enemy created is PrefabHolder->Enemies[enemIndex]
    // At map[mapIndex]; sprite facing in "face" direction
    private void setEnems(int enemIndex, int mapIndex, int face)
    {
        enemyTile = map[mapIndex];
        enemy = ((GameObject)Instantiate(PrefabHolder.Instance.Enemies[enemIndex])).GetComponent<Enemy>();
        enemy.transform.position = enemyTile.transform.position + enemy.charOffset;
        enemy.faceDir(face);
        enemy.tileLoc = enemyTile;
        enemyTile.occupied = enemy.group;
        enemy.charSprite.sortingOrder = enemyTile.sort + 3;
        enemyList.Add(enemy);
    }

    // select the character at the current tile
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

    // move the cursor one space in the given direction
    // 0 = left, 1 = up, 2 = right, 3 = down
    private void moveCursor(int direction)
    {
        prevTile = currTile;
        currTile = currTile.neighbors[direction];
        if (currTile != null)
        {
            cursor.transform.position = currTile.transform.position;
            cursorTop.transform.position = cursor.transform.position + cursorOffset;
            cursorSprite.sortingOrder = currTile.sort + 2;

            moveCamera(currTile.transform.position);
        }
        else
            currTile = prevTile;
    }

    // highlight the List of Tiles a certain color
    // 0 = no color, 1 = blue, 2 = red, 3 = green
    private void glowTiles(List<Tile> tiles, int color)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.occupied == 0)
                tile.tileHighlight(color);
        }
    }

    // Keyboard Controls
    public void Controls()
    { 
        // use arrow keys to move cursor around
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            moveCursor(0);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            moveCursor(1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            moveCursor(2);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            moveCursor(3);

        // press "z" to move selected character to another tile
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (player.Move(map, playerTile, currTile))
                playerTile = currTile;
        }

        // press "s" to select the character at the current tile (where cursor is)
        if (Input.GetKeyDown(KeyCode.S))
            selectChar(currTile);

        // press "g" to cause tiles to glow blue (where character can move)
        // press "r" to disable tile glow
        if (Input.GetKeyDown(KeyCode.G))
            glowTiles(player.AstarGlow(map), 1);
        if (Input.GetKeyDown(KeyCode.R))
            glowTiles(map, 0);
    }

    // Camera moves around to follow the cursor
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
