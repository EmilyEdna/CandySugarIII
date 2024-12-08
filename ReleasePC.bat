chcp 65001

echo 开始自动化发布
cd /d %~dp0
dotnet publish PC\Launch\CandySugar.MainUI\CandySugar.MainUI.csproj -c Release -o ..\CandySugar\Release -f net9.0-windows7.0 --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
dotnet publish PC\Launch\CandySugar.ModifyUI\CandySugar.ModifyUI.csproj -c Release -o ..\CandySugar\Release -f net9.0-windows7.0 --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false

set List=LightNovel Novel Music Movie WallPaper Bilibili Anime Rifan Comic Axgle Cosplay Manga NHViewer
for %%a in (%List%) do (
dotnet publish PC\Component\CandySugar.%%a\CandySugar.%%a.csproj -c Release -o ..\CandySugar\Release\Plugin -f net9.0-windows7.0 --sc true -r win-x64 /p:DebugType=None /p:DebugSymbols=false
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

xcopy PC\Launch\CandySugar.MainUI\bin\Debug\net9.0-windows7.0\ffmpeg Release\ffmpeg /e /s
xcopy PC\Launch\CandySugar.MainUI\bin\Debug\net9.0-windows7.0\vlclib Release\vlclib /e /s

cd Release
del *.pdb *.json 
ren CandySugar.MainUI.exe CandySugar.exe
ren CandySugar.ModifyUI.exe CandySugarModify.exe

echo 已完成处理

echo 处理插件

cd Plugin

del *.json *.pdb *.xml *.exe

set Params=LightNovel,Novel,Movie,Music,Bilibili,Manga,WallPaper,Anime,Rifan,Comic,Axgle,Cosplay,NHViewer

echo %CD%

for /d %%x in (*) do (
	rmdir /s /q %%x
)


mkdir folder

for /r %%x in (*) do (	
	for %%y in (%Params%) do (
		 if /i "%%~nx" == "CandySugar.%%y" (
			 copy /y CandySugar.%%y.dll folder
		 ) 
		 if /i "%%~nx" == "Sdk.Component" (
			  copy /y Sdk.Component.dll folder
		 )
		 if /i "%%~nx" == "Sdk.Component.Vip" (
			  copy /y Sdk.Component.Vip.dll folder
		 )
		 if /i "%%~nx" == "Sdk.Plugins" (
			  copy /y Sdk.Plugins.dll folder
		 )
	)
)

for %%f in (*) do (
	echo !%%~xff! | findstr /i ".dll" >nul
	  if not errorlevel 1 (
        del %%f
    ) else (
        echo "NO"
    )
)

cd folder
copy /y Sdk.Plugins.dll ..
copy /y Sdk.Component.Vip.dll ..
copy /y Sdk.Component.dll ..

for %%x in (%Params%) do (
	copy /y CandySugar.%%x.dll ..
)

cd ..

rmdir /s /q folder

echo 插件处理完成

pause
