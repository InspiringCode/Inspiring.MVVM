@ECHO OFF
ECHO Please make sure you run this file as Administrator!
ECHO.
ECHO Please close Visual Studio and press ENTER!
PAUSE

SET vs_dir=%ProgramFiles(x86)%\Microsoft Visual Studio 10.0
SET template_dir=%vs_dir%\Common7\IDE\ItemTemplates\CSharp
SET template_cache_dir=%vs_dir%\Common7\IDE\ItemTemplatesCache\CSharp

REM Change to the directory where the bat file is lcoated 
REM (imported if run as Administrator).

CD /D %~dp0

ERASE /F /Q  "%template_cache_dir%\1033\BasicUnitTest.zip\"
ERASE /F /Q  "%template_cache_dir%\Code\1033\Class.zip\"
ERASE /F /Q  "%template_cache_dir%\Code\1033\Interface.zip\"
ERASE /F /Q  "%template_cache_dir%\WPF\1033\WPFUserControl.zip\"
ERASE /F /Q  "%template_cache_dir%\WPF\1033\WPFWindow.zip\"

COPY /Y BasicUnitTest.zip "%template_dir%\1033\"
COPY /Y Class.zip "%template_dir%\Code\1033\"
COPY /Y Interface.zip "%template_dir%\Code\1033\"
COPY /Y WPFUserControl.zip "%template_dir%\WPF\1033\"
COPY /Y WPFWindow.zip "%template_dir%\WPF\1033\"

ECHO Please wait while Visual Studio updates its item template cache...
"%vs_dir%\Common7\IDE\devenv.exe" /InstallVSTemplates

ECHO.
ECHO Installation finished!
PAUSE