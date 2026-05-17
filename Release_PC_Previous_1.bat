chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish PC_Previous_1\Launch\CandySugar.MainUI\CandySugar.MainUI.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC_Previous_1\Launch\CandySugar.ModifyUI\CandySugar.ModifyUI.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false

set List=LightNovel Novel Music Movie WallPaper Bilibili Anime Rifan Comic Axgle Cosplay Manga NHViewer
for %%a in (%List%) do (
dotnet publish PC_Previous_1\Component\CandySugar.%%a\CandySugar.%%a.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
) 

rd /S /Q PC_Previous_1\Launch\CandySugar.MainUI\obj PC_Previous_1\Launch\CandySugar.MainUI\bin\Release

rd /S /Q PC_Previous_1\Launch\CandySugar.ModifyUI\obj PC_Previous_1\Launch\CandySugar.ModifyUI\bin\Release

rd /S /Q PC_Previous_1\Launch\CandySugar.HostServer\obj PC_Previous_1\Launch\CandySugar.HostServer\bin\Release

rd /S /Q PC_Previous_1\Common\CandySugar.Com.Controls\obj PC_Previous_1\Common\CandySugar.Com.Controls\bin\Release

rd /S /Q PC_Previous_1\Common\CandySugar.Com.Library\obj PC_Previous_1\Common\CandySugar.Com.Library\bin\Release
		  
rd /S /Q PC_Previous_1\Common\CandySugar.Com.Options\obj PC_Previous_1\Common\CandySugar.Com.Options\bin\Release
		  
rd /S /Q PC_Previous_1\Common\CandySugar.Com.Style\obj PC_Previous_1\Common\CandySugar.Com.Style\bin\Release
		  
rd /S /Q PC_Previous_1\Common\CandySugar.Com.Data\obj PC_Previous_1\Common\CandySugar.Com.Data\bin\Release

for %%a in (%List%) do (
rd /S /Q PC_Previous_1\Component\CandySugar.%%a\obj PC_Previous_1\Component\CandySugar.%%a\bin\Release
)

xcopy PC_Previous_1\Launch\CandySugar.MainUI\bin\Debug\net8.0-windows\ffmpeg Release\ffmpeg /e /s
xcopy PC_Previous_1\Launch\CandySugar.MainUI\bin\Debug\net8.0-windows\vlclib Release\vlclib /e /s

cd Release
del *.pdb *.json 
ren CandySugar.MainUI.exe CandySugar.exe
ren CandySugar.ModifyUI.exe CandySugarModify.exe
@echo 已完成处理
pause
