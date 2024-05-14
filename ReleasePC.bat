chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish PC\Launch\CandySugar.MainUI\CandySugar.MainUI.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Launch\CandySugar.ModifyUI\CandySugar.ModifyUI.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false

set List=LightNovel Novel Music Movie WallPaper Bilibili Anime Rifan Comic Axgle Cosplay Manga
for %%a in (%List%) do (
dotnet publish PC\Component\CandySugar.%%a\CandySugar.%%a.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
) 

rd /S /Q PC\Launch\CandySugar.MainUI\obj PC\Launch\CandySugar.MainUI\bin\Release

rd /S /Q PC\Launch\CandySugar.ModifyUI\obj PC\Launch\CandySugar.ModifyUI\bin\Release

rd /S /Q PC\Launch\CandySugar.HostServer\obj PC\Launch\CandySugar.HostServer\bin\Release

rd /S /Q PC\Common\CandySugar.Com.Controls\obj PC\Common\CandySugar.Com.Controls\bin\Release

rd /S /Q PC\Common\CandySugar.Com.Library\obj PC\Common\CandySugar.Com.Library\bin\Release
		  
rd /S /Q PC\Common\CandySugar.Com.Options\obj PC\Common\CandySugar.Com.Options\bin\Release
		  
rd /S /Q PC\Common\CandySugar.Com.Style\obj PC\Common\CandySugar.Com.Style\bin\Release
		  
rd /S /Q PC\Common\CandySugar.Com.Data\obj PC\Common\CandySugar.Com.Data\bin\Release

for %%a in (%List%) do (
rd /S /Q PC\Component\CandySugar.%%a\obj PC\Component\CandySugar.%%a\bin\Release
)

xcopy PC\Launch\CandySugar.MainUI\bin\Debug\net8.0-windows\ffmpeg Release\ffmpeg /e /s
xcopy PC\Launch\CandySugar.MainUI\bin\Debug\net8.0-windows\vlclib Release\vlclib /e /s

cd Release
del *.pdb *.json 
ren CandySugar.MainUI.exe CandySugar.exe
ren CandySugar.ModifyUI.exe CandySugarModify.exe
@echo 已完成处理
pause
