chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish -p:JavaSdkDirectory="D:\Soft\Microsoft Visual Studio\Shared\Android\openjdk\jdk-21.0.8" -p:AndroidSdkDirectory="D:\Soft\Microsoft Visual Studio\Shared\Android\android-sdk" /p:DebugType=None /p:DebugSymbols=false App_Previous_2\CandySugar.MainUI\CandySugar.MainUI.csproj -f net10.0-android -c Release  -o ..\CandySugar\ReleaseApp -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=candy.keystore -p:AndroidSigningKeyAlias=candy -p:AndroidSigningKeyPass=123456 -p:AndroidSigningStorePass=123456
rd /S /Q App_Previous_2\CandySugar.MainUI\obj App_Previous_2\CandySugar.MainUI\bin\Release
rd /S /Q App_Previous_2\CandySugar.Com.Service\obj App_Previous_2\CandySugar.Com.Service\bin\Release
rd /S /Q App_Previous_2\CandySugar.Com.Pages\obj App_Previous_2\CandySugar.Com.Pages\bin\Release
rd /S /Q App_Previous_2\CandySugar.Com.Library\obj App_Previous_2\CandySugar.Com.Library\bin\Release
::rd /S /Q App_Previous_2\CandySugar.Com.Controls\obj App_Previous_2\CandySugar.Com.Controls\bin\Release

@echo 已完成处理
pause