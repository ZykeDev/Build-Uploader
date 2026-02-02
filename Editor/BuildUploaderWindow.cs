using UnityEditor;
using UnityEngine;

namespace Noya.BuildUploader
{
	public class BuildUploaderWindow : EditorWindow
	{
		private string[] tabs;
		private int currentTab;
		private BuildUploaderSettings settings;
		
		
		[MenuItem("Tools/Build Uploader", false, 90)]
		public static void ShowWindow()
		{
			BuildUploaderWindow window = GetWindow<BuildUploaderWindow>("Build Uploader");
			window.minSize = new Vector2(450, 350);
			window.Show();
		}

		private void OnEnable()
		{
			settings = BuildUploaderSettings.GetOrCreateSettings();
			
			LoadTabs();
			LoadPrefsData();
		}

		private void LoadTabs()
		{
			tabs = settings.GetUploaderNames();
			currentTab = 0;
		}

		private void LoadPrefsData()
		{
			// Load all EditorPrefs data
		}

		private void OnGUI()
		{
			settings ??= BuildUploaderSettings.GetOrCreateSettings();
			
			// Draw tabs with a custom "+" button
			EditorGUILayout.BeginHorizontal();
    
			const int TAB_HEIGHT = 28;
			currentTab = GUILayout.Toolbar(currentTab, tabs, GUILayout.Height(TAB_HEIGHT));
    
			// Add the "+" button as a square tab
			GUIStyle tabButtonStyle = new GUIStyle(EditorStyles.toolbarButton) { fixedWidth = TAB_HEIGHT, fixedHeight = TAB_HEIGHT, fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
			if (GUILayout.Button("+", tabButtonStyle, GUILayout.Width(TAB_HEIGHT), GUILayout.Height(TAB_HEIGHT)))
			{
				BuildUploaderSettingsProvider.AddCustomUploader();
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();

			settings.GetUploaders()[currentTab].DrawGUI();
			
			Repaint();
		}
	}
}
