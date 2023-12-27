chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish PC\CandySugar.MainUI\CandySugar.MainUI.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\CandySugar.ModifyUI\CandySugar.ModifyUI.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false

set List=LightNovel Novel Music Movie WallPaper Bilibili Anime Rifan Comic Axgle
for %%a in (%List%) do (
dotnet publish PC\Component\CandySugar.%%a\CandySugar.%%a.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
) 

rd /S /Q PC\CandySugar.MainUI\obj PC\CandySugar.MainUI\bin\Release

rd /S /Q PC\CandySugar.ModifyUI\obj PC\CandySugar.ModifyUI\bin\Release

rd /S /Q PC\CandySugar.Com.Controls\obj PC\CandySugar.Com.Controls\bin\Release

rd /S /Q PC\CandySugar.Com.Library\obj PC\CandySugar.Com.Library\bin\Release

rd /S /Q PC\CandySugar.Com.Options\obj PC\CandySugar.Com.Options\bin\Release

rd /S /Q PC\CandySugar.Com.Style\obj PC\CandySugar.Com.Style\bin\Release

for %%a in (%List%) do (
rd /S /Q PC\Component\CandySugar.%%a\obj PC\Component\CandySugar.%%a\bin\Release
)

xcopy PC\CandySugar.MainUI\bin\Debug\net8.0-windows\ffmpeg Release\ffmpeg /e /s
xcopy PC\CandySugar.MainUI\bin\Debug\net8.0-windows\vlclib Release\vlclib /e /s

cd Release
del *.pdb *.json 
ren CandySugar.MainUI.exe CandySugar.exe
ren CandySugar.ModifyUI.exe CandySugarModify.exe
@echo 已完成处理
pause
