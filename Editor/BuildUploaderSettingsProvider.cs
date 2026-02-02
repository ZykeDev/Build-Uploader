using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noya.BuildUploader
{
	public class BuildUploaderSettingsProvider : SettingsProvider
	{
		private SerializedObject customSettings;
		
		
		public BuildUploaderSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
			: base(path, scopes, keywords)
		{
			
		}
		
		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			// Load settings when the section is opened
			customSettings = new SerializedObject(BuildUploaderSettings.GetOrCreateSettings());
		}
		
		public override void OnGUI(string searchContext)
		{
			EditorGUILayout.PropertyField(customSettings.FindProperty(BuildUploaderSettings.PROP_UPLOADERS), true);
			EditorGUILayout.Space();
			if (GUILayout.Button("Add Custom Uploader", GUILayout.Height(28)))
			{
				AddCustomUploader();
			}
			
			customSettings.ApplyModifiedProperties();
		}
		
		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider()
		{
			BuildUploaderSettingsProvider provider = new("Project/Build Uploader", SettingsScope.Project)
			{
				keywords = new[] { "Build", "Uploader", "Upload", "Noya", "builduploader" }
			};

			return provider;
		}

		public static void AddCustomUploader()
		{
			// Ask the user for confirmation
			bool create = EditorUtility.DisplayDialog("Create a new Uploader?",
				"You can create custom Uploaders to upload your build files to a specific service.\n" +
				"Clicking Create will create a new ScriptableObject script at your selected location. " +
				"Once you have set it up, you must create a new class that inherits from 'Uploader' and implements the necessary methods.\n" +
				"See the ItchioUploader.cs class for an example.",
				"Create", "Cancel");

			if (!create)
				return;
			
			
			string path = EditorUtility.SaveFilePanelInProject(
				"Create a Custom Uploader",
				"CustomUploader",
				"cs",
				"Please enter a file name to save the uploader to"
			);
			
			if (string.IsNullOrEmpty(path))
				return;

			
			string fileName = Path.GetFileNameWithoutExtension(path);
			string template = Resources.Load<TextAsset>("DefaultUploader").text;
			template = template.Replace("DefaultUploader", fileName);
			template = template.Replace("class Uploader", $"class {fileName}");
			File.WriteAllText(path, template);
			
			// Unity needs to wait for the new .cs file to be compiled before registering the new Type.
			// To do this, we store a pending instruction in the SessionState and handle it when the domain finishes relaoding.
			SessionState.SetString("PendingUploaderCompilation", fileName);
			
			AssetDatabase.ImportAsset(path);
			AssetDatabase.SaveAssets();
		}
		
		[InitializeOnLoadMethod]
		private static void OnCompilationComplete()
		{
			string typeName = SessionState.GetString("PendingUploaderCompilation", string.Empty);
			if (string.IsNullOrEmpty(typeName))
				return;

			SessionState.EraseString("PendingUploaderCompilation");

			Type type = AppDomain.CurrentDomain.GetAssemblies()
				.Select(assembly => assembly.GetType(typeName))
				.FirstOrDefault(tt => tt != null);

			BuildUploaderSettings settings = BuildUploaderSettings.GetOrCreateSettings();
			settings.AddUploaderType(type);
		}
	}
}
