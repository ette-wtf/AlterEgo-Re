using System;
using UnityEngine;

[Serializable]
public class BuildManifest
{
	[SerializeField]
	private string scmCommitId;

	[SerializeField]
	private string scmBranch;

	[SerializeField]
	private string buildNumber;

	[SerializeField]
	private string buildStartTime = DateTime.Now.ToUniversalTime().ToString();

	[SerializeField]
	private string projectId;

	[SerializeField]
	private string bundleId;

	[SerializeField]
	private string unityVersion;

	[SerializeField]
	private string xcodeVersion;

	[SerializeField]
	private string cloudBuildTargetName;

	public string ScmCommitId
	{
		get
		{
			return scmCommitId;
		}
	}

	public string ScmBranch
	{
		get
		{
			return scmBranch;
		}
	}

	public string BuildNumber
	{
		get
		{
			return buildNumber;
		}
	}

	public string BuildStartTime
	{
		get
		{
			return buildStartTime;
		}
	}

	public string ProjectId
	{
		get
		{
			return projectId;
		}
	}

	public string BundleId
	{
		get
		{
			return bundleId;
		}
	}

	public string UnityVersion
	{
		get
		{
			return unityVersion;
		}
	}

	public string XCodeVersion
	{
		get
		{
			return xcodeVersion;
		}
	}

	public string CloudBuildTargetName
	{
		get
		{
			return cloudBuildTargetName;
		}
	}

	public static BuildManifest Load()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
		if (textAsset == null)
		{
			textAsset = Resources.Load<TextAsset>("UnityCloudBuildManifestLocal.json");
			if (textAsset == null)
			{
				return new BuildManifest();
			}
			return JsonUtility.FromJson<BuildManifest>(textAsset.text);
		}
		return JsonUtility.FromJson<BuildManifest>(textAsset.text);
	}
}
