using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    private Animator anm;
    private Transform tr;
    private int currentStep;

    public bool hasFallen;

    public float fallOffset;

    // Start is called before the first frame update
    void Start()
    {
        anm = gameObject.GetComponent<Animator>();
        tr = gameObject.GetComponent<Transform>();
        currentStep = 0;
        hasFallen = false;
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

    bool ReachedCentre(float offset) {
        return CheckZPosition() <= (0.1f + offset) && CheckZPosition() >= (-0.1f + offset);
    }

    void MoveCharacter() {
        switch (currentStep) {
            case 0: // Walk forward
                Movement1();
                break;
            case 1: // Trip and fall
                Movement2();
                break;
            case 2: // Get up
                Movement3();
                break;
            case 3: // Turn left and camera zoom
                break;
        }
    }

    void Movement1() {
        if (!ReachedCentre(fallOffset)) {
            anm.SetBool("isWalking",true);
        } else {
            anm.SetBool("isTripping",true);
            IncrementStep();
        }
    }

    void Movement2() {
        anm.SetBool("isTripping",false);
        anm.SetBool("isWalking",false);
        IncrementStep();
    }

    void Movement3() {
        anm.SetBool("isTripping",false);
        TurnCharacter();
    }

    void TurnCharacter() {
        //Debug.Log(gameObject.GetComponent<Transform>().rotation.ToString());
        if (GameObject.Find("mixamorig9:Hips").GetComponent<Transform>().rotation != new Quaternion(0f,1f,0f,0f)) {
            anm.SetBool("isTurning",true);
        } else {
            anm.SetBool("isTurning",false);
        }
    }

    public void SetFallenStatus() {
        hasFallen = true;
    }

    public void IncrementStep() {
        currentStep++;
    }
}
