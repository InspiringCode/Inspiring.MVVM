using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("InspiringMvvm")]
[assembly: AssemblyDescription("https://github.com/InspiringCode/Inspiring.MVVM")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("InspiringCode")]
[assembly: AssemblyProduct("Inspiring.MVVM")]
[assembly: AssemblyCopyright("Copyright © InspiringCode")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("aed434b5-9baf-4200-85f2-1c03f8431c6e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: XmlnsDefinition("http://inspiringcode.com/mvvm", "Inspiring.Mvvm")]
[assembly: XmlnsDefinition("http://inspiringcode.com/mvvm", "Inspiring.Mvvm.Views")]

[assembly: InternalsVisibleTo("InspiringMvvmTest")]
[assembly: InternalsVisibleTo("InspiringMvvmTestability")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
