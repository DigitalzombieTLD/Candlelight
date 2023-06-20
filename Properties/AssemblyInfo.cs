using MelonLoader;
using Candlelight;
using System.Reflection;
using System.Runtime.InteropServices;


[assembly: AssemblyTitle("Candlelight")]
[assembly: AssemblyCopyright("Digitalzombie")]

//Version numbers in C# are a set of 1 to 4 positive integers separated by periods.
//Mods typically use 3 numbers. For example: 1.2.1
//The mod version need specified in three places.
[assembly: AssemblyVersion("1.3.8")]
[assembly: AssemblyFileVersion("1.3.8")]
[assembly: MelonInfo(typeof(Candlelight_Main), "Candlelight", "1.3.8", "Digitalzombie", null)]

//This tells MelonLoader that the mod is only for The Long Dark.
[assembly: MelonGame("Hinterland", "TheLongDark")]