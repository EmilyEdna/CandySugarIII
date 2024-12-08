echo %CD%

cd bin\Debug\net9.0-windows7.0\Plugin

del *.json *.pdb *.xml

set Params=LightNovel,Novel,Movie,Music,Bilibili,Manga,WallPaper,Anime,Rifan,Comic,Axgle,Cosplay,NHViewer

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