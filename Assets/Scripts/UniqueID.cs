using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Nabbed from
// https://answers.unity.com/questions/1780694/how-to-get-some-unique-identifier-for-gameobjects.html
public class UniqueID : MonoBehaviour
{
    //I would suggest adding some kind of ReadOnly attribute/inspector to this such that you can see it in the inspector but can't edit it directly
    [SerializeField] private string _guid = Guid.NewGuid().ToString();
    //If you need to access the ID, use this
    public string guid => _guid;

    //This allows you to re-generate the GUID for this object by clicking the 'Generate New ID' button in the context menu dropdown for this script
    [ContextMenu("Generate new ID")]
    private void RegenerateGUID() => _guid = Guid.NewGuid().ToString();
}