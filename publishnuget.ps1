param([Parameter(Mandatory)] [string] $ApiKey)

mkdir .\package\lib\net35 -ErrorAction SilentlyContinue | Out-Null

cp build\OriDeModLoader.dll package\lib\net35\
cp build\Assembly-CSharp.dll package\lib\net35\
cp build\UnityEngine.dll package\lib\net35\
nuget pack package -OutputDirectory pkgbuild

nuget push pkgbuild\*.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey $ApiKey
