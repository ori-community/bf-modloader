xcopy build\OriDeModLoader.dll package\lib\net35\ /Y
xcopy build\Assembly-CSharp.dll package\lib\net35\ /Y
xcopy build\UnityEngine.dll package\lib\net35\ /Y
nuget pack package -OutputDirectory pkgbuild
