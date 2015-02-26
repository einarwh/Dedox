Dedox
=====

Detox for your .NET documentation.

You can run Dedox on a single .cs file, all the files indicated by a .csproj file, or all the files indicated by a .sln file.

Dedox will output .cs files where useless comments have been stripped to an output folder. By default, this folder is C:\temp, but obviously you can override that. Dedox supports several flags:

 -o <output directory>: set output directory (default: C:\temp) <br/>
 -v : verbose mode <br/>
 -vv : very verbose mode <br/>
 -g : include generated code <br/>
 -m : enable metrics (really statistics) <br/>
 -l : set Levenshtein limit (default: 2) <br/>
 
So for instance, to run Dedox with a Levenshtein limit of 4 on all C# files included in the Fab.csproj project and output affected files to D:\out, you would run this command:

Dedox.exe -o D:\out -l 4 fab.csproj
