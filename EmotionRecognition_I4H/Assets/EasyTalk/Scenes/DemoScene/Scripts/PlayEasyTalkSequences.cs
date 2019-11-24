using UnityEngine;
using System.Collections;

public class PlayEasyTalkSequences : MonoBehaviour
{
    private EasyTalk _EasyTalk;

    bool didWePlayTalkingSequence = false;

    void Start ()
    {
        _EasyTalk = (EasyTalk)GetComponent<EasyTalk>();
    }
	
	void Update ()
    {
        if (_EasyTalk.IsSequencePlaying() == false)
        {
            if (didWePlayTalkingSequence == false)
            {
                _EasyTalk.PlaySequence("EasyTalk01_English");
                didWePlayTalkingSequence = true;
            }
            else
            {
                _EasyTalk.PlaySequence("EasyTalkIdle_English", true);
            }
        }
    }
}
