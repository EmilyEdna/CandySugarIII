chcp 65001
cd %~dp0
set dir=%~dp0
set exe=CandySugar.exe
set param=%dir%%exe%
@echo 【当前路劲】*******【%param%】
sc create CandySugar binpath="%param%" start=auto & sc start Candysugar
pause
