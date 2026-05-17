chcp 65001
set dirs=%~dp0
set target=%dirs:BatchCmd\=%
set exe=CandySugar.exe
set param=%target%%exe%
@echo 【当前路劲】*******【%param%】
sc create CandySugar binpath="%param%" start=auto & sc start Candysugar
pause
