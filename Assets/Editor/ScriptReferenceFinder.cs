using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class ScriptReferenceFinder : EditorWindow
{
    // Store scroll position for scrollable results
    private Vector2 scrollPos;

    // Store found result strings
    private List<(string line, GameObject obj)> results = new List<(string, GameObject)>(); 
    
    //Search input
    private string searchTerm = "";
    
    //results with gameobject
    

    // Add menu option under Tools > Script Reference Finder
    [MenuItem("Tools/Script Reference Finder")]
    public static void ShowWindow()
    {
        GetWindow<ScriptReferenceFinder>("Script Finder");
    }

    private void OnGUI()
    {
        // Title
        GUILayout.Label("Script Reference Finder", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        searchTerm = EditorGUILayout.TextField("Search Term", searchTerm);
        
        //clearing button
        if (!string.IsNullOrEmpty(searchTerm))
        {
            if (GUILayout.Button("Clear Search", GUILayout.MaxWidth(120)))
            {
                searchTerm = "";
            }
        }
        EditorGUILayout.Space();
        
        // Scan buttonj
        GUI.backgroundColor = new Color(1f, 0.5f, 0f); //the color must be on top and then must be resetted back to white
        if (GUILayout.Button("Scan Scene"))
        {
            ScanSceneForScripts();
        }
        
        //Reference Button
        GUI.backgroundColor = new Color(1f, 0.5f, 0f); //using new color to make custom colors 
        if (GUILayout.Button("Find References"))
        {
            FindReferences();
        }
        
        GUI.backgroundColor = Color.white;//reseting the buttons
        
        // Display results in a scrollable area
        if (results.Count() > 0)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (var (line, obj) in results)
            {
                if (!string.IsNullOrEmpty(searchTerm) && !line.ToLower().Contains(searchTerm.ToLower()))
                    continue;
                
                //color organisation
                if (line.Contains("Missing"))
                    GUI.color = Color.red;
                else if (line.Contains("No Scripts"))
                    GUI.color = Color.yellow;
                else
                GUI.color = Color.green;
                //color organisation
                GUILayout.Label(line);
                
                if (GUILayout.Button("Highlight", GUILayout.MaxWidth(80)))
                {
                    EditorGUIUtility.PingObject(obj);
                    Selection.activeObject = obj;
                    SceneView.lastActiveSceneView.FrameSelected();
                }

                if (line.StartsWith(" This is the "))
                {
                    int startIndex = " This is the ".Count();
                    int endIndex = line.IndexOf(" is on:");
                    if (endIndex > startIndex)
                    {
                        string scriptName = line.Substring(startIndex, endIndex - startIndex).Trim();

                        // Find the script by name
                        string[] guids = AssetDatabase.FindAssets(scriptName + " t:script");
                        if (guids.Count() > 0)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                            Object scriptAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                            if (GUILayout.Button("Locate", GUILayout.MaxWidth(60)))
                            {
                                EditorGUIUtility.PingObject(scriptAsset);
                                Selection.activeObject = scriptAsset;
                                
                            }
                        }
                        else
                        {
                            GUILayout.Label("No Scripts are here", GUILayout.MaxWidth(100));
                        }
                    }
                }
            }

            GUI.color = Color.white;
            GUILayout.EndScrollView();
        }
    }



    private void ScanSceneForScripts()
    {
        results.Clear();

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
            if (scripts.Length == 0)
            {
                results.Add(($"[ ] {obj.name} - No Scripts are added here", obj));
                continue;
            }

            foreach (var script in scripts)
            {
                if (script == null)
                {
                    results.Add(($"Missing {obj.name}", obj));
                }
                else
                {
                    string scriptName = script.GetType().Name;
                    results.Add(($" The object is {obj.name} - Script: {scriptName}", obj));
                }
            }
        }

    }

    private void FindReferences()
    {
        results.Clear();

        Dictionary<string, List<GameObject>> scriptToObjects = new Dictionary<string, List<GameObject>>();

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script == null) continue;

                string scriptName = script.GetType().Name;
                if (!scriptToObjects.ContainsKey(scriptName))
                {
                    scriptToObjects[scriptName] = new List<GameObject>();
                }

                scriptToObjects[scriptName].Add(obj);
            }
        }

        foreach (var kvp in scriptToObjects)
        {
            string scriptName = kvp.Key;
            List<GameObject> objs = kvp.Value;

            foreach (var obj in objs)
            {
                results.Add(($" This is the {scriptName} is on: {obj.name}", obj));
            }
        }
    }

}
