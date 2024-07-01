chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish OldPC\Launch\CandySugar.MainUI\CandySugar.MainUI.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish OldPC\Launch\CandySugar.ModifyUI\CandySugar.ModifyUI.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false

set List=LightNovel Novel Music Movie WallPaper Bilibili Anime Rifan Comic Axgle Cosplay Manga NHViewer
for %%a in (%List%) do (
dotnet publish OldPC\Component\CandySugar.%%a\CandySugar.%%a.csproj -c Release -o ..\CandySugar\Release -f net8.0-windows --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
) 

rd /S /Q OldPC\Launch\CandySugar.MainUI\obj OldPC\Launch\CandySugar.MainUI\bin\Release

rd /S /Q OldPC\Launch\CandySugar.ModifyUI\obj OldPC\Launch\CandySugar.ModifyUI\bin\Release

rd /S /Q OldPC\Launch\CandySugar.HostServer\obj OldPC\Launch\CandySugar.HostServer\bin\Release

rd /S /Q OldPC\Common\CandySugar.Com.Controls\obj OldPC\Common\CandySugar.Com.Controls\bin\Release

rd /S /Q OldPC\Common\CandySugar.Com.Library\obj OldPC\Common\CandySugar.Com.Library\bin\Release
		  
rd /S /Q OldPC\Common\CandySugar.Com.Options\obj OldPC\Common\CandySugar.Com.Options\bin\Release
		  
rd /S /Q OldPC\Common\CandySugar.Com.Style\obj OldPC\Common\CandySugar.Com.Style\bin\Release
		  
rd /S /Q OldPC\Common\CandySugar.Com.Data\obj OldPC\Common\CandySugar.Com.Data\bin\Release

for %%a in (%List%) do (
rd /S /Q OldPC\Component\CandySugar.%%a\obj OldPC\Component\CandySugar.%%a\bin\Release
)

xcopy OldPC\Launch\CandySugar.MainUI\bin\Debug\net8.0-windows\ffmpeg Release\ffmpeg /e /s
xcopy OldPC\Launch\CandySugar.MainUI\bin\Debug\net8.0-windows\vlclib Release\vlclib /e /s

cd Release
del *.pdb *.json 
ren CandySugar.MainUI.exe CandySugar.exe
ren CandySugar.ModifyUI.exe CandySugarModify.exe
@echo 已完成处理
pause
