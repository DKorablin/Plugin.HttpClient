using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("fdddc14d-5911-47bf-92dd-815a383c3f27")]

[assembly: AssemblyTitle("Plugin.HttpClient")]
[assembly: AssemblyDescription("HTTP Test client & server")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.HttpClient")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2015-2024")]

[assembly: AssemblyVersion("1.0.3")]
[assembly: AssemblyFileVersion("1.0.3")]
[assembly: System.CLSCompliant(false)]