chcp 65001
@echo 开始自动化发布
cd /d %~dp0
dotnet publish NC\CandySugar.MainUI\CandySugar.MainUI.csproj -f net7.0-android -c Debug  -o ..\CandySugar\ReleaseApp -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=candy.keystore -p:AndroidSigningKeyAlias=candy -p:AndroidSigningKeyPass=123456 -p:AndroidSigningStorePass=123456
rd /S /Q NC\CandySugar.MainUI\obj NC\CandySugar.MainUI\bin\Release
rd /S /Q NC\CandySugar.Com.Service\obj NC\CandySugar.Com.Service\bin\Release
rd /S /Q NC\CandySugar.Com.Pages\obj NC\CandySugar.Com.Pages\bin\Release
rd /S /Q NC\CandySugar.Com.Library\obj NC\CandySugar.Com.Library\bin\Release
rd /S /Q NC\CandySugar.Com.Controls\obj NC\CandySugar.Com.Controls\bin\Release

@echo 已完成处理
pause