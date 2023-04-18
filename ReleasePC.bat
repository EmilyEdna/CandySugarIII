chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish PC\CandySugar.EntryUI\CandySugar.EntryUI.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64
dotnet publish PC\Component\CandySugar.LightNovel\CandySugar.LightNovel.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64
dotnet publish PC\Component\CandySugar.Music\CandySugar.Music.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64
dotnet publish PC\Component\CandySugar.WallPaper\CandySugar.WallPaper.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64
dotnet publish PC\Component\CandySugar.Bilibili\CandySugar.Bilibili.csproj -c Release -o ..\CandySugar\Release -f net7.0-windows --sc true -r win-x64

rd /S /Q PC\CandySugar.EntryUI\obj PC\CandySugar.EntryUI\bin\Release

rd /S /Q PC\CandySugar.Com.Controls\obj PC\CandySugar.Com.Controls\bin\Release

rd /S /Q PC\CandySugar.Com.Library\obj PC\CandySugar.Com.Library\bin\Release

rd /S /Q PC\CandySugar.Com.Options\obj PC\CandySugar.Com.Options\bin\Release

rd /S /Q PC\CandySugar.Com.Style\obj PC\CandySugar.Com.Style\bin\Release

rd /S /Q PC\Component\CandySugar.LightNovel\obj PC\Component\CandySugar.LightNovel\bin\Release

rd /S /Q PC\Component\CandySugar.Music\obj PC\Component\CandySugar.Music\bin\Release

rd /S /Q PC\Component\CandySugar.WallPaper\obj PC\Component\CandySugar.WallPaper\bin\Release

rd /S /Q PC\Component\CandySugar.Bilibili\obj PC\Component\CandySugar.Bilibili\bin\Release

xcopy PC\CandySugar.EntryUI\bin\Debug\net7.0-windows\ffmpeg Release\ffmpeg /e /s

cd Release
del *.pdb *.json 
ren CandySugar.EntryUI.exe CandySugar.exe
@echo 已完成处理
pause
