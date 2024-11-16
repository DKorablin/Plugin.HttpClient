namespace Plugin.HttpClient.Project
{
	public enum AuthorizationType
	{
		/// <summary>Don't use any authorization</summary>
		None = 0,
		/// <summary>In a Windows network, NT (New Technology) LAN Manager (NTLM) is a suite of Microsoft security protocols intended to provide authentication, integrity, and confidentiality to users</summary>
		NTLM = 1,
		/// <summary>In the context of an HTTP transaction, basic access authentication is a method for an HTTP user agent (e.g. a web browser) to provide a user name and password when making a request</summary>
		Basic = 2,
	}
}