using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

namespace Noya.BuildUploader
{
	[Serializable]
	public class UploaderTypeReference
	{
		[SerializeField] private string assemblyQualifiedName;
        
		public Type Type
		{
			get => string.IsNullOrEmpty(assemblyQualifiedName) ? null : Type.GetType(assemblyQualifiedName);
			set => assemblyQualifiedName = value?.AssemblyQualifiedName ?? string.Empty;
		}
        
		public UploaderTypeReference() { }
		public UploaderTypeReference(Type type) => Type = type;
	}

	[Serializable]
	public class UploaderEntry
	{
		public UploaderTypeReference typeReference = new();
		[CanBeNull] public UploaderSettings settings;
	}
	
	[CreateAssetMenu(fileName = "BuildUploaderSettings", menuName = "Build Uploader/BuildUploaderSettings", order = 2)]
	public class BuildUploaderSettings : ScriptableObject
	{
		public const string PROP_UPLOADERS = "uploaders";
		[SerializeField] private List<UploaderEntry> uploaders = new();
		private Uploader[] uploaderInstances;

		
		public static BuildUploaderSettings GetOrCreateSettings()
		{
			var settings = Resources.Load<BuildUploaderSettings>("BuildUploaderSettings");
			Assert.IsNotNull(settings, "BuildUploader settings not found.");
			if (!settings)
			{
				settings = CreateInstance<BuildUploaderSettings>();
				// Ensure you have a Resources folder or handle asset creation via AssetDatabase
			}
			return settings;
		} 
		
		
		public Uploader[] GetUploaders()
		{
			if (uploaderInstances == null || uploaderInstances.Length != uploaders.Count)
			{
				uploaderInstances = new Uploader[uploaders.Count];
				for (int i = 0; i < uploaders.Count; i++)
				{
					Type type = uploaders[i]?.typeReference?.Type;
					if (type != null && typeof(IUploader).IsAssignableFrom(type))
					{
						var uploader = (Uploader)Activator.CreateInstance(type);
						uploader.Initialize(uploaders[i].settings); // Pass the data
						uploaderInstances[i] = uploader;
					}
				}
			}
			return uploaderInstances;
		}

		public Uploader GetUploaderAt(int index)
		{
			return uploaderInstances[index];
		}
		
		public void AddUploaderType(Type uploaderType, UploaderSettings settings = null)
		{
			if (!typeof(IUploader).IsAssignableFrom(uploaderType) || uploaderType.IsAbstract)
				return;

			uploaders.Add(new UploaderEntry 
			{ 
				typeReference = new UploaderTypeReference(uploaderType),
				settings = settings
			});
			uploaderInstances = null; // Invalidate cache
		}

		public void RemoveUploaderAt(int index)
		{
			if (index < 0 || index >= uploaders.Count)
				return;

			uploaders.RemoveAt(index);
			uploaderInstances = null; // Invalidate cache
		}

		/// <summary>
		/// Returns an array of string with the names of the available uploaders.
		/// </summary>
		public string[] GetUploaderNames()
		{
			Uploader[] uploadersList = GetUploaders();
			string[] names = new string[uploadersList.Length];
			for (int i = 0; i < uploadersList.Length; i++)
			{
				names[i] = uploadersList[i]?.GetDisplayName() ?? "Unknown";
			}
			return names;
		}
	}
}
