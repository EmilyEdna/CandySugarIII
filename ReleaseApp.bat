chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish /p:JavaSdkDirectory="D:\Android\openjdk\jdk-17.0.8.101-hotspot" /p:AndroidSdkDirectory="D:\Android\android-sdk" /p:DebugType=None /p:DebugSymbols=false App\CandySugar.MainUI\CandySugar.MainUI.csproj -f net8.0-android -c Release  -o ..\CandySugar\ReleaseApp -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=candy.keystore -p:AndroidSigningKeyAlias=candy -p:AndroidSigningKeyPass=123456 -p:AndroidSigningStorePass=123456
rd /S /Q App\CandySugar.MainUI\obj App\CandySugar.MainUI\bin\Release
rd /S /Q App\CandySugar.Com.Service\obj App\CandySugar.Com.Service\bin\Release
rd /S /Q App\CandySugar.Com.Pages\obj App\CandySugar.Com.Pages\bin\Release
rd /S /Q App\CandySugar.Com.Library\obj App\CandySugar.Com.Library\bin\Release
::rd /S /Q App\CandySugar.Com.Controls\obj App\CandySugar.Com.Controls\bin\Release

@echo 已完成处理
pause