using Cysharp.Threading.Tasks;

namespace Noya.BuildUploader
{
	public interface IUploader
	{
		string GetBuildPath();
		string GetBuiltDestination();
		string[] GetUploadParams();
		UniTask Upload();
	}
}
