chcp 65001
echo Start Build
if "%1"=="Debug" (
  echo Plugin Build
  (for %%a in (Music,Bilibili) do (
	echo Begin Build %%a
	dotnet build ../../Component/CandySugar.%%a/CandySugar.%%a.csproj --no-dependencies  -c %1
  ))
  (for %%b in (WallPaper,Axgle,Cosplay) do (
	echo Begin Build %%b
	dotnet build ../../Component/CandySugar.%%b/CandySugar.%%b.csproj --no-dependencies  -c %1
  ))
  rem dotnet build ../CandySugar.ModifyUI/CandySugar.ModifyUI.csproj --no-dependencies  -c %1
  echo Build Complete
  call PluginComplete.bat
)
