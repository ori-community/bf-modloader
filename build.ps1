rm -R .\build -ErrorAction SilentlyContinue
rm -R .\release -ErrorAction SilentlyContinue

msbuild /t:Build /restore /p:Configuration=Release /p:OutDir=..\build
dotnet publish Injector/Injector.csproj -c Release -o release

cp .\build\OriDeModLoader.dll .\release\
cp .\build\0Harmony.dll .\release\

rm .\OriDeModLoader.zip -ErrorAction SilentlyContinue
Compress-Archive .\release\* .\OriDeModLoader.zip
