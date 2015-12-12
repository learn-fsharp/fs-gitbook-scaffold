//------------------------------------------
// Paket Bootstrap and Package Restoration
//------------------------------------------
open System
open System.IO
open System.Diagnostics
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__


let run' cmd args dir =
    let pinfo = ProcessStartInfo(cmd,args)
    pinfo.WorkingDirectory <- if String.IsNullOrEmpty dir then Environment.CurrentDirectory else dir 
    pinfo.UseShellExecute <- false; pinfo.RedirectStandardOutput <- true
    let proc = Process.Start(pinfo)
    proc.StandardOutput.ReadToEnd() |> printfn "< %s >\n\n%s" cmd; proc.WaitForExit()


if (not<<File.Exists) ".paket\paket.exe" then
    run' @".paket\paket.bootstrapper.exe" "" ""
    
run' @".paket\paket.exe" "restore" ""

//-------------------------
// FAKE Build Targets
//-------------------------

#r @"packages/Fake/tools/Fakelib.dll"

open Fake


let npm args    = run' @"packages\Npm.js\tools\npm.cmd" args "" 
let gitbook args = run' @"node_modules\.bin\gitbook.cmd" args ""


Target "SetupBook" (fun _ ->
    ensureDirectory "node_modules"

    trace "Installing node package `gitbook-cli`"
    npm "install gitbook-cli"

    trace "Initializing new gitbook"
    gitbook "init"     
)

Target "GitbookPlugins" ( fun _ ->
    trace "Installing plugins from `book.json` "
    gitbook "install"
)

Target "SnipGen" (fun _ ->
    let r,_ = executeFSI "." "generate.fsx" []
    tracef "Generated Snippets - %s\n" <| string r
)

Target "BuildBook" (fun _ ->
    gitbook "build"
)


Target "Default" DoNothing

match fsi.CommandLineArgs with
| [|filename|]  -> RunTargetOrDefault "Default"
| arr           -> RunTargetOrDefault arr.[1]
