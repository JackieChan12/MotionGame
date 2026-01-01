using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
[CustomEditor(typeof(TestNuitrack))]

public class ImageListHolderEditor : Editor
{
    private GUIStyle boxStyle; 
    private void OnEnable() 
    { 
        
    }

    public override void OnInspectorGUI()
    {
        boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.padding = new RectOffset(10, 10, 10, 10);

        DrawDefaultInspector();

        var listHolder = (TestNuitrack)target;

        // Hiển thị danh sách chính
        for (int i = 0; i < listHolder.listObjectImage.Count; i++)
        {
            EditorGUILayout.LabelField("List " + i);
            EditorGUILayout.BeginVertical(boxStyle);
            if (listHolder.listObjectImage[i] == null) 
            { 
                listHolder.listObjectImage[i] = new List<Image>(); 
            }
            //if (GUILayout.Button("Thêm Image vào List " + i)) 
            //{ 
            //    listHolder.listObjectImage[i].Add(null); 
            //}

            if (listHolder.listObjectImage[i] != null)
            {
                // Hiển thị từng danh sách con
                for (int j = 0; j < listHolder.listObjectImage[i].Count; j++)
                {
                    listHolder.listObjectImage[i][j] = (Image)EditorGUILayout.ObjectField("Sprite " + j, listHolder.listObjectImage[i][j], typeof(Image), false);
                }
            }
            EditorGUILayout.EndVertical();
            //if (GUILayout.Button("Thêm Sprite vào List " + i))
            //{
            //    listHolder.listObjectImage[i].Add(null);
            //}
        }


        //if (GUILayout.Button("Thêm List"))
        //{
        //    listHolder.listObjectImage.Add(new List<Image>());
        //}

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
#endif
