#region Using directives

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

#endregion

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle(@"")]
[assembly: AssemblyDescription(@"")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(@"Dyvenix")]
[assembly: AssemblyProduct(@"GenIt")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: System.Resources.NeutralResourcesLanguage("en")]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion(@"1.0.0.0")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]

//
// Make the Dsl project internally visible to the DslPackage assembly
//
[assembly: InternalsVisibleTo(@"Dyvenix.GenIt.DslPackage, PublicKey=002400000480000094000000060200000024000052534131000400000100010031C5F94C37BDAD98EAEC57AC4FC8C53BEAB7502181AA8497772FF317B297A112DA90CB32A9D0A4439409C7913DB8B2A5CBD576D245725C170319FC386F8B39B6848F389F7FA768A54E4ABC3BC809AF6B9B77F4DFC75042B82C0FF019CE2A78D50205CD0984297B4EC615447FA5012418E3D0239B3884D35DCAA6ADAD2D9CC4C9")]