rmdir /Q /S .\release

xcopy .\build\OriDeModLoader.dll .\release\
xcopy .\build\0Harmony.dll .\release\
xcopy .\winhttp.dll .\release\
xcopy .\doorstop_config.ini .\release\

del .\OriDeModLoader.zip
powershell Compress-Archive .\release\* .\OriDeModLoader.zip
