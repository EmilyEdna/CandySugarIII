chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish MC\CandySugar.MainUI\CandySugar.MainUI.csproj -f net7.0-android -c Release  -o ..\CandySugar\ReleaseApp -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=candy.keystore -p:AndroidSigningKeyAlias=candy -p:AndroidSigningKeyPass=123456 -p:AndroidSigningStorePass=123456

rd /S /Q MC\CandySugar.MainUI\obj MC\CandySugar.MainUI\bin\Release
rd /S /Q MC\CandySugar.Com.Service\obj MC\CandySugar.Com.Service\bin\Release
rd /S /Q MC\CandySugar.Com.Pages\obj MC\CandySugar.Com.Pages\bin\Release
rd /S /Q MC\CandySugar.Com.Library\obj MC\CandySugar.Com.Library\bin\Release
rd /S /Q MC\CandySugar.Com.Controls\obj MC\CandySugar.Com.Controls\bin\Release

@echo 已完成处理
pause