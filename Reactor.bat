"D:\Soft\.NET Reactor\dotNET_Reactor.Console.exe" -file "D:\Project\CandySugar\Release\CandySugar.MainUI.dll" -files "D:\Project\CandySugar\Release\CandySugar.Com.Controls.dll;D:\Project\CandySugar\Release\CandySugar.Com.Data.dll;D:\Project\CandySugar\Release\CandySugar.Com.Library.dll;D:\Project\CandySugar\Release\CandySugar.Com.Options.dll;D:\Project\CandySugar\Release\CandySugar.Com.Style.dll;D:\Project\CandySugar\Release\CandySugar.HostServer.dll;D:\Project\CandySugar\Release\CandySugar.MainUI.dll;D:\Project\CandySugar\Release\CandySugar.ModifyUI.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Anime.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Axgle.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Bilibili.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Comic.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Cosplay.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.LightNovel.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Manga.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Movie.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Music.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.NHViewer.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Novel.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.Rifan.dll;D:\Project\CandySugar\Release\Plugin\CandySugar.WallPaper.dll;D:\Project\CandySugar\Release\Plugin\Sdk.Component.dll;D:\Project\CandySugar\Release\Plugin\Sdk.Component.Vip.dll;D:\Project\CandySugar\Release\Plugin\Sdk.Plugins.dll" -antitamp 1 -anti_debug 1 -hide_calls 1 -control_flow 1 -flow_level 9 -stringencryption 0 -virtualization 1 -necrobit 1 -obfuscation 0



chcp 65001
@echo 开始覆盖
xcopy Release\CandySugar.Com.Controls_Secure\CandySugar.Com.Controls.dll Release /e /s /y
xcopy Release\CandySugar.Com.Library_Secure\CandySugar.Com.Library.dll Release /e /s /y
xcopy Release\CandySugar.Com.Options_Secure\CandySugar.Com.Options.dll Release /e /s /y
xcopy Release\CandySugar.Com.Style_Secure\CandySugar.Com.Style.dll Release /e /s /y
xcopy Release\CandySugar.Com.Data_Secure\CandySugar.Com.Data.dll Release /e /s /y
xcopy Release\CandySugar.HostServer_Secure\CandySugar.HostServer.dll Release /e /s /y
xcopy Release\Plugin\Sdk.Component_Secure\Sdk.Component.dll Release\Plugin /e /s /y
xcopy Release\Plugin\Sdk.Component.Vip_Secure\Sdk.Component.Vip.dll Release\Plugin /e /s /y
xcopy Release\Plugin\Sdk.Plugins_Secure\Sdk.Plugins.dll Release\Plugin /e /s /y
xcopy Release\Sdk.Plugins_Secure\Sdk.Plugins.dll Release /e /s /y
xcopy Release\CandySugar.ModifyUI_Secure\CandySugar.ModifyUI.dll Release /e /s /y
xcopy Release\CandySugar.MainUI_Secure\CandySugar.MainUI.dll Release /e /s /y
set List=LightNovel Novel Music Movie WallPaper Bilibili Anime Rifan Comic Axgle Cosplay Manga NHViewer
for %%a in (%List%) do (
 xcopy Release\Plugin\CandySugar.%%a_Secure\CandySugar.%%a.dll Release\Plugin /e /s /y
)
@echo 覆盖完成
@echo 执行删除
rd /S /Q Release\CandySugar.MainUI_Secure
rd /S /Q Release\CandySugar.ModifyUI_Secure
rd /S /Q Release\CandySugar.Com.Controls_Secure
rd /S /Q Release\CandySugar.Com.Library_Secure
rd /S /Q Release\CandySugar.Com.Options_Secure
rd /S /Q Release\CandySugar.Com.Style_Secure
rd /S /Q Release\CandySugar.Com.Data_Secure
rd /S /Q Release\CandySugar.HostServer_Secure
rd /S /Q Release\Plugin\Sdk.Component_Secure
rd /S /Q Release\Plugin\Sdk.Component.Vip_Secure
rd /S /Q Release\Plugin\Sdk.Plugins_Secure
rd /S /Q Release\Sdk.Plugins_Secure
for %%a in (%List%) do (
 rd /S /Q Release\Plugin\CandySugar.%%a_Secure
)
@echo 删除完成
pause