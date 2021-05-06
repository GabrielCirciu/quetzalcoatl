using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour {
    Animator charAnimator;

    void Start() {
        charAnimator = GetComponent<Animator>();
    }

    void Update() {
        if ( Input.GetKeyDown(KeyCode.Mouse0) ) {
            //Debug.Log("Left click, make something happen here!");
        }
    }
}
