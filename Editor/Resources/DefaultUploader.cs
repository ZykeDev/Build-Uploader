using System;
using Cysharp.Threading.Tasks;
using Noya.BuildUploader;

public class DefaultUploader : Uploader
{
	private UploaderSettings settings;

	
	public override void Initialize(UploaderSettings settings)
	{
		this.settings = settings;
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

	public override async UniTask Upload()
	{
		throw new NotImplementedException();
	}

	public override void DrawGUI()
	{
		throw new NotImplementedException();
	}
}
