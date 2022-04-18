cp build\OriDeModLoader.dll package\lib\net35\
cp build\Assembly-CSharp.dll package\lib\net35\
cp build\UnityEngine.dll package\lib\net35\
nuget pack package -OutputDirectory pkgbuild
