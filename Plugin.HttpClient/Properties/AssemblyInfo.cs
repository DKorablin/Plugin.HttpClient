using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("fdddc14d-5911-47bf-92dd-815a383c3f27")]
[assembly: System.CLSCompliant(false)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=99")]
#else

[assembly: AssemblyTitle("Plugin.HttpClient")]
[assembly: AssemblyProduct("Plugin.HttpClient")]
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2015-2024")]
[assembly: AssemblyDescription("HTTP Test client & server")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

#endif