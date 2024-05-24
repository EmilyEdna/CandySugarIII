"D:\Soft\.NET Reactor\dotNET_Reactor.Console.exe" -files "D:\Project\CandySugar\Release\CandySugar.Anime.dll;D:\Project\CandySugar\Release\CandySugar.Axgle.dll;D:\Project\CandySugar\Release\CandySugar.Bilibili.dll;D:\Project\CandySugar\Release\CandySugar.Com.Controls.dll;D:\Project\CandySugar\Release\CandySugar.Com.Data.dll;D:\Project\CandySugar\Release\CandySugar.Com.Library.dll;D:\Project\CandySugar\Release\CandySugar.Com.Options.dll;D:\Project\CandySugar\Release\CandySugar.Com.Style.dll;D:\Project\CandySugar\Release\CandySugar.Comic.dll;D:\Project\CandySugar\Release\CandySugar.Cosplay.dll;D:\Project\CandySugar\Release\CandySugar.HostServer.dll;D:\Project\CandySugar\Release\CandySugar.LightNovel.dll;D:\Project\CandySugar\Release\CandySugar.MainUI.dll;D:\Project\CandySugar\Release\CandySugar.Manga.dll;D:\Project\CandySugar\Release\CandySugar.ModifyUI.dll;D:\Project\CandySugar\Release\CandySugar.Movie.dll;D:\Project\CandySugar\Release\CandySugar.Music.dll;D:\Project\CandySugar\Release\CandySugar.Novel.dll;D:\Project\CandySugar\Release\CandySugar.NHViewer.dll;D:\Project\CandySugar\Release\CandySugar.Rifan.dll;D:\Project\CandySugar\Release\CandySugar.WallPaper.dll;D:\Project\CandySugar\Release\Sdk.Component.dll;D:\Project\CandySugar\Release\Sdk.Component.Vip.dll;D:\Project\CandySugar\Release\Sdk.Plugins.dll" -antitamp 1 -anti_debug 1 -hide_calls 1 -control_flow 1 -flow_level 9 -stringencryption 0 -virtualization 1 -necrobit 1 -obfuscation 0

chcp 65001
@echo 开始覆盖
xcopy Release\CandySugar.Com.Controls_Secure\CandySugar.Com.Controls.dll Release /e /s /y
xcopy Release\CandySugar.Com.Library_Secure\CandySugar.Com.Library.dll Release /e /s /y
xcopy Release\CandySugar.Com.Options_Secure\CandySugar.Com.Options.dll Release /e /s /y
xcopy Release\CandySugar.Com.Style_Secure\CandySugar.Com.Style.dll Release /e /s /y
xcopy Release\CandySugar.Com.Data_Secure\CandySugar.Com.Data.dll Release /e /s /y
xcopy Release\CandySugar.HostServer_Secure\CandySugar.HostServer.dll Release /e /s /y
xcopy Release\Sdk.Component_Secure\Sdk.Component.dll Release /e /s /y
xcopy Release\Sdk.Component.Vip_Secure\Sdk.Component.Vip.dll Release /e /s /y
xcopy Release\Sdk.Plugins_Secure\Sdk.Plugins.dll Release /e /s /y
set List=MainUI ModifyUI LightNovel Novel Music Movie WallPaper Bilibili Anime Rifan Comic Axgle Cosplay Manga NHViewer
for %%a in (%List%) do (
 xcopy Release\CandySugar.%%a_Secure\CandySugar.%%a.dll Release /e /s /y
)
@echo 覆盖完成
@echo 执行删除
rd /S /Q Release\CandySugar.Com.Controls_Secure
rd /S /Q Release\CandySugar.Com.Library_Secure
rd /S /Q Release\CandySugar.Com.Options_Secure
rd /S /Q Release\CandySugar.Com.Style_Secure
rd /S /Q Release\CandySugar.Com.Data_Secure
rd /S /Q Release\CandySugar.Com.HostServer_Secure
rd /S /Q Release\Sdk.Component_Secure
rd /S /Q Release\Sdk.Component.Vip_Secure
rd /S /Q Release\Sdk.Plugins_Secure
for %%a in (%List%) do (
 rd /S /Q Release\CandySugar.%%a_Secure
)
@echo 删除完成
pause