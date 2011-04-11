@ECHO OFF
ECHO Please make sure you run this file as Administrator!
PAUSE

SET vs_dir=%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\VC#\Snippets\1033
SET contracts_dir=%ProgramFiles(x86)%\Microsoft\Contracts\Languages\CSharp\Code Contract Snippets
SET my_dir=%USERPROFILE%\Documents\Visual Studio 2010\Code Snippets\Visual C#\My Code Snippets

REM Change to the directory where the bat file is lcoated 
REM (imported if run as Administrator).

CD /D %~dp0

COPY /Y propa.snippet "%vs_dir%\NetFX30"
COPY /Y propdp.snippet "%vs_dir%\NetFX30"
COPY /Y exception.snippet "%vs_dir%\Visual C#"

COPY /Y contractrequiresalwaysnotnull.snippet "%contracts_dir%"

COPY /Y "curlybraces.snippet" "%my_dir%"

ECHO.
ECHO Installation finished!
PAUSE