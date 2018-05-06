using UnityEngine;
using System.Collections;
public abstract class BattleState : State
{
    protected BattleController owner;
    public Board board { get { return owner.board; } }

    public string str;
    private Tile prevTile = null;
    private Tile currTile = null;
    public GameObject cursor;
    private GameObject cursorTop;
    private SpriteRenderer cursorSprite;
    private Camera mainCamera;

    protected virtual void Awake()
    {
        owner = GetComponent<BattleController>();

        currTile = board.map[0];
        cursor = (GameObject)Instantiate(PrefabHolder.Instance.CursorBase, currTile.transform.position, Quaternion.identity);
        cursorTop = (GameObject)Instantiate(PrefabHolder.Instance.CursorTop, currTile.transform.position, Quaternion.identity);
        cursorTop.transform.position += new Vector3(0, 2f, 0);
        cursorSprite = cursor.GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    protected override void AddListeners()
    {
        InputController.moveEvent += OnMove;
        InputController.fireEvent += OnFire;
    }

    protected override void RemoveListeners()
    {
        InputController.moveEvent -= OnMove;
        InputController.fireEvent -= OnFire;
    }

    protected virtual void OnMove(object sender, InfoEventArgs<int> e)
    {

    }

    protected virtual void OnFire(object sender, InfoEventArgs<int> e)
    {

    }

    protected virtual void moveCursor(int direction)
    {
        prevTile = currTile;
        currTile = currTile.neighbors[direction];
        if (currTile != null)
        {
            cursor.transform.position = currTile.transform.position;
            cursorTop.transform.position = cursor.transform.position + new Vector3(0, 2f, 0);
            moveCamera(cursor.transform.position);
            if (currTile.hasObj)
                cursorSprite.sortingOrder = 3;
            else
                cursorSprite.sortingOrder = 1;
        }
        else
            currTile = prevTile;
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