echo $env:PATH

function Build()
{
    Push-Location .\src
    try {
        dotnet build
    } 
    finally {
        Pop-Location
    }
}

function Publish()
{
    Push-Location .\src
    try {
        if (Test-Path "..\publish") 
        {
            Remove-Item "..\publish" -Force -Recurse
        }

        dotnet publish -o ..\publish
    } 
    finally {
        Pop-Location
    }
}

function Deploy()
{
    $targetPath = $env:DEPLOY_TARGET_PATH
    if (!$targetPath) {
        throw "Missing DEPLOY_TARGET_PATH"
    }
    
    $appPool = $env:DEPLOY_APP_POOL
 
    if (!(Test-Path -Path $targetPath))
    {
        mkdir $targetPath
    }

    # Take site offline
    echo "Copying app_offline.htm"
    Copy-Item -Path ".\deploy\app_offline.htm" -Destination "$targetPath" -Force
    
    if ($appPool) {
        echo "Restarting App Pool $appPool"
        Restart-WebAppPool -Name "$appPool"
    }
    
    echo "Waiting"
    Start-Sleep -Seconds 2

    # Replace app
    echo "Removing previous version"
    Remove-Item -Path "$targetPath\*.*" -Exclude "*.local.json","app_offline.htm" -Recurse -Force
    
    echo "Deploying new version"
    Copy-Item ".\publish\*" -Exclude "*.local.json" $targetPath -Recurse -Force 

    # Take site online
    echo "Removing app_offline.htm"
    Remove-Item -Path "$targetPath\app_offline.htm"
}
