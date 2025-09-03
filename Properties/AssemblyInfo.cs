using System.Reflection;
using System.Runtime.InteropServices;

using org.efool.subnautica.seaglide_sprint;

[assembly: AssemblyTitle(Info.title)]
[assembly: AssemblyDescription(Info.desc)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("efool")]
[assembly: AssemblyProduct(Info.name)]
[assembly: AssemblyCopyright("Copyright 2025")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion(Info.version)]
[assembly: AssemblyFileVersion(Info.version)]

[assembly: ComVisible(false)]

[assembly: Guid("89ed9a66-7fb7-435b-a89d-a277e2d0df13")]

namespace org.efool.subnautica.seaglide_sprint {
public static class Info
{
	public const string FQN = "org.efool.subnautica.seaglide_sprint";
	public const string name = "efool-seaglide-sprint";
	public const string title = "efool's Seaglide Sprint";
	public const string desc = "efool's seaglide sprint mod for Subnautica";
	public const string version = "0.0.4";
}
}