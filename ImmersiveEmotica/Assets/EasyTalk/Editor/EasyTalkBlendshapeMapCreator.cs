/*
 * This source code is (c)Copyright 2017 Realtime VR, LLC. All rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * */

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class EasyTalkBlendshapeMapCreator : AssetPostprocessor
{
    public enum BlendshapeType
    {
        Expression,
        English,
        French
    }

    public class EasyTalkBlendshapeItem
    {
        public EasyTalkBlendshapeItem()
        {
            BlendshapeType = BlendshapeType.Expression;
            BlendShapeWeight = 100;
        }

        public BlendshapeType BlendshapeType { get; set; }
        public string PhonemeName { get; set; }
        public string MeshName { get; set; }
        public string BlendShapeName { get; set; }
        public int BlendShapeIndex { get; set; }
        public float BlendShapeWeight { get; set; }
    }

    public static bool ImportInitiatedByEasyTalk = false;

    public class BlendshapeInfo
    {
        public BlendshapeInfo()
        {
            MeshName = "";
            BlendshapeName = "";
            Weight = 100;
        }

        public string MeshName { get; set; }
        public string BlendshapeName { get; set; }
        public float Weight { get; set; }
    }

    public class Phoneme
    {
        public Phoneme()
        {
            RecognizedBlendShapeNames = new List<BlendshapeInfo>();
        }

        public string PhonemeName { get; set; }
        public List<BlendshapeInfo> RecognizedBlendShapeNames { get; set; }
    }

    #region PhonemeDictionaries
    public Dictionary<BlendshapeType, List<Phoneme>> PhonemeDictionaries = new Dictionary<BlendshapeType, List<Phoneme>>()
    {
        {
            BlendshapeType.English,
            new List<Phoneme>()
            {
                new Phoneme()
                {
                    PhonemeName = "SIL"
                },
                new Phoneme()
                {
                    PhonemeName = "AA",
                },
                new Phoneme()
                {
                    PhonemeName = "AE",
                },
                new Phoneme()
                {
                    PhonemeName = "AH",
                },
                new Phoneme()
                {
                    PhonemeName = "AO",
                },
                new Phoneme()
                {
                    PhonemeName = "AW",
                },
                new Phoneme()
                {
                    PhonemeName = "AY",
                },
                new Phoneme()
                {
                    PhonemeName = "B",
                },
                new Phoneme()
                {
                    PhonemeName = "CH",
                },
                new Phoneme()
                {
                    PhonemeName = "D",
                },
                new Phoneme()
                {
                    PhonemeName = "DH",
                },
                new Phoneme()
                {
                    PhonemeName = "EH",
                },
                new Phoneme()
                {
                    PhonemeName = "ER",
                },
                new Phoneme()
                {
                    PhonemeName = "EY",
                },
                new Phoneme()
                {
                    PhonemeName = "F",
                },
                new Phoneme()
                {
                    PhonemeName = "G",
                },
                new Phoneme()
                {
                    PhonemeName = "HH",
                },
                new Phoneme()
                {
                    PhonemeName = "IH",
                },
                new Phoneme()
                {
                    PhonemeName = "IY",
                },
                new Phoneme()
                {
                    PhonemeName = "JH",
                },
                new Phoneme()
                {
                    PhonemeName = "K",
                },
                new Phoneme()
                {
                    PhonemeName = "L",
                },
                new Phoneme()
                {
                    PhonemeName = "M",
                },
                new Phoneme()
                {
                    PhonemeName = "N",
                },
                new Phoneme()
                {
                    PhonemeName = "NG",
                },
                new Phoneme()
                {
                    PhonemeName = "OW",
                },
                new Phoneme()
                {
                    PhonemeName = "OY",
                },
                new Phoneme()
                {
                    PhonemeName = "P",
                },
                new Phoneme()
                {
                    PhonemeName = "R",
                },
                new Phoneme()
                {
                    PhonemeName = "S",
                },
                new Phoneme()
                {
                    PhonemeName = "SH",
                },
                new Phoneme()
                {
                    PhonemeName = "T",
                },
                new Phoneme()
                {
                    PhonemeName = "TH",
                },
                new Phoneme()
                {
                    PhonemeName = "UH",
                },
                new Phoneme()
                {
                    PhonemeName = "UW",
                },
                new Phoneme()
                {
                    PhonemeName = "V",
                },
                new Phoneme()
                {
                    PhonemeName = "W",
                },
                new Phoneme()
                {
                    PhonemeName = "Y",
                },
                new Phoneme()
                {
                    PhonemeName = "Z",
                },
                new Phoneme()
                {
                    PhonemeName = "ZH",
                }
            }
        },
        {
            BlendshapeType.French,
            new List<Phoneme>()
            {
                new Phoneme()
                {
                    PhonemeName = "SIL",
                },
                new Phoneme()
                {
                    PhonemeName = "b",
                },
                new Phoneme()
                {
                    PhonemeName = "d",
                },
                new Phoneme()
                {
                    PhonemeName = "f",
                },
                new Phoneme()
                {
                    PhonemeName = "k",
                },
                new Phoneme()
                {
                    PhonemeName = "l",
                },
                new Phoneme()
                {
                    PhonemeName = "m",
                },
                new Phoneme()
                {
                    PhonemeName = "n",
                },
                new Phoneme()
                {
                    PhonemeName = "p",
                },
                new Phoneme()
                {
                    PhonemeName = "s",
                },
                new Phoneme()
                {
                    PhonemeName = "t",
                },
                new Phoneme()
                {
                    PhonemeName = "v",
                },
                new Phoneme()
                {
                    PhonemeName = "z",
                },
                new Phoneme()
                {
                    PhonemeName = "ɡ",
                },
                new Phoneme()
                {
                    PhonemeName = "N",
                },
                new Phoneme()
                {
                    PhonemeName = "r",
                },
                new Phoneme()
                {
                    PhonemeName = "S",
                },
                new Phoneme()
                {
                    PhonemeName = "Z",
                },
                new Phoneme()
                {
                    PhonemeName = "dZ",
                },
                new Phoneme()
                {
                    PhonemeName = "tS",
                },
                new Phoneme()
                {
                    PhonemeName = "G",
                },
                new Phoneme()
                {
                    PhonemeName = "j",
                },
                new Phoneme()
                {
                    PhonemeName = "w",
                },
                new Phoneme()
                {
                    PhonemeName = "ÿ",
                },
                new Phoneme()
                {
                    PhonemeName = "a",
                },
                new Phoneme()
                {
                    PhonemeName = "é",
                },
                new Phoneme()
                {
                    PhonemeName = "i",
                },
                new Phoneme()
                {
                    PhonemeName = "o",
                },
                new Phoneme()
                {
                    PhonemeName = "u",
                },
                new Phoneme()
                {
                    PhonemeName = "y",
                },
                new Phoneme()
                {
                    PhonemeName = "ö",
                },
                new Phoneme()
                {
                    PhonemeName = "ë",
                },
                new Phoneme()
                {
                    PhonemeName = "ò",
                },
                new Phoneme()
                {
                    PhonemeName = "e",
                },
                new Phoneme()
                {
                    PhonemeName = "è",
                },
                new Phoneme()
                {
                    PhonemeName = "â",
                },
                new Phoneme()
                {
                    PhonemeName = "ô",
                },
                new Phoneme()
                {
                    PhonemeName = "ê",
                }
            }
        }
    };
    #endregion


    [MenuItem("Assets/Create EasyTalk Blendshape Map")]
    private static void CreateEasyTalkBlendshapeMap()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (assetPath.ToLower().EndsWith(".fbx"))
        {
            ImportInitiatedByEasyTalk = true;
            AssetDatabase.ImportAsset(assetPath);
        }
        ImportInitiatedByEasyTalk = false;
    }    

    // https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPostprocessModel.html
    // And the real key to this is here: https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPostprocessModel.html
    void OnPostprocessModel(GameObject gameObject)
    {
        if (ImportInitiatedByEasyTalk == false)
        {
            return;
        }
        ImportInitiatedByEasyTalk = false;

        //EasyTalkBlendshapeMapEditor window = EasyTalkBlendshapeMapEditor.CreateInstance<EasyTalkBlendshapeMapEditor>();
        //window.Valid = true;
        //window.Show();

        // Matches phonemes with recognized blendshape names for standard DAZ Studio 
        // characters and Autodesk Character Generator characters.
        ImportRecognizedBlendshapes();

        string fbxFilePathAbsolute = (Application.dataPath + assetPath.Substring(6)).Replace('\\', '/');

        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
        List<EasyTalkBlendshapeItem> easyTalkBlendshapeItems = new List<EasyTalkBlendshapeItem>();
        foreach (Transform transform in transforms)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                Mesh mesh = skinnedMeshRenderer.sharedMesh;
                string[] arr;
                arr = new string[mesh.blendShapeCount];
                for (int i = 0; i < mesh.blendShapeCount; i++)
                {
                    easyTalkBlendshapeItems.Add(new EasyTalkBlendshapeItem()
                    {
                        BlendshapeType = BlendshapeType.Expression,
                        MeshName = mesh.name,
                        BlendShapeName = mesh.GetBlendShapeName(i),
                        BlendShapeIndex = i,
                        BlendShapeWeight = 100
                    });
                }
            }
        }
        
        // Now iterate over phonemes and add to easyTalkBlendshapeItems based on recognized visemes
        List<EasyTalkBlendshapeItem> additionalEasyTalkBlendshapeItems = new List<EasyTalkBlendshapeItem>();
        foreach (KeyValuePair<BlendshapeType, List<Phoneme>> kvp in PhonemeDictionaries)
        {
            BlendshapeType blendshapeType = (BlendshapeType)(kvp.Key);
            List<Phoneme> phonemeList = (List<Phoneme>)(kvp.Value);
            foreach (Phoneme phoneme in phonemeList)
            {
                foreach (EasyTalkBlendshapeItem item in easyTalkBlendshapeItems)
                {
                    foreach (BlendshapeInfo blendshapeInfo in phoneme.RecognizedBlendShapeNames)
                    {
                        if (item.BlendShapeName.Contains(blendshapeInfo.BlendshapeName) && item.MeshName.Contains(blendshapeInfo.MeshName))
                        {
                            if (item.BlendShapeName.Contains("Shout"))
                            {
                                bool stopHere = true;
                            }

                            // Found. Assign it to additionalEasyTalkBlendshapeItems
                            EasyTalkBlendshapeItem additionalEasyTalkBlendshapeItem = new EasyTalkBlendshapeItem()
                            {
                                BlendshapeType = blendshapeType,
                                MeshName = item.MeshName,
                                BlendShapeIndex = item.BlendShapeIndex,
                                BlendShapeName = item.BlendShapeName,
                                PhonemeName = phoneme.PhonemeName,
                                BlendShapeWeight = blendshapeInfo.Weight
                            };
                            if (additionalEasyTalkBlendshapeItems.Contains(additionalEasyTalkBlendshapeItem) == false)
                            {
                                additionalEasyTalkBlendshapeItems.Add(additionalEasyTalkBlendshapeItem);
                            }
                        }
                    }
                }
            }
        }
        easyTalkBlendshapeItems.AddRange(additionalEasyTalkBlendshapeItems);
        //UnityEditor.EditorJsonUtility.ToJson( (UnityEngine.Object) easyTalkBlendshapeList);
        StringBuilder csvFile = new StringBuilder();
        foreach (EasyTalkBlendshapeItem item in easyTalkBlendshapeItems)
        {
            string line = string.Format("{0},{1},{2},{3},{4},{5}", item.BlendshapeType, item.PhonemeName, item.MeshName, item.BlendShapeIndex, item.BlendShapeName, item.BlendShapeWeight);
            csvFile.AppendLine(line);
        }
        //string blendshapeInfoFilePathAbsolute = fbxFilePathAbsolute + ".BlendshapeInfo.txt";
        string blendshapeInfoFilePathAbsolute = Path.ChangeExtension(fbxFilePathAbsolute, ".EasyTalkBlendshapeMap.csv");
        File.WriteAllText(blendshapeInfoFilePathAbsolute, csvFile.ToString());
    }

    private class MeshBlendshape : IEquatable<MeshBlendshape>
    {
        public string Mesh { get; set; }
        public string Blendshape { get; set; }
        public float Weight { get; set; }

        public bool Equals(MeshBlendshape other)
        {
            if (other == null)
                return false;

            if (this.Mesh == other.Mesh && this.Blendshape == other.Blendshape)
                return true;
            else
                return false;
        }
    }

    private void ImportRecognizedBlendshapes()
    {
        string text = File.ReadAllText(Application.dataPath + "/EasyTalk/Editor/RecognizedBlendshapes.csv");
        List<List<string>> recognizedBlendshapes = CsvFileParser(text);

        // Auto add SIL to recognizedBlendshapes for all languages
        Dictionary<string, List<MeshBlendshape>> meshBlendshapes = new Dictionary<string, List<MeshBlendshape>>();
        foreach (List<string> row in recognizedBlendshapes)
        {
            string blendshapeType = row[0];
            if (meshBlendshapes.ContainsKey(blendshapeType) == false)
            {
                meshBlendshapes.Add(blendshapeType, new List<MeshBlendshape>());
            }

            for (int i = 2; i < row.Count; i+=3)
            {
                if (string.IsNullOrEmpty(row[i + 2]) == false)
                {
                    MeshBlendshape meshBlendshape = new MeshBlendshape()
                    {
                        Mesh = row[i],
                        Blendshape = row[i + 1],
                        Weight = float.Parse(row[i + 2])
                    };

                    if (meshBlendshapes[blendshapeType].Contains(meshBlendshape) == false)
                    {
                        meshBlendshapes[blendshapeType].Add(meshBlendshape);
                    }
                }
            }
        }
        foreach (KeyValuePair<string, List<MeshBlendshape>> kvp in meshBlendshapes)
        {
            List<string> silRow = new List<string>();
            string blendshapeType = kvp.Key;
            silRow.Add(blendshapeType);
            silRow.Add("SIL");
            foreach (MeshBlendshape meshBlendshape in kvp.Value)
            {
                silRow.Add(meshBlendshape.Mesh);
                silRow.Add(meshBlendshape.Blendshape);
                silRow.Add("0");
            }
            recognizedBlendshapes.Add(silRow);
        }

        // Cycle through recognizedBlendshapes and add to our PhonemeDictionaries
        foreach (KeyValuePair<BlendshapeType, List<Phoneme>> kvp in PhonemeDictionaries)
        {
            BlendshapeType blendshapeType = kvp.Key;
            string blendshapeTypeString = blendshapeType.ToString();
            List<Phoneme> phonemes = kvp.Value;
            foreach (Phoneme phoneme in phonemes)
            {
                string phonemename = phoneme.PhonemeName;
                foreach (List<string> row in recognizedBlendshapes)
                {
                    if (row[0] == blendshapeTypeString && 
                        row[1] == phoneme.PhonemeName)
                    {
                        phoneme.RecognizedBlendShapeNames = new List<BlendshapeInfo>();
                        for (int i = 2; i < row.Count; i += 3)
                        {
                            string meshName = row[i];
                            string blendshapeName = row[i + 1];                            
                            if (string.IsNullOrEmpty(meshName) == false && string.IsNullOrEmpty(blendshapeName) == false)
                            {
                                float weight = float.Parse(row[i + 2]);
                                phoneme.RecognizedBlendShapeNames.Add(new BlendshapeInfo()
                                {
                                    MeshName = meshName,
                                    BlendshapeName = blendshapeName,
                                    Weight = weight
                                });
                            }
                        }
                    }
                }
            }
        }
    }

    private List<List<string>> CsvFileParser(string text)
    {
        List<List<string>> result = new List<List<string>>();

        int numColumns = -1;
        bool addOneToColumnCount = false;
        string[] lines = text.Split(new char[] { '\n' });
        foreach (string line in lines)
        {
            if (line.EndsWith(",\r"))
                addOneToColumnCount = true;

            string row = line.Replace("\r", "");

            List<string> cells = new List<string>();

            int last = -1;
            int current = 0;
            bool inText = false;

            while (current < row.Length)
            {
                switch (row[current])
                {
                    case '"':
                        inText = !inText;
                        break;
                    case ',':
                        if (!inText)
                        {
                            string cellData = row.Substring(last + 1, (current - last)).Trim(' ', ',');
                            if (cellData.StartsWith("\"") && cellData.EndsWith("\""))
                                cellData = cellData.Substring(1, cellData.Length - 2);
                            cellData = cellData.Replace("\"\"", "\"");
                            cells.Add(cellData);
                            last = current;
                        }
                        break;
                    default:
                        break;
                }
                current++;
            }

            if (last != row.Length - 1)
            {
                string cellData = row.Substring(last + 1).Trim();
                if (cellData.StartsWith("\"") && cellData.EndsWith("\""))
                    cellData = cellData.Substring(1, cellData.Length - 2);
                cellData = cellData.Replace("\"\"", "\"");
                cells.Add(cellData);
            }

            if (numColumns == -1)
            {
                numColumns = cells.Count;
                if (addOneToColumnCount)
                    numColumns += 1;
            }

            // This can happen if the last column is empty
            if (cells.Count == numColumns - 1)
            {
                cells.Add("");
            }

            if (numColumns != cells.Count)
            {
                // Num cell columns not equal to inital number. 
                // Could be a bad CSV file or there is an extra line 
                // in the file at end... We're done!
                break;
            }
            else
            {
                result.Add(cells);
            }
        }
        return result;
    }


}