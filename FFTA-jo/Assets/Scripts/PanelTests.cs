using UnityEngine;
using System.Collections;
public class PanelTests : MonoBehaviour
{
    Panel panel;
    const string Show = "Show";
    const string Hide = "Hide";
    const string Center = "Center";
    void Start()
    {
        panel = GetComponent<Panel>();
        Panel.Position centerPos = new Panel.Position(Center, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter);
        panel.AddPosition(centerPos);
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), Show))
            panel.SetPosition(Show);
        if (GUI.Button(new Rect(10, 50, 100, 30), Hide))
            panel.SetPosition(Hide);
        if (GUI.Button(new Rect(10, 90, 100, 30), Center))
            panel.SetPosition(Center);
    }
}
