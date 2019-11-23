﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    private Animator anm;
    private Transform tr;

    // Start is called before the first frame update
    void Start()
    {
        anm = gameObject.GetComponent<Animator>();
        tr = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCharacter();
    }

    float CheckXPosition() {
        return tr.position.x;
    }

    float CheckYPosition() {
        return tr.position.y;
    }

    float CheckZPosition() {
        return tr.position.z;
    }

    bool ReachedCentre() {
        return CheckZPosition() <= 0.15f && CheckZPosition() >= -0.15f;
    }

    void MoveCharacter() {
        if (!ReachedCentre()) {
            anm.SetBool("isWalking",true);
        } else if (ReachedCentre()) {
            anm.SetBool("isWalking",false);
            TurnCharacter();
        }
    }

    void TurnCharacter() {
        //Debug.Log(gameObject.GetComponent<Transform>().rotation.ToString());
        if (GameObject.Find("mixamorig9:Hips").GetComponent<Transform>().rotation != new Quaternion(0f,1f,0f,0f)) {
            anm.SetBool("isTurning",true);
        } else {
            anm.SetBool("isTurning",false);
        }
    }
}
