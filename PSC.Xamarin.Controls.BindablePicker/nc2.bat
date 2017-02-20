@ECHO OFF
ECHO Upload Package for NuGet
ECHO Use as parameter the project name (*.nupkg)
nuget.exe push %1 19965959-addd-4d82-bf90-36fe4db22ead -Source https://www.nuget.org/api/v2/package