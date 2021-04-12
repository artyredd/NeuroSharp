robocopy ".\bin\Debug\net5.0" "C:\_Programming\Repos\Unity Projects\FlappyCloneML\Assets\Scripts\Libraries\NeuroSharp" /mir /z /xd ref
if ErrorLevel 8 (exit /B 1) else (exit /B 0)