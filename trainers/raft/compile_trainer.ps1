$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$managed = "D:\SteamLibrary\steamapps\common\Raft\Raft_Data\Managed"

$refs = @(
    "$managed\netstandard.dll",
    "$managed\UnityEngine.dll",
    "$managed\UnityEngine.CoreModule.dll",
    "$managed\UnityEngine.IMGUIModule.dll",
    "$managed\UnityEngine.InputLegacyModule.dll",
    "$managed\UnityEngine.PhysicsModule.dll",
    "$managed\UnityEngine.UI.dll",
    "$managed\Assembly-CSharp.dll"
) -join ","

& $csc /target:library /out:"c:\Users\Admin\Downloads\raft\RaftTrainer.dll" /reference:$refs "c:\Users\Admin\Downloads\raft\RaftTrainer.cs"
Write-Host "ExitCode: $LASTEXITCODE"
