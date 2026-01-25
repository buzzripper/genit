
set SCRIPT_DIR=%~dp0

pwsh -ExecutionPolicy Bypass -File  "%SCRIPT_DIR%\GmdlUpdate.ps1" -FilePath "D:\Code\buzzripper\app1\Common\Data\Models\app1.gmdl"