# Increment Builder

A simple utility for use with MS Build to auto increment the build number in the properties element. This initial, dirty, non-elegant solution does a quick parse to find the `ProductVersion` element, pulls out the version string, and increments the specified symantic position.

This implementation makes several assumptions. One of which is that the build number (3rd or 4th) will never be reset. Second, that the use of versioning will follow a semantic versioning scheme of either `major.minor.patch.buld` or `major.minor.build`. The former being the preferred.

The use of the command is as follows:

```
incbuild <filename> [major|minor|patch|build]
```

Where `build` is the default option used if only `filename` is specified. Additionally specifing one of `major` or `minor` will clear all the lower order version numbers **except** the build number.

Future plans are to use a proper Xml parser and do some better error checking.

Warning: This is a super dirty implementation. Use at your own risk.

# Example usage:

AssemblyInfo.props

```
<?xml version="1.0" encoding="utf-8" ?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <AssemblyName>HelloWorld</AssemblyName>
        <ProductVersion>1.0.0.0</ProductVersion>
        <ProjectGuid>{...}</ProjectGuid>
    </PropertyGroup>
</Project>
```

Project.csproj

```
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

  <Import Project="AssemblyInfo.props"/>
  
  <PropertyGroup>
    <OutputPath>bin\</OutputPath>
    <OutputType>WinExe</OutputType> 
    <ToolsBin Condition=" '$(ToolsBin)' == '' ">\Tools</ToolsBin>
  </PropertyGroup>
  
  <ItemGroup>
    <Compile Include="HelloWorld.cs" />
  </ItemGroup>

  <Target Name="Build" DependsOnTargets="IncrementBuild" />

  <Target Name="IncrementBuild" DependsOnTargets="BuildHelloExe">
    <Exec Command="$(ToolsBin)\incbuild.exe AssemblyInfo.props build" />
  </Target>

  <Target Name="BuildHelloExe">
    <Csc Sources="@(Compile)" OutputAssembly="hello.exe" />
  </Target>
</Project>
```
# MIT open source license

Code is licensed under the MIT open source license.

Copyright 2022, 2023 Charles Freedman

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
