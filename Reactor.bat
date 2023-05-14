"D:\Soft\.NET Reactor\dotNET_Reactor.Console.exe" -satellite_assemblies "D:\Project\CandySugar\Release\CandySugar.Comic.dll;D:\Project\CandySugar\Release\CandySugar.Rifan.dll;D:\Project\CandySugar\Release\CandySugar.Bilibili.dll;D:\Project\CandySugar\Release\CandySugar.Com.Controls.dll;D:\Project\CandySugar\Release\CandySugar.Com.Library.dll;D:\Project\CandySugar\Release\CandySugar.Com.Options.dll;D:\Project\CandySugar\Release\CandySugar.Com.Style.dll;D:\Project\CandySugar\Release\CandySugar.MainUI.dll;D:\Project\CandySugar\Release\CandySugar.ModifyUI.dll;D:\Project\CandySugar\Release\CandySugar.LightNovel.dll;D:\Project\CandySugar\Release\CandySugar.Music.dll;D:\Project\CandySugar\Release\CandySugar.WallPaper.dll;D:\Project\CandySugar\Release\Sdk.Component.dll;D:\Project\CandySugar\Release\Sdk.Component.Vip.dll;D:\Project\CandySugar\Release\Sdk.Plugins.dll" -antitamp 1 -anti_debug 1 -hide_calls 1 -stringencryption 0 -virtualization 1 -necrobit 1 -obfuscation 0 control_flow_obfuscation 1 -flow_level 9

chcp 65001
@echo 开始覆盖
xcopy Release\CandySugar.ModifyUI_Secure\CandySugar.ModifyUI.dll Release /e /s /y
xcopy Release\CandySugar.Com.Controls_Secure\CandySugar.Com.Controls.dll Release /e /s /y
xcopy Release\CandySugar.Com.Library_Secure\CandySugar.Com.Library.dll Release /e /s /y
xcopy Release\CandySugar.Com.Options_Secure\CandySugar.Com.Options.dll Release /e /s /y
xcopy Release\CandySugar.Com.Style_Secure\CandySugar.Com.Style.dll Release /e /s /y
xcopy Release\CandySugar.MainUI_Secure\CandySugar.MainUI.dll Release /e /s /y
xcopy Release\CandySugar.LightNovel_Secure\CandySugar.LightNovel.dll Release /e /s /y
xcopy Release\CandySugar.Music_Secure\CandySugar.Music.dll Release /e /s /y
xcopy Release\CandySugar.WallPaper_Secure\CandySugar.WallPaper.dll Release /e /s /y
xcopy Release\CandySugar.Bilibili_Secure\CandySugar.Bilibili.dll Release /e /s /y
xcopy Release\CandySugar.Rifan_Secure\CandySugar.Rifan.dll Release /e /s /y
xcopy Release\CandySugar.Comic_Secure\CandySugar.Comic.dll Release /e /s /y
xcopy Release\Sdk.Component_Secure\Sdk.Component.dll Release /e /s /y
xcopy Release\Sdk.Component.Vip_Secure\Sdk.Component.Vip.dll Release /e /s /y
xcopy Release\Sdk.Plugins_Secure\Sdk.Plugins.dll Release /e /s /y
@echo 覆盖完成
@echo 执行删除
rd /S /Q Release\CandySugar.Com.Controls_Secure
rd /S /Q Release\CandySugar.Com.Library_Secure
rd /S /Q Release\CandySugar.Com.Options_Secure
rd /S /Q Release\CandySugar.Com.Style_Secure
rd /S /Q Release\CandySugar.ModifyUI_Secure
rd /S /Q Release\CandySugar.MainUI_Secure
rd /S /Q Release\CandySugar.LightNovel_Secure
rd /S /Q Release\CandySugar.Music_Secure
rd /S /Q Release\CandySugar.WallPaper_Secure
rd /S /Q Release\CandySugar.Bilibili_Secure
rd /S /Q Release\CandySugar.Comic_Secure
rd /S /Q Release\CandySugar.Rifan_Secure
rd /S /Q Release\Sdk.Component_Secure
rd /S /Q Release\Sdk.Component.Vip_Secure
rd /S /Q Release\Sdk.Plugins_Secure
@echo 删除完成
pause