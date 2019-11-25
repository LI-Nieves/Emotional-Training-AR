using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    public GameObject eventBox;
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
        eventBox = GameObject.Find("EVENTBOX");
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

    bool IsIdle() {
        return anm.GetCurrentAnimatorClipInfo(0)[0].clip.ToString() == "Idle (UnityEngine.AnimationClip)";
    }

    void MoveCharacter() {
        if (ReachedCentre(fallOffset)) {
            anm.SetBool("isTripping",true);
        }

        if (IsIdle()) {
            eventBox.GetComponent<UIScript>().ShowChoicesUI();
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
        if (GameObject.Find("mixamorig9:Hips").GetComponent<Transform>().rotation.eulerAngles.y >= -100f &&
            GameObject.Find("mixamorig9:Hips").GetComponent<Transform>().rotation.eulerAngles.y <= -80f) {
            anm.SetBool("isTurning",false);
        } else {
            anm.SetBool("isTurning",true);
        }
    }

    void TurnCharacter() {
    }

    public void SetFallenStatus() {
        hasFallen = true;
    }

    public void IncrementStep() {
        currentStep++;
    }
}
