using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
        //base.setStat("group", 2);
        group = 2;
    }
	

}
