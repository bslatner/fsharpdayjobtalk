#if !FAKE
  #r "netstandard"
#endif

#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"

open System
open System.Collections.Generic
open System.IO
open System.Text
open Fake.BuildServer
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators
open Fake.Runtime.Trace
open Newtonsoft.Json
open Newtonsoft.Json.Linq

////////////////////////////////////////////////////////////////////////////////
// HELPERS
////////////////////////////////////////////////////////////////////////////////

let (|CaseInsensitiveEqual|_|) (str:string) arg = 
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    then Some() else None

let strJoin (sep : string) (strs : seq<string>) = String.Join(sep, strs)

////////////////////////////////////////////////////////////////////////////////
// BUILD OPTIONS
////////////////////////////////////////////////////////////////////////////////

tracefn "config=%s" (Environment.environVar "config")

let buildConfigurationString = Environment.environVarOrDefault "config" "Debug"
let isBuildConfig c = String.Equals(buildConfigurationString, c, StringComparison.CurrentCultureIgnoreCase)

////////////////////////////////////////////////////////////////////////////////
// CUSTOM OPTIONS FOR DotNetCli
////////////////////////////////////////////////////////////////////////////////

let DefaultDotNetCliDir = @"C:\Program Files\dotnet" @@ (if Environment.isUnix then "dotnet" else "dotnet.exe")

let buildConfiguration = 
    match buildConfigurationString with
    | CaseInsensitiveEqual "release" -> DotNet.BuildConfiguration.Release
    | CaseInsensitiveEqual "debug" -> DotNet.BuildConfiguration.Debug
    | n -> DotNet.BuildConfiguration.Custom n

let ourOptions (opt : DotNet.Options) =
    { opt with DotNetCliPath = DefaultDotNetCliDir }

let defaultDotNetOptions proj (opt : DotNet.Options) =
    { ourOptions opt with WorkingDirectory = Path.GetDirectoryName(proj) }

let dnBuildOptions (opt : DotNet.BuildOptions) =
    { opt with Configuration = buildConfiguration; Common = ourOptions opt.Common }

let DotNetClean() =
    DotNet.exec ourOptions "clean" ""

let DotNetRun proj =
    DotNet.exec (defaultDotNetOptions proj) "run" ""

////////////////////////////////////////////////////////////////////////////////
// BUILD TARGETS
////////////////////////////////////////////////////////////////////////////////

Target.create "Clean" <| fun _ ->
    // clean up project outputs
    let r = DotNetClean()
    if r.ExitCode <> 0 then
        failwithf "Clean failed"

Target.create "BuildProjects" <| fun _ ->
    DotNet.build dnBuildOptions "FSharpDayJobTalk.sln"

Target.create "RunTests" <| fun _ ->
    let p = "FSharpLibraryTest/FSharpLibraryTest.fsproj"
    let r = p |> DotNetRun
    if r.ExitCode <> 0 then
        failwithf "One or more tests failed in %s" p

Target.create "Default" <| ignore

open Fake.Core.TargetOperators

// dependencies
"Clean"
    ==> "BuildProjects"
    ==> "RunTests"
    ==> "Default"

// start build
Target.runOrDefault "Default"
