using System;
using UnityEngine;

namespace Noya.BuildUploader
{
	/// <summary>
	/// Base class for uploader configuration data.
	/// Inherit from this to create data containers for your uploaders.
	/// </summary>
	public abstract class UploaderSettings : ScriptableObject
	{
		/// <summary>
		/// Returns the Type of the Uploader that uses this data.
		/// Override this to link your data to your uploader implementation.
		/// </summary>
		public abstract Type GetUploaderType();
	}
}
