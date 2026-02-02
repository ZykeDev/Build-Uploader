using System;
using UnityEditor;
using UnityEngine;

namespace Noya.BuildUploader
{
	[CreateAssetMenu(fileName = "ItchioUploaderSettings", menuName = "Build Uploader/Settings/Itch.io")]
	internal class ItchioUploaderSettings : UploaderSettings
	{
		internal const string BUTLER_COMMAND = "butler";
		
		internal string BuildPath;
		internal string User;
		internal string Game;
		internal string Channel;
		internal string Version;
		internal bool IsButlerInPATH = true;
		internal string OptionalButlerLocation;
		
		public override Type GetUploaderType() => typeof(ItchioUploaderSettings);
	}

	[CustomEditor(typeof(ItchioUploaderSettings))]
	internal class ItchioUploaderSettingsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			ItchioUploaderSettings uploaderSettings = (ItchioUploaderSettings)target;
			
			serializedObject.Update();
			
			DrawBuildLocation(uploaderSettings);
			DrawBuildDestination(uploaderSettings);
			
			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		private static void DrawBuildLocation(ItchioUploaderSettings uploaderSettings)
		{
			GUILayout.Label("Build Location:", EditorStyles.largeLabel);
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.TextField(uploaderSettings.BuildPath);
				EditorGUI.EndDisabledGroup();

				if (GUILayout.Button("Browse...", GUILayout.Width(80)))
				{
					string path = EditorUtility.OpenFolderPanel("Select the Folder containing the Build", uploaderSettings.BuildPath, "");
					if (!string.IsNullOrEmpty(path))
					{
						uploaderSettings.BuildPath = path;
						EditorUtility.SetDirty(uploaderSettings);
					}
				}
			}

			EditorGUILayout.HelpBox("This should be the folder that contains your 'Build' subfolder (i.e. if it's under 'folder/Build', please select '/folder').", MessageType.Info);
			EditorGUILayout.Space();
		}
		
		private static void DrawBuildDestination(ItchioUploaderSettings uploaderSettings)
		{
			GUILayout.Label("Itch.io Settings", EditorStyles.largeLabel);
		
			uploaderSettings.User = EditorGUILayout.TextField("Username", uploaderSettings.User);
			uploaderSettings.Game = EditorGUILayout.TextField("Game", uploaderSettings.Game);
			uploaderSettings.Channel = EditorGUILayout.TextField("Channel", uploaderSettings.Channel);
			uploaderSettings.Version = EditorGUILayout.TextField("Version", string.IsNullOrEmpty(uploaderSettings.Version) ? Application.version : uploaderSettings.Version);

			if (!string.IsNullOrEmpty(uploaderSettings.User) && !string.IsNullOrEmpty(uploaderSettings.Game) && !string.IsNullOrEmpty(uploaderSettings.Channel))
			{
				EditorGUILayout.LabelField($"Command: butler push [directory] {uploaderSettings.User}/{uploaderSettings.Game}:{uploaderSettings.Channel}", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleRight });
			}

			uploaderSettings.IsButlerInPATH = EditorGUILayout.Toggle("Use butler in PATH", uploaderSettings.IsButlerInPATH);
		
			if (!uploaderSettings.IsButlerInPATH)
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Custom Butler path");
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.TextField(uploaderSettings.OptionalButlerLocation);
					EditorGUI.EndDisabledGroup();
				
					if (GUILayout.Button("Browse...", GUILayout.Width(80)))
					{
						string path = EditorUtility.OpenFolderPanel("Select the folder containing butler", uploaderSettings.OptionalButlerLocation, "");
						if (!string.IsNullOrEmpty(path))
						{
							uploaderSettings.OptionalButlerLocation = path;
							EditorUtility.SetDirty(uploaderSettings);
						}
					}
				}
			}

			EditorGUILayout.Space();
		}
	}
}
