rm -R .\build -ErrorAction SilentlyContinue
rm -R .\release -ErrorAction SilentlyContinue

msbuild /t:Build /restore /p:Configuration=Release /p:OutDir=..\build
dotnet publish Injector/Injector.csproj -c Release -o release

cp .\build\OriDeModLoader.dll .\release\
cp .\build\0Harmony.dll .\release\
mkdir .\release\Mods | Out-Null
echo "Add mod files (*.dll) here" | Out-File .\release\Mods\_readme.txt

rm .\OriDeModLoader.zip -ErrorAction SilentlyContinue
Compress-Archive .\release\* .\OriDeModLoader.zip
