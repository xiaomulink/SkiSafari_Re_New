using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Mu_SyncTrans))]
public class Mu_SyncTrans_View : Editor
{
    Texture2D titleLabel;

    void OnEnable()
    {
        titleLabel = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/MuNet1.1/EditorImages/MuNet_Mu_SyncTrans_View_Tilte.png");
    }
 
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();


        GUILayout.Label(titleLabel, new[] { GUILayout.Height(100) });

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.Label("Test");

        base.OnInspectorGUI();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
