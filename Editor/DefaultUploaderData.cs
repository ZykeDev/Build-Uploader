using UnityEngine;

namespace Noya.BuildUploader
{
	[CreateAssetMenu(fileName = "SteamUploaderSettings", menuName = "Build Uploader/Settings/Steam")]
	public class SteamUploaderSettings : UploaderSettings
	{
		[Header("Steam Settings")]
		public string username;
		public string appId;
		public string depotId;
		public string buildDescription;
        
		[Header("Paths")]
		public string steamCmdPath;
		public string buildOutputPath;

		public override System.Type GetUploaderType() => typeof(DefaultUploader);
	}
}
