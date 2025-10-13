namespace Plugin.HttpClient.UI
{
	/// <summary>Tree node visual state</summary>
	internal enum NodeStateEnum
	{
		/// <summary>New node that has not been tested yet</summary>
		New = 0,
		/// <summary>Test currently running</summary>
		Running = 1,
		/// <summary>Test finished successfully</summary>
		Success = 2,
		/// <summary>Test failed</summary>
		Failure = 3,
		/// <summary>Test failed: forbidden (HTTP 403 or similar)</summary>
		FailureForbidden = 4,
		/// <summary>Folder placeholder (tests folders not implemented)</summary>
		Folder = 5,
		/// <summary>Skip this test when running batch or all URLs</summary>
		Skip = 6,
	}
}