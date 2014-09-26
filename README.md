# time-based-version

MSBuild target for auto-generating a C# assembly version or JS file using the current date\time. 

## How To Use

Add time-based-version nuget package to your project by running the following command in the Package Manager Console:
```
PM> Install-Package time-based-version
```

Once you have the package added to your project you can set a project item to be your version file. To do this, open your CSProj in a text editor and add an item to an ```ItemGroup``` element like this:
```xml
<ItemGroup>
  <Compile Include="Properties\Version.cs">
    <IsVersionInfo>True</IsVersionInfo>
    <Major>1</Major>
    <Minor>0</Minor>
    <Namespace>$(RootNamespace)</Namespace>
  </Compile>
</ItemGroup>
```

Lastly, make sure to remove the duplicate version attributes in your AssemblyInfo.cs file

```c#
// Remove these
[assembly: AssemblyVersion("1.1.0.*")]
[assembly: AssemblyFileVersion("1.1.0.0")]
```

# Supported Extensions

### .cs

A Compile or Content item in your CSProj with a file extension of *.cs will produce a C# file that contains an AssemblyInfo class and assembly version attributes. This will look something like this:

```c#
[assembly: System.Reflection.AssemblyVersion(VersionMSBuildTest.AssemblyInfo.VersionString)]
[assembly: System.Reflection.AssemblyFileVersion(VersionMSBuildTest.AssemblyInfo.VersionString)]

namespace VersionMSBuildTest
{
    internal static class AssemblyInfo
    {
        public const string VersionString = "1.0.50926.1609";
        public static readonly System.Version Version = new System.Version(VersionString);
    }
}
```

### .js

A Compile or Content item in your CSProj with a file extension of *.js will produce a [RequireJS](http://requirejs.org/) type file for use in your Single Page application. This will look something like this:

```javascript
define(function() {
    return {"version": "1.0.50926.1609"}
});
```
