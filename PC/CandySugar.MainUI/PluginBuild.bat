chcp 65001
echo Start Build
if "%1"=="Debug" (
  echo Plugin Build
  (for %%a in (LightNovel,Novel,Movie,Music,Bilibili,Manga) do (
	echo Begin Build %%a
	dotnet build ../Component/CandySugar.%%a/CandySugar.%%a.csproj --no-dependencies  -c %1
  ))
  (for %%b in (WallPaper,Anime,Rifan,Comic,Axgle Cosplay) do (
	echo Begin Build %%b
	dotnet build ../Component/CandySugar.%%b/CandySugar.%%b.csproj --no-dependencies  -c %1
  ))
  dotnet build ../CandySugar.ModifyUI/CandySugar.ModifyUI.csproj --no-dependencies  -c %1
  echo Build Complete
)
