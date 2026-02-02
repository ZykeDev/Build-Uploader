using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Noya.BuildUploader
{
	[PublicAPI]
	internal class ItchioUploader : Uploader
	{
		private ItchioUploaderSettings settings;
		private Editor dataEditor;
		

		public override void Initialize(UploaderSettings settings)
		{
			this.settings = settings as ItchioUploaderSettings;
		}

		public override string GetBuildPath()
		{
			throw new NotImplementedException();
		}

		public override string GetBuiltDestination()
		{
			throw new NotImplementedException();
		}

		public override string[] GetUploadParams()
		{
			throw new NotImplementedException();
		}

		public override void DrawGUI()
		{
			DrawSettings();
			DrawUploadButton();
			DrawProgressBar();
		}

		private void DrawSettings()
		{
			Editor.CreateCachedEditor(settings, typeof(ItchioUploaderSettingsEditor), ref dataEditor);

			if (dataEditor)
			{
				dataEditor.OnInspectorGUI();
			}
		}

		private void DrawUploadButton()
		{
			GUI.enabled = !isUploading;
			if (GUILayout.Button("Upload Build", GUILayout.Height(50)))
			{
				Upload().Forget();
			}

			GUI.enabled = true;
			EditorGUILayout.Space();
		}

		private void DrawProgressBar()
		{
			if (!isUploading)
				return;

			GUILayout.Label(uploadStatus, EditorStyles.boldLabel);

			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 1.5f);
			EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f, 1f)); // Background
			Rect progressRect = new Rect(rect.x, rect.y, rect.width * uploadProgress, rect.height);
			EditorGUI.DrawRect(progressRect, new Color(0.2f, 0.6f, 0.8f, 1f)); // Fill

			GUIStyle centeredStyle = new GUIStyle(GUI.skin.label)
			{
				alignment = TextAnchor.MiddleCenter,
				normal = { textColor = Color.white }
			};
			GUI.Label(rect, $"{uploadProgress * 100:0}%", centeredStyle);
			EditorGUILayout.Space();
		}

		public override async UniTask Upload()
		{
			uploadCTS = new CancellationTokenSource();
			var utcs = new UniTaskCompletionSource<int>();

			isUploading = true;
			uploadStatus = "Initializing...";
			uploadProgress = 0;
			bytesToUpload = 0;
			bytesUploaded = 0;

			// TODO force a repaint?

			// string args = $"push \"{localBuildLocation}\" {user}/{game}:{channel}";
			//
			// if (!string.IsNullOrEmpty(version))
			// 	args += $" --userversion {version}";
			//
			// string command = BUTLER_COMMAND;
			//
			// if (!isButlerInPATH)
			// {
			// 	command = Path.Combine(optionalButlerLocation, "butler.exe");
			//
			// 	if (!File.Exists(command))
			// 	{
			// 		Debug.LogError($"Butler not found at: {command}");
			// 		isUploading = false;
			// 		return;
			// 	}
			// }

			// ProcessStartInfo startInfo = new ProcessStartInfo
			// {
			// 	FileName = command,
			// 	Arguments = args,
			// 	UseShellExecute = false,
			// 	RedirectStandardOutput = true,
			// 	RedirectStandardError = true,
			// 	CreateNoWindow = true
			// };
			//
			// // if (!isButlerInPATH)
			// // {
			// // 	startInfo.WorkingDirectory = Path.GetDirectoryName(Application.dataPath) ?? startInfo.WorkingDirectory;
			// // }
			//
			// using Process process = new Process();
			// process.StartInfo = startInfo;
			// process.EnableRaisingEvents = true;
			//
			// process.OutputDataReceived += (sender, e) =>
			// {
			// 	if (!string.IsNullOrEmpty(e.Data)) Debug.Log($"[Butler] {e.Data}");
			// };
			// process.ErrorDataReceived += (sender, e) =>
			// {
			// 	if (!string.IsNullOrEmpty(e.Data)) Debug.LogError($"[Butler] {e.Data}");
			// };
			// process.Exited += (sender, e) =>
			// {
			// 	utcs.TrySetResult(process.ExitCode);
			// 	uploadCTS.Cancel();
			// };
			//
			// uploadStatus = "Uploading...";
			// process.Start();
			// process.BeginOutputReadLine();
			// process.BeginErrorReadLine();
			//
			// int exitCode = await utcs.Task;
			//
			// if (exitCode != 0)
			// {
			// 	Debug.LogError("error");
			// }
			//
			isUploading = false;
		}
	}
}
