﻿using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace ParrelSync
{
    /// <summary>
    ///Clones manager Unity editor window
    /// </summary>
	public class ClonesManagerWindow : EditorWindow
    {
        /// <summary>
        /// Returns true if project clone exists.
        /// </summary>
        public bool isCloneCreated
        {
            get { return ClonesManager.GetClonePaths().Count >= 1; }
        }

        [MenuItem("Build/Cloning/Clone Manager", priority = 0)]
        private static void InitWindow()
        {
            ClonesManagerWindow window = (ClonesManagerWindow)EditorWindow.GetWindow(typeof(ClonesManagerWindow));
            window.titleContent = new GUIContent("Clones Manager");
            window.Show();
        }

        /// <summary>
        /// For storing the scroll position of clones list
        /// </summary>
        Vector2 clonesScrollPos;

        private void OnGUI()
        {
            /// If it is a clone project...
            if (ClonesManager.IsClone())
            {
                //Find out the original project name and show the help box
                string originalProjectPath = ClonesManager.GetOriginalProjectPath();
                if (originalProjectPath == string.Empty)
                {
                    /// If original project cannot be found, display warning message.
                    EditorGUILayout.HelpBox(
                        "This project is a clone, but the link to the original seems lost.\nYou have to manually open the original and create a new clone instead of this one.\n",
                        MessageType.Warning);
                }
                else
                {
                    /// If original project is present, display some usage info.
                    EditorGUILayout.HelpBox(
                        "This project is a clone of the project '" + Path.GetFileName(originalProjectPath) + "'.\nIf you want to make changes the project files or manage clones, please open the original project through Unity Hub.",
                        MessageType.Info);
                }

                //Clone project custom context.
                EditorGUILayout.LabelField("Context");

                string contextFilePath = Path.Combine(ClonesManager.GetCurrentProjectPath(), ClonesManager.ContextFileName);
                //Need to be careful with file reading / writing since it will effect the deletion of
                //  the clone project(The directory won't be fully deleted if there's still file inside being read or write).
                //The context file will be deleted first at the beginning of the project deletion process
                //to prevent any further being read and write.
                //Will need to take some extra cautious if want to change the design of how file editing is handled.
                if (File.Exists(contextFilePath))
                {
                    string context = File.ReadAllText(contextFilePath, System.Text.Encoding.UTF8);
                    string contextTextAreaInput = EditorGUILayout.TextArea(context,
                        GUILayout.Height(50),
                        GUILayout.MaxWidth(300)
                    );
                    File.WriteAllText(contextFilePath, contextTextAreaInput, System.Text.Encoding.UTF8);
                }
                else
                {
                    EditorGUILayout.LabelField("No context file found.");
                }
            }
            else// If it is an original project...
            {
                if (isCloneCreated)
                {
                    GUILayout.BeginVertical("HelpBox");
                    GUILayout.Label("Clones of this Project");

                    //List all clones
                    clonesScrollPos =
                         EditorGUILayout.BeginScrollView(clonesScrollPos);
                    var cloneProjectsPath = ClonesManager.GetClonePaths();
                    for (int i = 0; i < cloneProjectsPath.Count; i++)
                    {

                        GUILayout.BeginVertical("GroupBox");
                        string cloneProjectPath = cloneProjectsPath[i];

                        bool isOpenInAnotherInstance = ClonesManager.IsCloneProjectRunning(cloneProjectPath);

                        if (isOpenInAnotherInstance == true)
                            EditorGUILayout.LabelField("Clone " + i + " (Running)", EditorStyles.boldLabel);
                        else
                            EditorGUILayout.LabelField("Clone " + i);


                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(cloneProjectPath);
                        if (GUILayout.Button("View Folder", GUILayout.Width(80)))
                        {
                            ClonesManager.OpenProjectInFileExplorer(cloneProjectPath);
                        }
                        GUILayout.EndHorizontal();

                        EditorGUILayout.LabelField("Context");

                        string contextFilePath = Path.Combine(cloneProjectPath, ClonesManager.ContextFileName);
                        //Need to be careful with file reading/writing since it will effect the deletion of
                        //the clone project(The directory won't be fully deleted if there's still file inside being read or write).
                        //The context file will be deleted first at the beginning of the project deletion process 
                        //to prevent any further being read and write.
                        //Will need to take some extra cautious if want to change the design of how file editing is handled.
                        if (File.Exists(contextFilePath))
                        {
                            string context = File.ReadAllText(contextFilePath, System.Text.Encoding.UTF8);
                            string contextTextAreaInput = EditorGUILayout.TextArea(context,
                                GUILayout.Height(50),
                                GUILayout.MaxWidth(300)
                            );
                            File.WriteAllText(contextFilePath, contextTextAreaInput, System.Text.Encoding.UTF8);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No context file found.");
                        }

                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();


                        EditorGUI.BeginDisabledGroup(isOpenInAnotherInstance);

                        if (GUILayout.Button("Open in New Editor"))
                        {
                            ClonesManager.OpenProject(cloneProjectPath);
                        }

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Delete"))
                        {
                            bool delete = EditorUtility.DisplayDialog(
                                "Delete the clone?",
                                "Are you sure you want to delete the clone project '" + Path.GetFileName(cloneProjectPath) + "'?",
                                "Delete",
                                "Cancel");
                            if (delete)
                            {
                                ClonesManager.DeleteClone(cloneProjectPath);
                            }
                        }

                        GUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndVertical();

                    }
                    EditorGUILayout.EndScrollView();

                    if (GUILayout.Button("Add new clone"))
                    {
                        ClonesManager.CreateCloneFromCurrent();
                    }

                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    /// If no clone created yet, we must create it.
                    EditorGUILayout.HelpBox("No project clones found. Create a new one!", MessageType.Info);
                    if (GUILayout.Button("Create new clone"))
                    {
                        ClonesManager.CreateCloneFromCurrent();
                    }
                }
            }
        }
    }
}
