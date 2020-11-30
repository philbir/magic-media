
Copy-Item .\src\Directory.Build.props .\src\Identity\Directory.Build.props
Copy-Item .\src\Versions.props .\src\Identity\Versions.props
Copy-Item .\global.json .\src\Identity\global.json

docker build -t magic-media-identity:dev -f .\src\Identity\Identity.Host\Dockerfile .\src\Identity\

Remove-Item .\src\Identity\Directory.Build.props
Remove-Item .\src\Identity\Versions.props
Remove-Item .\src\Identity\global.json

docker tag magic-media-identity:dev skycontainers.azurecr.io/magic-media-identity:latest
docker push skycontainers.azurecr.io/magic-media-identity:latest

