chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish PC\CandySugar.MainUI\CandySugar.MainUI.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\CandySugar.ModifyUI\CandySugar.ModifyUI.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Component\CandySugar.LightNovel\CandySugar.LightNovel.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Component\CandySugar.Novel\CandySugar.Novel.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Component\CandySugar.Music\CandySugar.Music.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Component\CandySugar.WallPaper\CandySugar.WallPaper.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Component\CandySugar.Bilibili\CandySugar.Bilibili.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Component\CandySugar.Anime\CandySugar.Anime.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Component\CandySugar.Rifan\CandySugar.Rifan.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Component\CandySugar.Comic\CandySugar.Comic.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false

rd /S /Q PC\CandySugar.MainUI\obj PC\CandySugar.MainUI\bin\Release

rd /S /Q PC\CandySugar.ModifyUI\obj PC\CandySugar.ModifyUI\bin\Release

rd /S /Q PC\CandySugar.Com.Controls\obj PC\CandySugar.Com.Controls\bin\Release

rd /S /Q PC\CandySugar.Com.Library\obj PC\CandySugar.Com.Library\bin\Release

rd /S /Q PC\CandySugar.Com.Options\obj PC\CandySugar.Com.Options\bin\Release

rd /S /Q PC\CandySugar.Com.Style\obj PC\CandySugar.Com.Style\bin\Release

rd /S /Q PC\Component\CandySugar.LightNovel\obj PC\Component\CandySugar.LightNovel\bin\Release

rd /S /Q PC\Component\CandySugar.Novel\obj PC\Component\CandySugar.Novel\bin\Release

rd /S /Q PC\Component\CandySugar.Music\obj PC\Component\CandySugar.Music\bin\Release

rd /S /Q PC\Component\CandySugar.WallPaper\obj PC\Component\CandySugar.WallPaper\bin\Release

rd /S /Q PC\Component\CandySugar.Bilibili\obj PC\Component\CandySugar.Bilibili\bin\Release

rd /S /Q PC\Component\CandySugar.Anime\obj PC\Component\CandySugar.Anime\bin\Release

rd /S /Q PC\Component\CandySugar.Comic\obj PC\Component\CandySugar.Comic\bin\Release

rd /S /Q PC\Component\CandySugar.Rifan\obj PC\Component\CandySugar.Rifan\bin\Release

xcopy PC\CandySugar.MainUI\bin\Debug\net7.0-windows\ffmpeg Release\ffmpeg /e /s
xcopy PC\CandySugar.MainUI\bin\Debug\net7.0-windows\vlclib Release\vlclib /e /s

cd Release
del *.pdb *.json 
ren CandySugar.MainUI.exe CandySugar.exe
ren CandySugar.ModifyUI.exe CandySugarModify.exe
@echo 已完成处理
pause
