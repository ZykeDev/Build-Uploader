using System.Threading;
using Cysharp.Threading.Tasks;

namespace Noya.BuildUploader
{
	public abstract class Uploader : IUploader
	{
		public virtual string GetDisplayName() => GetType().Name.Replace("Uploader", "");
        
		protected bool isUploading;
		protected string uploadStatus;
		protected float uploadProgress;
		protected CancellationTokenSource uploadCTS;
		protected long bytesUploaded, bytesToUpload;
		

		// Each uploader gets initialized with its data
		public virtual void Initialize(UploaderSettings settings) { }
		
		public abstract string GetBuildPath();
		public abstract string GetBuiltDestination();
		public abstract string[] GetUploadParams();
		public abstract UniTask Upload();
		public abstract void DrawGUI();
	}
}
