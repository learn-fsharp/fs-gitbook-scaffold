// --------------------------------------------------------------------------------------
// Builds the documentation from `.fsx` and `.md` files in the 'src' directory
// (the generated documentation is stored in the 'book' directory)
// --------------------------------------------------------------------------------------
open System
#I @"packages/FAKE/tools"
#load @"packages/FSharp.Formatting/FSharp.Formatting.fsx"
#r "FakeLib.dll"
open Fake
open System.IO
open FSharp.Literate
open FSharp.MetadataFormat

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__


let script = @"src/ch_01/test.fsx"

Literate.ProcessScriptFile
    (   script
    ,   @"fs-formatting/template-file.html"
    ,   generateAnchors = true
    ,   lineNumbers     = false
    )