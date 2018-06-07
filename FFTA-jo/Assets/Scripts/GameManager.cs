using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // level selection 
    public int mLevel = 1;
    public List<Tile> map = new List<Tile>();
    private GameObject mapObject;

    // cursor
    private Tile prevTile = null;
    private Tile currTile = null;
    private GameObject cursor;
    private GameObject cursorTop;
    private Vector3 cursorOffset = new Vector3(0, 2.5f, 0);
    private SpriteRenderer cursorSprite;

    // players & enemy
    private List<Character> charList = new List<Character>();
    private Character player;
    private Tile playerTile = null;
    private Character enemy;
    private int damage;

    private bool first = true;                      // reset menu hotfix
    private bool enemyRunning = false;
    System.Random randomHit;

    // UI
    private Camera mainCamera;
    private GameObject actionMenu;
    private bool moving;
    private GameObject leftInfo;
    private GameObject rightInfo;
    private GameObject sysUI;
    private GameObject damageUI;
    private GameObject endgameTitle;

    // Music and Sound Effects
    private SoundEffects sfx_init;
    private Music music_init;

    private void Awake()
    {
        sfx_init = (SoundEffects)FindObjectOfType(typeof(SoundEffects));
        music_init = (Music)FindObjectOfType(typeof(Music));

        Instance = this;
        mainCamera = Camera.main;
        mapObject = transform.Find("mapObject").gameObject;
        mLevel = MainMenu.level;

        moving = false;
        randomHit = new System.Random();
    }

    private void Start()
    {
        // 1 = giza, 2 = lutia #1, 3 = lutia #2
        loadLevel(mLevel);
        cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
        cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
        cursorTop.transform.position += cursorOffset;
        cursorSprite = cursor.GetComponent<SpriteRenderer>();

        charList = charList.OrderByDescending(x => x.speedStat).ToList();
        setCursor(charList[0].tileLoc);
        selectChar(currTile);
        mainCamera.transform.position = new Vector3(currTile.transform.position.x, currTile.transform.position.y, -10);

        actionMenu = (GameObject)Instantiate(PrefabHolder.Instance.battleUI);
        rightInfo = (GameObject)Instantiate(PrefabHolder.Instance.rightUI);
        leftInfo = (GameObject)Instantiate(PrefabHolder.Instance.leftUI);
        sysUI = (GameObject)Instantiate(PrefabHolder.Instance.sysUI);
        damageUI = (GameObject)Instantiate(PrefabHolder.Instance.damageUI);
        actionMenu.SetActive(false);
        leftInfo.SetActive(false);
        rightInfo.SetActive(false);
        sysUI.SetActive(false);
        actionMenu.GetComponent<BattleUI>().setNumOptions(4);
        sysUI.GetComponent<BattleUI>().setNumOptions(3);

        endgameTitle = GameObject.Find("endgameTitleCard");
        endgameTitle.transform.GetChild(0).gameObject.SetActive(false);
        endgameTitle.transform.GetChild(1).gameObject.SetActive(false);
        endgameTitle.transform.GetChild(2).gameObject.SetActive(false);
    }

    void Update()
    {
        Controls();
    }

    // Load map from XML; initiate cursor, enemies, and characters; set camera position
    // Character selected by default is Marche
    private void loadLevel(int level)
    {
        if (level == 1)
        {
            XMLManager.LoadMap(1, map, mapObject);
            currTile = map[22];

            setChars(6, 19, 0, 2);         // fairy,       (1, 9), facing left
            player.setStats(25, 47, 98, 82, 116, 130, 123, 4, 3, 60);
            setChars(7, 47, 0, 2);         // blue goblin, (4, 7)
            player.setStats(34, 12, 103, 103, 71, 67, 108, 3, 2, 50);
            setChars(7, 77, 3, 2);         // blue goblin, (7, 5), facing down
            player.setStats(33, 12, 106, 105, 71, 67, 107, 3, 2, 50);
            setChars(7, 108, 3, 2);        // blue goblin, (10, 10)
            player.setStats(33, 12, 105, 108, 71, 67, 108, 3, 2, 50);
            setChars(8, 80, 3, 2);         // red goblin,  (7, 8)
            player.setStats(51, 35, 118, 118, 89, 91, 114, 3, 2, 55);

            setChars(1, 12, 2, 1);         // montblanc,   (1, 2), facing right
            setChars(2, 21, 2, 1);         // soldier,     (2, 1)
            setChars(3, 11, 2, 1);         // monk,        (1, 1)
            setChars(4, 1, 2, 1);          // white mage,  (0, 1)
            setChars(5, 10, 2, 1);         // archer,      (1, 0)
            setChars(0, 22, 2, 1);         // marche,      (2, 2)

            music_init.playL1Music();
        }
        else if (level == 2)
        {
            XMLManager.LoadMap(2, map, mapObject);
            currTile = map[121];

            setChars(7, 125, 2, 2);        // blue goblin, (9, 2), facing right
            player.setStats(52, 16, 131, 133, 88, 86, 109, 3, 2, 50);
            setChars(9, 159, 2, 2);        // red panther, (11, 6)
            player.setStats(54, 31, 132, 131, 101, 101, 117, 4, 3, 50);
            setChars(9, 113, 2, 2);        // red panther, (8, 5)
            player.setStats(54, 31, 123, 128, 103, 100, 110, 4, 3, 50);

            setChars(1, 122, 0, 1);        // montblanc,   (8, 14), facing left
            setChars(2, 151, 0, 1);        // soldier,     (10, 13)
            setChars(3, 136, 0, 1);        // monk,        (9, 13)
            setChars(4, 152, 0, 1);        // white mage,  (10, 14)
            setChars(5, 137, 0, 1);        // archer,      (9, 14)
            setChars(0, 121, 0, 1);        // marche,      (8, 13)

            music_init.playL2Music();
        }
        else
        {
            XMLManager.LoadMap(3, map, mapObject);
            currTile = map[58];

            setChars(10, 39, 2, 2);         // soldier,     (5, 3), facing right
            player.setStats(52, 15, 125, 131, 69, 90, 112, 4, 2, 54);
            setChars(11, 78, 2, 2);         // thief,       (8, 5)
            player.setStats(46, 17, 119, 116, 98, 82, 119, 4, 3, 66);
            setChars(12, 61, 2, 2);         // archer,      (7, 1)
            player.setStats(44, 19, 100, 100, 77, 97, 110, 4, 2, 50);
            setChars(13, 105, 2, 2);        // white mage,  (10, 6)
            player.setStats(41, 66, 90, 105, 110, 139, 111, 3, 2, 40);
            setChars(11, 31, 2, 2);         // thief,       (4, 5)
            player.setStats(56, 17, 138, 140, 71, 88, 108, 4, 2, 50);
            setChars(10, 120, 2, 2);        // soldier,     (11, 8)
            player.setStats(54, 18, 136, 127, 102, 98, 120, 4, 3, 66);

            setChars(1, 59, 0, 1);         // montblanc,   (6, 12), facing left
            setChars(2, 71, 0, 1);         // soldier,     (7, 11)
            setChars(3, 84, 0, 1);         // monk,        (8, 11)
            setChars(4, 85, 0, 1);         // white mage,  (8, 12)
            setChars(5, 72, 0, 1);         // archer,      (7, 12)
            setChars(0, 58, 0, 1);         // marche,      (6, 11)

            music_init.playL3Music();
        }
    }

    // create character on map
    // Character created is PrefabHolder->Player[playindex]
    // At map[mapIndex]; sprite facing in "face" direction
    private void setChars(int playIndex, int mapIndex, int face, int group)
    {
        playerTile = map[mapIndex];
        player = ((GameObject)Instantiate(PrefabHolder.Instance.Characters[playIndex])).GetComponent<Character>();
        player.transform.position = playerTile.transform.position + player.charOffset;
        player.faceDir(face);
        player.group = group;
        player.tileLoc = playerTile;
        playerTile.occupied = player.group;
        player.charSprite.sortingOrder = playerTile.sort + 3;
        player.id = playIndex;

        if (player.group == 1) {
            if (player.id == 0)
            {
                player.charName = "Marche";
                player.setStats(59, 16, 138, 140, 75, 94, 106, 4, 2, 54);
            }
            else if (player.id == 1)
            {
                player.charName = "Montblanc";
                player.setStats(43, 44, 93, 120, 117, 153, 104, 3, 2, 35);
            }
            else if (player.id == 2)
            {
                player.charName = "Victor";
                player.setStats(60, 17, 137, 137, 73, 94, 109, 4, 2, 54);
            }
            else if (player.id == 3)
            {
                player.charName = "Syrus";
                player.setStats(47, 14, 126, 112, 102, 88, 118, 4, 3, 61);
            }
            else if (player.id == 4)
            {
                player.charName = "Elnan";
                player.setStats(36, 60, 81, 98, 101, 124, 112, 3, 2, 40);
            }
            else if (player.id == 5)
            {
                player.charName = "Candice";
                player.setStats(45, 16, 115, 96, 83, 88, 116, 4, 2, 50);
            }
        }
        else if (player.group == 2)
        {
            if (player.id == 6)
                player.charName = "Fairy";
            else if (player.id == 7 || player.id == 8)
                player.charName = "Goblin";
            else if (player.id == 9)
                player.charName = "Panther";
            else if (player.id == 10)
                player.charName = "Soldier";
            else if (player.id == 11)
                player.charName = "Thief";
            else if (player.id == 12)
                player.charName = "Archer";
            else if (player.id == 13)
                player.charName = "White Mage";
        }

        charList.Add(player);
    }

    // Keyboard Controls
    public void Controls()
    {
        if (!endgameTitle.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            if (player.group == 2 && !enemyRunning)
            {
                enemyRunning = true;
                enemyAI(player);
            }
            if (player.group == 1)
            {
                // no UI elements
                if (!actionMenu.activeInHierarchy && !rightInfo.activeInHierarchy && !sysUI.activeInHierarchy)
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
                    

                    // move player to selected tile
                    if (Input.GetKeyDown(KeyCode.Z) && currTile.glow == 1 && moving)
                    {
                        glowTiles(map, 0);
                        if (player.Move(map, playerTile, currTile))
                        {
                            playerTile = currTile;
                            moving = false;
                        }
                        actionMenu.GetComponent<BattleUI>().selectOption();
                        return;
                    }

                    // choose character to attack
                    if (Input.GetKeyDown(KeyCode.Z) && currTile.glow == 3 && currTile.occupied != 0 && moving)
                    {
                        // check if target enemy already KO'd
                        enemy = currTile.getChar(charList);
                        if (!enemy.ko)
                        {
                            glowTiles(map, 0);
                            leftInfo.SetActive(true);
                            damage = player.calcDamage(enemy);
                            leftInfo.GetComponent<InfoUI>().updateLeftUI(player, enemy, false);

                            enemy.highlight(true);
                            rightInfo.SetActive(true);
                            rightInfo.GetComponent<InfoUI>().updateRightUI(enemy);
                            moving = false;
                            return;
                        }
                    }
                }

                // system menu
                if (sysUI.activeInHierarchy)
                {
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        sysUI.GetComponent<BattleUI>().selectNext();
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        sysUI.GetComponent<BattleUI>().selectPrev();

                    if (Input.GetKeyDown(KeyCode.Z) && sysUI.GetComponent<BattleUI>().actMenuSelected == 0)
                    {
                        SceneManager.LoadScene("menu");
                    }

                    if (Input.GetKeyDown(KeyCode.Z) && sysUI.GetComponent<BattleUI>().actMenuSelected == 1)
                    {
                        Application.Quit();
                        Debug.Log("Quit!");
                    }

                    if (Input.GetKeyDown(KeyCode.Z) && sysUI.GetComponent<BattleUI>().actMenuSelected == 2)
                    {
                        toggleMenu(sysUI);
                        return;
                    }
                }

                // choose from move, action, wait, status
                if (actionMenu.activeInHierarchy)
                {
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        actionMenu.GetComponent<BattleUI>().selectNext();
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        actionMenu.GetComponent<BattleUI>().selectPrev();

                    // choose move
                    if (Input.GetKeyDown(KeyCode.Z) && actionMenu.GetComponent<BattleUI>().actMenuSelected == 0)
                    {
 //                       sfx_init.playBattleMenuSelectionSound();
                        setMenu(false);
                        glowTiles(player.AstarGlow(map), 1);
                        moving = true;
                        return;
                    }

                    // choose action
                    if (Input.GetKeyDown(KeyCode.Z) && actionMenu.GetComponent<BattleUI>().actMenuSelected == 1)
                    {
//                        sfx_init.playBattleMenuSelectionSound();
                        setMenu(false);
                        glowTiles(player.allAttack(map), 3);
                        moving = true;
                        return;
                    }

                    // chose wait
                    if (Input.GetKeyDown(KeyCode.Z) && actionMenu.GetComponent<BattleUI>().actMenuSelected == 2)
                    {
//                        sfx_init.playBattleMenuSelectionSound();
                        setMenu(false);
                        actionMenu.GetComponent<BattleUI>().resetOptions();
                        selectNextChar(charList);
                        leftInfo.GetComponent<InfoUI>().updateLeftUI(player, enemy, true);
                        return;
                    }
                }

                // show player and enemy info on attack
                if (rightInfo.activeInHierarchy)
                {
                    // attack selected enemy
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        damage = player.Attack(currTile, charList, endgameTitle, randomHit);
                        damageUI.GetComponent<InfoUI>().showDamage(enemy, damage);
                        actionMenu.GetComponent<BattleUI>().selectOption();
                        enemy.highlight(false);
                        rightInfo.SetActive(false);
                        leftInfo.SetActive(false);
                        setCursor(playerTile);
                        return;
                    }

                    // cancel out of attack
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        glowTiles(player.allAttack(map), 3);
                        moving = true;
                        enemy.highlight(false);
                        leftInfo.SetActive(false);
                        rightInfo.SetActive(false);
                        return;
                    }
                }

                // show action menu (move, action, wait, status)
                if (Input.GetKeyDown(KeyCode.Z) && player.tileLoc == currTile && !moving)
                {
                    setMenu(true);
                }

                // cancel out of action menu
                if (Input.GetKeyDown(KeyCode.X))
                {
                    setMenu(false);
                    moving = false;
                    glowTiles(map, 0);
                }
            }

            // press "s" to select the character at the current tile (where cursor is)
            if (Input.GetKeyDown(KeyCode.S))
                selectChar(currTile);

            // press "esc" to toggle system menu
            if (Input.GetKeyDown(KeyCode.Escape))
                toggleMenu(sysUI);
        }
        else
        {
            StartCoroutine(finishLevel());
        }
    }

    private IEnumerator finishLevel()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("menu");
    }

    // select the character at the current tile
    private void selectChar(Tile currTile)
    {
        foreach (Character chara in charList)
        {
            if (chara.tileLoc == currTile)
            {
                player = chara;
                playerTile = chara.tileLoc;
            }
        }
    }

    // select next character in list
    private void selectNextChar(List<Character> charList)
    {
        do
        {
            for (int i = 0; i < charList.Count; i++)
            {
                if (charList[i] == player)
                {
                    if (i == charList.Count - 1)
                        player = charList[0];
                    else
                        player = charList[i + 1];

                    playerTile = player.tileLoc;
                    setCursor(playerTile);
                    sfx_init.playNewPlayerTurnSound();
                    mainCamera.transform.position = new Vector3(playerTile.transform.position.x, playerTile.transform.position.y, -10);
                    break;
                }
            }
        } while (player.ko);
    }

    // move cursor helper function; set cursor to another tile
    private void setCursor(Tile tile)
    {
        cursor.transform.position = tile.transform.position;
        cursorTop.transform.position = cursor.transform.position + cursorOffset;
        cursorSprite.sortingOrder = tile.sort + 2;

        currTile = tile;
        moveCamera(tile.transform.position);
    }

    // move the cursor one space in the given direction
    // 0 = left, 1 = up, 2 = right, 3 = down
    private void moveCursor(int direction)
    {
        sfx_init.playCursorSound();
        prevTile = currTile;
        currTile = currTile.neighbors[direction];
        if (currTile != null)
        {
            setCursor(currTile);
            enemy = null;
            enemy = currTile.getChar(charList);
            if (enemy != null)
            {
                leftInfo.SetActive(true);
                leftInfo.GetComponent<InfoUI>().updateLeftUI(enemy, enemy, true);
            }
            else
            {
                leftInfo.SetActive(false);
            }
        }
        else
            currTile = prevTile;
    }

    // highlight the List of Tiles a certain color
    // 0 = no color, 1 = blue, 2 = red, 3 = green
    private void glowTiles(List<Tile> tiles, int color)
    {
        foreach (Tile tile in tiles)
            tile.tileHighlight(color);
    }

    // toggles system menu
    private void toggleMenu(GameObject UI)
    {
        UI.SetActive(!UI.activeInHierarchy);
    }

    // turns on/off action menu
    private void setMenu(bool status)
    {
        actionMenu.SetActive(status);

        if (status && !first)
            actionMenu.GetComponent<BattleUI>().resetMenu();

        first = false;
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

    private void enemyAI(Character actor)
    {
        StartCoroutine(enemyMove(actor));
    }

    private IEnumerator enemyMove(Character actor)
    {
        List<Tile> moveTiles = actor.AstarGlow(map);            // move tiles
        Tile moveTile = null;

        List<Character> targets = new List<Character>();        // targets to attack
        Character target = null;
        int realAtkRange = actor.atkRange;
        actor.atkRange += actor.moveStat;

        List<Tile> attackTilesRaw = actor.allAttack(map);       // all tiles in attack range
        List<Tile> attackTiles = new List<Tile>();              // attack tiles that are accessible
        bool addTile = false;
        actor.atkRange = realAtkRange;

        // check that attack range tiles are accessible (not surrounded by players, blocked by tree, etc.)
        foreach (Tile tile in attackTilesRaw)
        {
            addTile = false;
            foreach (Tile nbor in tile.neighbors)
            {
                if (nbor)
                    if (nbor.occupied == 0 && attackTilesRaw.Contains(nbor) &&
                        (tile.height - nbor.height) <= actor.atkHeightLow && (tile.height - nbor.height) >= actor.atkHeightHigh)
                        addTile = true;
            }

            if (addTile)
                attackTiles.Add(tile);
        }

        // find targets in attack range
        foreach (Tile tile in attackTiles)
        {
            if (tile.occupied == 1 && !tile.getChar(charList).ko)
                targets.Add(tile.getChar(charList));
        }

        // if target exists, find weakest target and tile to move to
        if (targets.Count > 0)
        {
            targets = targets.OrderBy(x => x.currhpStat).ToList();

            for (int i = 0; i < targets.Count; i++)
            {
                target = targets[i];
                for (int j = 0; j < 4; j++)
                {
                    if (target.tileLoc.neighbors[j])
                        if ((target.tileLoc.neighbors[j].occupied == 0 && moveTiles.Contains(target.tileLoc.neighbors[j])) || 
                            target.tileLoc.neighbors[j] == actor.tileLoc)
                        {
                            moveTile = target.tileLoc.neighbors[j];
                            if (j != target.currFace)
                                break;
                        }
                }

                // if found target, then done; if all targets inaccessible, then no target
                if (moveTile)
                    break;
                else
                {
                    if (i == targets.Count - 1 && moveTile == null)
                        target = null;
                }
            }
        }
        // if no target exists, move towards closest character
        else
        {
            int tileDiff = 0;
            int minTileDiff = 100;

            // find closest character
            foreach (Character cha in charList)
            {
                if (cha.group == 1 && !cha.ko)
                {
                    tileDiff = Mathf.Abs(cha.tileLoc.row - actor.tileLoc.row) + Mathf.Abs(cha.tileLoc.col - cha.tileLoc.col);
                    if (minTileDiff > tileDiff)
                    {
                        minTileDiff = tileDiff;
                        target = cha;
                    }
                }
            }

            // find closest moveable tile to character
            minTileDiff = 100;
            foreach (Tile tile in moveTiles)
            {
                tileDiff = Mathf.Abs(target.tileLoc.row - tile.row) + Mathf.Abs(target.tileLoc.col - tile.col);
                if (minTileDiff > tileDiff)
                {
                    minTileDiff = tileDiff;
                    moveTile = tile;
                }
            }

            // no attack target
            target = null;
        }
        yield return new WaitForSeconds(1.1f);

        // highlight move tiles and move
        foreach (Tile tile in moveTiles)
        {
            tile.tileHighlight(1);
        }
        if (moveTile)
            setCursor(moveTile);

        yield return new WaitForSeconds(1.1f);
        foreach (Tile tile in moveTiles)
        {
            tile.tileHighlight(0);
        }

        if (moveTile)
        {
            actor.Move(map, actor.tileLoc, moveTile);
            yield return new WaitForSeconds(2);
        }

        // if target exists, highlight attack tiles and attack
        if (target)
        {
            foreach (Tile tile in actor.allAttack(map))
                tile.tileHighlight(3);
            setCursor(target.tileLoc);
            yield return new WaitForSeconds(1.1f);
            foreach (Tile tile in actor.allAttack(map))
                tile.tileHighlight(0);

            target.highlight(true);
            leftInfo.SetActive(true);
            leftInfo.GetComponent<InfoUI>().updateLeftUI(actor, target, false);
            rightInfo.SetActive(true);
            rightInfo.GetComponent<InfoUI>().updateRightUI(target);
            yield return new WaitForSeconds(1.1f);

            target.highlight(false);
            leftInfo.SetActive(false);
            rightInfo.SetActive(false);
            int damage = actor.Attack(target.tileLoc, charList, endgameTitle, randomHit);
            damageUI.GetComponent<InfoUI>().showDamage(target, damage);
            yield return new WaitForSeconds(2);
        }

        // end turn
        enemyRunning = false;
        selectNextChar(charList);
        yield return null;
    }
}
