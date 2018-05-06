using UnityEngine;
using System;
using System.Collections;

public class InputController : MonoBehaviour
{
    public static event EventHandler<InfoEventArgs<int>> moveEvent;
    public static event EventHandler<InfoEventArgs<int>> fireEvent;

    string[] _buttons = new string[] { "Fire1", "Fire2", "Fire3" };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (moveEvent != null)
                moveEvent(this, new InfoEventArgs<int>(0));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (moveEvent != null)
                moveEvent(this, new InfoEventArgs<int>(1));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (moveEvent != null)
                moveEvent(this, new InfoEventArgs<int>(2));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (moveEvent != null)
                moveEvent(this, new InfoEventArgs<int>(3));
        }

        for (int i = 0; i < 3; ++i)
        {
            if (Input.GetButtonUp(_buttons[i]))
            {
                if (fireEvent != null)
                    fireEvent(this, new InfoEventArgs<int>(i));
            }
        }
    }
}

