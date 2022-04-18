rm -R .\release -ErrorAction SilentlyContinue

mkdir .\release | Out-Null

cp .\build\OriDeModLoader.dll .\release\
cp .\build\0Harmony.dll .\release\
cp .\winhttp.dll .\release\
cp .\doorstop_config.ini .\release\

rm .\OriDeModLoader.zip -ErrorAction SilentlyContinue
Compress-Archive .\release\* .\OriDeModLoader.zip