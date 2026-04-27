Write-Output "build: Build started"

Push-Location $PSScriptRoot

Write-Output "build: Tool versions follow"

dotnet --version
dotnet --list-sdks

if(Test-Path .\artifacts) {
	Write-Output "build: Cleaning ./artifacts"
	Remove-Item ./artifacts -Force -Recurse
}

& dotnet restore --no-cache

$dbp = [Xml] (Get-Content .\Directory.Build.props)
$versionPrefix = $dbp.Project.PropertyGroup.VersionPrefix

Write-Output "build: Package version prefix is $versionPrefix"

$branch = @{ $true = $env:CI_TARGET_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$NULL -ne $env:CI_TARGET_BRANCH];
$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:CI_BUILD_NUMBER, 10); $false = "local" }[$NULL -ne $env:CI_BUILD_NUMBER];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)) -replace '([^a-zA-Z0-9\-]*)', '')-$revision"}[$branch -eq "main" -and $revision -ne "local"]

Write-Output "build: Package version suffix is $suffix"

foreach ($src in Get-ChildItem src/*) {
    Push-Location $src

	Write-Output "build: Packaging project in $src"

    if ($suffix) {
        & dotnet pack -c Release -o ../../artifacts --version-suffix=$suffix
    } else {
        & dotnet pack -c Release -o ../../artifacts
    }
    if($LASTEXITCODE -ne 0) { throw "Packaging failed" }

    Pop-Location
}

Write-Output "build: Checking complete solution builds"
& dotnet build
if($LASTEXITCODE -ne 0) { throw "Solution build failed" }


foreach ($test in Get-ChildItem test/*.Tests) {
    Push-Location $test

	Write-Output "build: Testing project in $test"

    & dotnet test -c Release
    if($LASTEXITCODE -ne 0) { throw "Testing failed" }

    Pop-Location
}

Pop-Location

if ($env:NUGET_API_KEY) {
    # GitHub Actions will only supply this to branch builds and not PRs. We publish
    # builds from any branch this action targets (i.e. main and dev).
    
    Write-Output "build: Publishing NuGet packages"
    
    foreach ($nupkg in Get-ChildItem artifacts/*.nupkg) {
        & dotnet nuget push -k $env:NUGET_API_KEY -s https://api.nuget.org/v3/index.json "$nupkg"
        if($LASTEXITCODE -ne 0) { throw "Publishing failed" }
    }

    if (!($suffix)) {
        Write-Output "build: Creating release for version $versionPrefix"

        iex "gh release create v$versionPrefix --title v$versionPrefix --generate-notes $(get-item ./artifacts/*.nupkg) $(get-item ./artifacts/*.snupkg)"
    }
}
