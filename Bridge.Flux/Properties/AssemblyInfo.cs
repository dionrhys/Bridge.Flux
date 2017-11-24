using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Bridge.Flux")]
[assembly: AssemblyDescription("Simple bindings for implementing a Flux architecture with Bridge.NET")] // TODO: These aren't really "bindings" but I can't think of a good word to describe these classes - they're essentially architecture enforcers/guidelines?
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Dion Williams")]
[assembly: AssemblyProduct("Bridge.Flux")]
[assembly: AssemblyCopyright("Copyright © 2017 Dion Williams")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("43fc4667-2824-4880-9be7-ac941a2e2252")]

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
// TODO: I'm not sure what to do about Assembly and Assembly File Versioning right now.
// I think when v1 hits we'll set AssemblyVersion to 1.0.0.0 and keep that until a
// breaking v2 release changes it to 2.0.0.0. Need to think about assembly conflicts
// and possibly automatic binding redirects could be helpful and not require us to limit
// to one major version number?
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyFileVersion("0.0.0.0")]

// This version string is the official version of the library and is used by the NuGet package.
// It should follow the rules of Semantic Versioning (http://semver.org/).
// Not sure NuGet understands 0.y.z versions as being "experimental", so the "-alpha" suffix is also added.
[assembly: AssemblyInformationalVersion("0.2.2-alpha")]
