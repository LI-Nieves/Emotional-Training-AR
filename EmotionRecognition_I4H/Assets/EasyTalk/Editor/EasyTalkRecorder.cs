/*
 * This source code is (c)Copyright 2017 Realtime VR, LLC. All rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(EasyTalk))]
public class EasyTalkRecorder : Editor
{
    public bool _Initialized = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        string[] controllerNamesList = new string[]
                        {
                            "None",
                            "LSE-MouseX-", 
                            "LSE-MouseX+",
                            "LSE-MouseY-", 
                            "LSE-MouseY+",
                            "LSE-ControllerAxis1-", 
                            "LSE-ControllerAxis1+", 
                            "LSE-ControllerAxis2-", 
                            "LSE-ControllerAxis2+", 
                            "LSE-ControllerAxis3-", 
                            "LSE-ControllerAxis3+", 
                            "LSE-ControllerAxis4-", 
                            "LSE-ControllerAxis4+", 
                            "LSE-ControllerAxis5-", 
                            "LSE-ControllerAxis5+", 
                            "LSE-ControllerAxis6-", 
                            "LSE-ControllerAxis6+"
                        };

        EasyTalk easyTalk = (EasyTalk)this.target;

        

        if (easyTalk.Sequences.Count == 0)
            return;        

        if (_Initialized == false)
        {
            _Initialized = true;

            easyTalk.GetBlendshapeDictionary();

            // Save the asset sequence path
            foreach (EasyTalk.SequenceData sequenceData in easyTalk.Sequences)
            {
                if (sequenceData.SequenceFile == null)
                    continue;
                string pathToSequenceTextAsset = AssetDatabase.GetAssetPath(sequenceData.SequenceFile);
                sequenceData.BasePath = pathToSequenceTextAsset.Substring(0, pathToSequenceTextAsset.Length - 4);

                // The goal here is twofold:
                // 1. Ensure that the sequence.expression_name files exist
                // 2. Ensure that we have these loaded as TextAsset's in the easyTalk.SequenceData because remember, 
                //    easyTalk.Sequences is a public member of the GameObject and exists for the life of that
                //    GameObject. So we need to ensure the expressions sequence TextAsset exists and is 
                //    also assigned to our public variable.
                // TODO: Break here: expressionRecordingsTextAssetsMisconfigured is always false. 
                //       animator.parameters.Length include visemes and sequenceData.ExpressionRecordings does not.
                bool expressionRecordingsTextAssetsMisconfigured = false;
                if (sequenceData.ExpressionRecordings.Count != easyTalk.BlendshapeDictionary.Count)
                {
                    expressionRecordingsTextAssetsMisconfigured = true;
                }
                foreach (TextAsset expressionRecording in sequenceData.ExpressionRecordings)
                {
                    if (expressionRecording == null)
                    {
                        expressionRecordingsTextAssetsMisconfigured = true;
                    }
                }
                if (expressionRecordingsTextAssetsMisconfigured)
                {
                    // Reassign everything.
                    sequenceData.ExpressionRecordings.Clear();
                    foreach (KeyValuePair<string, List<EasyTalk.BlendshapeElement>> kvp in easyTalk.BlendshapeDictionary)
                    {
                        string keyName = kvp.Key; // Ex. Expression_Smile
                        if (keyName.StartsWith("Expression_") == false)
                            continue;
                        string textAssetFilepath = sequenceData.BasePath + "." + keyName + ".txt";

                        // 1. Ensure that the sequence.expression_name files exist
                        if (File.Exists(textAssetFilepath) == false)
                        {
                            File.WriteAllText(textAssetFilepath, "");
                            // Allow it to actually exist before continuing...
                            while (File.Exists(textAssetFilepath) == false) ;
                        }

                        // 2. Ensure that we have these loaded as TextAsset's in the easyTalk.SequenceData
                        TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(textAssetFilepath, typeof(TextAsset));
                        if (textAsset == null)
                        {
                            AssetDatabase.ImportAsset(textAssetFilepath, ImportAssetOptions.ForceSynchronousImport);
                        }
                        textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(textAssetFilepath, typeof(TextAsset));
                        if (sequenceData.ExpressionRecordings.Contains(textAsset) == false)
                            sequenceData.ExpressionRecordings.Add(textAsset);
                    }
                }
            }

            // Only set input axis if Game playing (running)
            if (Application.isPlaying)
            {
                // controllerNum = 0;  is for input from any/all controllers
                int controllerNum = 0;
                for (int axisNum = 0; axisNum < 6; axisNum++)
                {
                    EasyTalkInputController.AddAxis(new EasyTalkInputController.InputAxis()
                    {
                        name = "LSE-ControllerAxis" + (axisNum + 1).ToString() + "+",
                        dead = 0.25f,
                        sensitivity = 1f,
                        type = EasyTalkInputController.AxisType.JoystickAxis,
                        axis = (axisNum + 1),
                        joyNum = controllerNum
                    });
                    EasyTalkInputController.AddAxis(new EasyTalkInputController.InputAxis()
                    {
                        name = "LSE-ControllerAxis" + (axisNum + 1).ToString() + "-",
                        dead = 0.25f,
                        sensitivity = 1f,
                        type = EasyTalkInputController.AxisType.JoystickAxis,
                        axis = (axisNum + 1),
                        joyNum = controllerNum
                    });
                }
                //_AssetDatabaseRefreshed = false;
            }
            else
            {
                //  AssetDatabase.Refresh(ImportAssetOptions.Default); slows down GUI performance.
                //if (_AssetDatabaseRefreshed == false)
                {
                    AssetDatabase.Refresh(ImportAssetOptions.Default);
                    //_AssetDatabaseRefreshed = true;
                }
            }

        }


        EditorGUILayout.LabelField("________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
        
        EditorGUILayout.LabelField(" ");
        easyTalk._Active = EditorGUILayout.ToggleLeft("Enable Expression Recording and Playback:", easyTalk._Active); //, GUILayout.Width(90));\
        EditorGUI.BeginDisabledGroup(!easyTalk._Active);
        EditorGUI.indentLevel += 1;
        EditorGUILayout.HelpBox(
            //"HOW TO RECORD EXPRESSIONS:\n\n1. Disable any input controllers and use a main camera to focus in on your character.\n\n2. Set up the sequence and recording options below.\n\n3. Ensure that this GameObject instance is selected in the Hierarchy.\n\n4. Hide or close any Animator windows in the Unity editor! If you do not do this, you may notice an extreme performance drop in the Game window!\n\n5. Click on the Play button.\n\n6. Click somewhere within the Unity Game window to enter Standby mode.\n\n7. Use the Record button you set below to start and stop recording. To modify a recorded expression, check the corresponding Enable button and restart your recording. All other recorded expressions will persist if the Enable button is not checked.", 
            "HOW TO RECORD EXPRESSIONS\n\n1. Click the [Play] button.\n2. Select the character in the Hierarchy.\n3. Click in the Game window.\n4. Press the Recording button you selected below.",
            MessageType.Info, 
            true);
        EditorGUILayout.LabelField(" ");

        EditorGUILayout.LabelField("Select a Sequence File to Record to:");
        EditorGUI.indentLevel += 1;
        List<string> sequenceFileList = new List<string>();
        foreach (EasyTalk.SequenceData sequenceData in easyTalk.Sequences)
        {
            if (sequenceData.SequenceFile != null)
                sequenceFileList.Add(sequenceData.SequenceFile.name);
        }
        easyTalk.SelectedSequenceIndex = EditorGUILayout.Popup(easyTalk.SelectedSequenceIndex, sequenceFileList.ToArray());
        EditorGUI.indentLevel -= 1;

        EditorGUILayout.LabelField("Select a Key or Button for Playback:");
        EditorGUI.indentLevel += 1;
        easyTalk.PlayButton = (EasyTalk.ButtonNames)EditorGUILayout.EnumPopup(easyTalk.PlayButton); //EditorGUILayout.EnumPopup(ButtonNames, easyTalk.RecordStartButtonIndex, buttonNamesList);
        EditorGUI.indentLevel += 1;       
        easyTalk._DisplayAllRecordedExpressionsOnPlayback = EditorGUILayout.ToggleLeft("Display all recorded expressions during playback. (Recommended)", easyTalk._DisplayAllRecordedExpressionsOnPlayback); //, GUILayout.Width(90));\
        EditorGUI.indentLevel -= 1;
        EditorGUI.indentLevel -= 1;

        EditorGUILayout.LabelField("Select a Key or Button to Start Recording:");
        EditorGUI.indentLevel += 1;
        easyTalk.RecordButton = (EasyTalk.ButtonNames)EditorGUILayout.EnumPopup(easyTalk.RecordButton); //EditorGUILayout.EnumPopup(ButtonNames, easyTalk.RecordStartButtonIndex, buttonNamesList);
        EditorGUI.indentLevel -= 1;

        EditorGUILayout.LabelField("Select Control Inputs for Recording:");
        //if (Application.isPlaying == false)
        {
            EditorGUI.indentLevel += 1;

            foreach (KeyValuePair<string, List<EasyTalk.BlendshapeElement>> kvp in easyTalk.BlendshapeDictionary)
            {
                string keyName = kvp.Key; // Ex. Expression_Smile
                if (keyName.StartsWith("Expression_"))
                {
                    int parameterControllerListIndex = -1;
                    for (int i = 0; i < easyTalk.ExpressionControllerList.Count; i++)
                    {
                        if (easyTalk.ExpressionControllerList[i].KeyName == keyName)
                        {
                            parameterControllerListIndex = i;
                            break;
                        }
                    }

                    if (parameterControllerListIndex == -1)
                    {
                        easyTalk.ExpressionControllerList.Add(new EasyTalk.ExpressionController
                        {
                            KeyName = keyName,
                            SelectedInputControllerIndex = 0,
                            SelectedInputControllerName = controllerNamesList[0]
                        });
                        parameterControllerListIndex = easyTalk.ExpressionControllerList.Count - 1;
                    }

                    EditorGUI.BeginDisabledGroup(!easyTalk.ExpressionControllerList[parameterControllerListIndex].Record);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(keyName, GUILayout.MinWidth(100));//, GUILayout.Width(230));


                    easyTalk.ExpressionControllerList[parameterControllerListIndex].SelectedInputControllerIndex =
                        EditorGUILayout.Popup(easyTalk.ExpressionControllerList[parameterControllerListIndex].SelectedInputControllerIndex, controllerNamesList, GUILayout.Width(160));
                    easyTalk.ExpressionControllerList[parameterControllerListIndex].SelectedInputControllerName =
                        controllerNamesList[easyTalk.ExpressionControllerList[parameterControllerListIndex].SelectedInputControllerIndex];

                    EditorGUI.EndDisabledGroup();

                    easyTalk.ExpressionControllerList[parameterControllerListIndex].Record = EditorGUILayout.ToggleLeft("Enable", easyTalk.ExpressionControllerList[parameterControllerListIndex].Record, GUILayout.Width(90));

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.indentLevel -= 1;
        EditorGUI.EndDisabledGroup();
    }
}