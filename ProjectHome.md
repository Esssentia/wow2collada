# wow2collada #
VB.NET Project to read data out of World of Warcraft .MPQ Files and exporting them to a Collada File.

The goal is to be able to export any model and complete area including all doodads to a Collada File to be imported into any 3D package of choice in order to create true hybrid machinima.

## Planned features: ##
  * Support all features currently available in the model files
  * Support for Textures, Armatures, Animations
  * Support for M2/SKIN, ADT and WMO

## Current todos: ##
  * Write a Collada Exporter Class (this is pushed back until Blender has a working Collada importer for testing)
  * Write a Blender Exporter Class (using a custom intermediary file format)
  * Write a Blender Importer Plugin (python plugin for the intermediary file format)
  * Unterstand the way animations work in M2s (including animated textures and such)
  * Understand the way textures work in M2s (for Character models)
  * Understand the way submeshes are determined (i.e. which submesh corresponds to which hair style and such)

## Programming Language(s): ##
  * VB.NET
  * C#.NET (MPQ Library / Hex Editor)
  * C++.NET (if absolutely unavoidable)

## Completed stuff: ##
  * Read and Display M2s (mesh + simple textures)
  * Deal with BLP files (native VB.NET Implementation, needs additional debugging)
  * Direct MPQ access (through a C# library)
  * Display ADT including texture splatting (WotLK logic only for now)