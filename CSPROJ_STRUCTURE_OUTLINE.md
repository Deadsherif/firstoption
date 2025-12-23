# .csproj File Structure Outline

This document provides a reference outline for building .csproj files for Revit add-ins with multi-version support.

## Table of Contents
1. [Basic Project Structure](#basic-project-structure)
2. [Property Groups](#property-groups)
3. [Revit API References Pattern](#revit-api-references-pattern)
4. [Item Groups](#item-groups)
5. [Common Patterns](#common-patterns)

---

## 1. Basic Project Structure

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <!-- Import Microsoft.Common.props -->
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" 
          Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  
  <!-- All content goes here -->
  
  <!-- Import Microsoft.CSharp.targets at the end -->
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
</Project>
```

---

## 2. Property Groups

### 2.1 Main Property Group (Required)
```xml
<PropertyGroup>
  <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
  <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
  <ProjectGuid>{GUID-HERE}</ProjectGuid>
  <OutputType>Library</OutputType>  <!-- or Exe for executables -->
  <AppDesignerFolder>Properties</AppDesignerFolder>
  <RootNamespace>YourNamespace</RootNamespace>
  <AssemblyName>YourAssemblyName</AssemblyName>
  <TargetFrameworkVersion>v4.8.1</TargetFrameworkVersion>
  <FileAlignment>512</FileAlignment>
  <Deterministic>true</Deterministic>
</PropertyGroup>
```

### 2.2 Standard Configuration Property Groups (Debug/Release)
```xml
<!-- Debug Configuration -->
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
  <DebugSymbols>true</DebugSymbols>
  <DebugType>full</DebugType>
  <Optimize>false</Optimize>
  <OutputPath>bin\Debug\</OutputPath>
  <DefineConstants>DEBUG;TRACE</DefineConstants>
  <ErrorReport>prompt</ErrorReport>
  <WarningLevel>4</WarningLevel>
</PropertyGroup>

<!-- Release Configuration -->
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
  <DebugType>pdbonly</DebugType>
  <Optimize>true</Optimize>
  <OutputPath>bin\Release\</OutputPath>
  <DefineConstants>TRACE</DefineConstants>
  <ErrorReport>prompt</ErrorReport>
  <WarningLevel>4</WarningLevel>
</PropertyGroup>
```

### 2.3 Revit Version-Specific Property Groups
```xml
<!-- R2020 Configuration -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'R2020|AnyCPU'">
  <OutputPath>bin\R2020\</OutputPath>
  <DefineConstants>TRACE;DEBUG;R2020</DefineConstants>
</PropertyGroup>

<!-- R2021 Configuration -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'R2021|AnyCPU'">
  <OutputPath>bin\R2021\</OutputPath>
  <DefineConstants>TRACE;DEBUG;R2021</DefineConstants>
  <PlatformTarget>AnyCPU</PlatformTarget>
</PropertyGroup>

<!-- R2022 Configuration -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'R2022|AnyCPU'">
  <OutputPath>bin\R2022\</OutputPath>
  <DefineConstants>TRACE;DEBUG;R2022</DefineConstants>
</PropertyGroup>

<!-- R2023 Configuration -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'R2023|AnyCPU'">
  <OutputPath>bin\R2023\</OutputPath>
  <DefineConstants>TRACE;DEBUG;R2023</DefineConstants>
</PropertyGroup>

<!-- R2024 Configuration -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'R2024|AnyCPU'">
  <OutputPath>bin\R2024\</OutputPath>
  <DefineConstants>TRACE;DEBUG;R2024</DefineConstants>
</PropertyGroup>
```

---

## 3. Revit API References Pattern

### 3.1 Conditional References (Choose/When Pattern)
This pattern allows different Revit API versions to be used based on the build configuration.

```xml
<!-- Revit-specific references -->
<Choose>
  <When Condition=" '$(Configuration)'=='R2020' ">
    <ItemGroup>
      <Reference Remove="RevitAPI" />
      <Reference Remove="RevitAPIUI" />
      <Reference Include="RevitAPI">
        <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2020\RevitAPI.dll</HintPath>
        <Private>False</Private>
      </Reference>
      <Reference Include="RevitAPIUI">
        <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2020\RevitAPIUI.dll</HintPath>
        <Private>False</Private>
      </Reference>
    </ItemGroup>
  </When>
  
  <When Condition=" '$(Configuration)'=='R2021' ">
    <ItemGroup>
      <Reference Remove="RevitAPI" />
      <Reference Remove="RevitAPIUI" />
      <Reference Include="RevitAPI">
        <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2021\RevitAPI.dll</HintPath>
        <Private>False</Private>
      </Reference>
      <Reference Include="RevitAPIUI">
        <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2021\RevitAPIUI.dll</HintPath>
        <Private>False</Private>
      </Reference>
    </ItemGroup>
  </When>
  
  <When Condition=" '$(Configuration)'=='R2022' ">
    <ItemGroup>
      <Reference Remove="RevitAPI" />
      <Reference Remove="RevitAPIUI" />
      <Reference Include="RevitAPI">
        <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2022\RevitAPI.dll</HintPath>
        <Private>False</Private>
      </Reference>
      <Reference Include="RevitAPIUI">
        <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2022\RevitAPIUI.dll</HintPath>
        <Private>False</Private>
      </Reference>
    </ItemGroup>
  </When>
  
  <When Condition=" '$(Configuration)'=='R2023' ">
    <ItemGroup>
      <Reference Remove="RevitAPI" />
      <Reference Remove="RevitAPIUI" />
      <Reference Include="RevitAPI">
        <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2023\RevitAPI.dll</HintPath>
        <Private>False</Private>
      </Reference>
      <Reference Include="RevitAPIUI">
        <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2023\RevitAPIUI.dll</HintPath>
        <Private>False</Private>
      </Reference>
    </ItemGroup>
  </When>
  
  <When Condition=" '$(Configuration)'=='R2024' ">
    <ItemGroup>
      <!-- R2024 uses the same as unconditional, so no need to remove/add -->
    </ItemGroup>
  </When>
</Choose>
```

### 3.2 Unconditional References (For IntelliSense)
These references are always included so Visual Studio can provide IntelliSense, even when not building with a Revit configuration.

```xml
<!-- Unconditional references for IntelliSense (defaults to R2024) -->
<ItemGroup>
  <Reference Include="RevitAPI">
    <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2024\RevitAPI.dll</HintPath>
    <Private>False</Private>
  </Reference>
  <Reference Include="RevitAPIUI">
    <HintPath>D:\AhmedRashad\firstoption_net48\packages\Revit_All_Main_Versions_API_x64.2024\RevitAPIUI.dll</HintPath>
    <Private>False</Private>
  </Reference>
</ItemGroup>
```

**Important Notes:**
- The unconditional references use R2024 as the default
- When building R2020-R2023, the `Choose/When` block removes the unconditional references and adds version-specific ones
- For R2024, no action is needed since it matches the unconditional references
- `<Private>False</Private>` prevents copying the DLLs to the output directory (Revit provides them)

---

## 4. Item Groups

### 4.1 System References
```xml
<ItemGroup>
  <Reference Include="System" />
  <Reference Include="System.Core" />
  <Reference Include="System.Xml.Linq" />
  <Reference Include="System.Data.DataSetExtensions" />
  <Reference Include="Microsoft.CSharp" />
  <Reference Include="System.Data" />
  <Reference Include="System.Net.Http" />
  <Reference Include="System.Xml" />
  
  <!-- WPF References (if needed) -->
  <Reference Include="System.Xaml">
    <RequiredTargetFramework>4.0</RequiredTargetFramework>
  </Reference>
  <Reference Include="WindowsBase" />
  <Reference Include="PresentationCore" />
  <Reference Include="PresentationFramework" />
</ItemGroup>
```

### 4.2 NuGet Package References
```xml
<ItemGroup>
  <Reference Include="Microsoft.Bcl.AsyncInterfaces, Version=10.0.0.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51, processorArchitecture=MSIL">
    <HintPath>..\packages\Microsoft.Bcl.AsyncInterfaces.10.0.1\lib\net462\Microsoft.Bcl.AsyncInterfaces.dll</HintPath>
  </Reference>
  <!-- More NuGet packages... -->
</ItemGroup>
```

### 4.3 Source Files (Compile)
```xml
<ItemGroup>
  <Compile Include="Command.cs" />
  <Compile Include="MVVM\View\AuthWindow.xaml.cs">
    <DependentUpon>AuthWindow.xaml</DependentUpon>
  </Compile>
  <Compile Include="MVVM\ViewModel\AuthViewModel.cs" />
  <Compile Include="Properties\AssemblyInfo.cs" />
</ItemGroup>
```

### 4.4 XAML Pages (WPF)
```xml
<ItemGroup>
  <Page Include="MVVM\View\AuthWindow.xaml">
    <SubType>Designer</SubType>
    <Generator>MSBuild:Compile</Generator>
  </Page>
</ItemGroup>
```

### 4.5 Other Files
```xml
<ItemGroup>
  <None Include="packages.config" />
  <None Include="YourAddin.addin">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

---

## 5. Common Patterns

### 5.1 NuGet Package Import Targets
```xml
<Import Project="..\packages\System.ValueTuple.4.6.1\build\net471\System.ValueTuple.targets" 
        Condition="Exists('..\packages\System.ValueTuple.4.6.1\build\net471\System.ValueTuple.targets')" />
<Target Name="EnsureNuGetPackageBuildImports" BeforeTargets="PrepareForBuild">
  <PropertyGroup>
    <ErrorText>This project references NuGet package(s) that are missing on this computer. Use NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=322105. The missing file is {0}.</ErrorText>
  </PropertyGroup>
  <Error Condition="!Exists('..\packages\System.ValueTuple.4.6.1\build\net471\System.ValueTuple.targets')" 
         Text="$([System.String]::Format('$(ErrorText)', '..\packages\System.ValueTuple.4.6.1\build\net471\System.ValueTuple.targets'))" />
</Target>
```

### 5.2 Post-Build Events
```xml
<PropertyGroup>
  <PostBuildEvent>
    mkdir "$(AppData)\Autodesk\Revit\Addins\2023\$(ProjectName)\" 2>NUL
    if exist "$(AppData)\Autodesk\Revit\Addins\2023" copy "$(ProjectDir)*.addin" "$(AppData)\Autodesk\REVIT\Addins\2023"
    if exist "$(AppData)\Autodesk\Revit\Addins\2023" copy "$(ProjectDir)$(OutputPath)*.dll" "$(AppData)\Autodesk\REVIT\Addins\2023\$(ProjectName)\"
  </PostBuildEvent>
</PropertyGroup>
```

### 5.3 Project Type GUIDs (for WPF projects)
```xml
<PropertyGroup>
  <ProjectTypeGuids>{60dc8134-eba5-43b8-bcc9-bb4bc16c2548};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
</PropertyGroup>
```

---

## 6. Complete File Structure Order

1. XML Declaration
2. Project Opening Tag
3. Import Microsoft.Common.props
4. Main PropertyGroup
5. Debug PropertyGroup
6. Release PropertyGroup
7. R2020 PropertyGroup
8. R2021 PropertyGroup
9. R2022 PropertyGroup
10. R2023 PropertyGroup
11. R2024 PropertyGroup
12. Choose/When Block (Revit API conditional references)
13. Unconditional Revit API References (for IntelliSense)
14. System References ItemGroup
15. NuGet Package References ItemGroup
16. Compile ItemGroup (source files)
17. Page ItemGroup (XAML files)
18. None ItemGroup (other files)
19. Import NuGet targets
20. NuGet package validation targets
21. Import Microsoft.CSharp.targets
22. Project Closing Tag

---

## 7. Key Points to Remember

1. **Reference Order Matters**: Unconditional references should come AFTER the Choose/When block
2. **Remove Before Add**: When using conditional references, use `<Reference Remove="..."/>` before adding the new one
3. **Private=False**: Always set `<Private>False</Private>` for Revit API references
4. **HintPath Format**: Use absolute paths or relative paths from project root
5. **Configuration Names**: Must match exactly (case-sensitive): R2020, R2021, R2022, R2023, R2024
6. **IntelliSense**: Unconditional references ensure Visual Studio can provide code completion
7. **Build-Time**: Conditional references ensure the correct version is used during compilation

---

## 8. Quick Reference Checklist

When creating a new .csproj file:

- [ ] XML declaration and Project opening tag
- [ ] Import Microsoft.Common.props
- [ ] Main PropertyGroup with ProjectGuid, OutputType, etc.
- [ ] Debug and Release PropertyGroups
- [ ] R2020-R2024 PropertyGroups
- [ ] Choose/When block for conditional Revit API references
- [ ] Unconditional Revit API references (for IntelliSense)
- [ ] System references ItemGroup
- [ ] Compile ItemGroup (all .cs files)
- [ ] Page ItemGroup (if using WPF/XAML)
- [ ] Import Microsoft.CSharp.targets
- [ ] Project closing tag

---

## 9. Example: Minimal Revit Add-in .csproj

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" 
          Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{YOUR-GUID-HERE}</ProjectGuid>
    <OutputType>Library</OutputType>
    <RootNamespace>YourNamespace</RootNamespace>
    <AssemblyName>YourAssembly</AssemblyName>
    <TargetFrameworkVersion>v4.8.1</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <Deterministic>true</Deterministic>
  </PropertyGroup>
  
  <!-- Add Debug, Release, R2020-R2024 PropertyGroups here -->
  
  <!-- Add Choose/When block for conditional Revit references -->
  
  <!-- Add unconditional Revit references -->
  
  <!-- Add System references -->
  
  <!-- Add Compile ItemGroup -->
  
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
</Project>
```

---

**Last Updated**: Based on AuthService.csproj structure
**Version**: 1.0
