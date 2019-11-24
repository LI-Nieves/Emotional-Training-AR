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
            case 1:
                anm.SetBool("isWalking",false);
                anm.SetBool("isTripping",false);
                if (!(tr.rotation.eulerAngles.y > 85f) && !(tr.rotation.eulerAngles.y < 95f)) {
                    tr.rotation = Quaternion.Euler(new Vector3(0f,90f,0f));
                }
                break;
                /*
            case 1: // Trip and fall
                Movement2();
                break;
            case 2: // Get up
                Movement3();
                break;
            case 3: // Turn left and camera zoom
                break;
                */
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

    /*
    void Movement2() {
        anm.SetBool("isTripping",false);
        anm.SetBool("isWalking",false);
        IncrementStep();
    }

    void Movement3() {
        anm.SetBool("isTripping",false);
        TurnCharacter();
    }
    
    void Movement4() {
        anm.SetBool("isIdle",true);
    }

    void TurnCharacter() {
        Debug.Log(gameObject.GetComponent<Transform>().rotation.eulerAngles.y.ToString());
        if ((GameObject.Find("mixamorig9:Hips").GetComponent<Transform>().rotation.eulerAngles.y > 85f) &&
            (GameObject.Find("mixamorig9:Hips").GetComponent<Transform>().rotation.eulerAngles.y < 95f)) {
            tr.Rotate(new Vector3(0f,90f,0f));
        } else {
            //anm.SetBool("isTurning",false);
            IncrementStep();
        }
    }
    */

    public void IncrementStep() {
        currentStep++;
    }
}
