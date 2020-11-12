
yarn --cwd .\src\UI\media-ui build

Copy-Item .\src\UI\media-ui\dist\* .\src\services\Api.Host\wwwroot\ -Recurse -Force

Copy-Item .\src\Directory.Build.props .\src\Services\Directory.Build.props
Copy-Item .\src\Versions.props .\src\Services\Versions.props
Copy-Item .\global.json .\src\Services\global.json

docker build -t magic-media-host:dev -f .\src\services\Api.Host\Dockerfile .\src\Services\

docker tag magic-media-host:dev skycontainers.azurecr.io/magic-media-host:latest
docker push skycontainers.azurecr.io/magic-media-host:latest

Remove-Item .\src\Services\Directory.Build.props
Remove-Item .\src\Services\Versions.props
Remove-Item .\src\Services\global.json

#docker build -t magic-media-face:dev -f  .

#docker tag magic-media-face:dev skycontainers.azurecr.io/magic-media-face:v1
#docker push skycontainers.azurecr.io/magic-media-face:v1
