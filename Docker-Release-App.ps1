$env:DOCKER_BUILDKIT = 1 
yarn --cwd .\src\UI\media-ui build

New-Item -ItemType "directory" -Path .\src\services\Api.Host\wwwroot\
Copy-Item .\src\UI\media-ui\dist\* .\src\services\Api.Host\wwwroot\ -Recurse -Force

docker build -t magic-media-host:dev -f .\.docker\api.Dockerfile .

docker tag magic-media-host:dev skycontainers.azurecr.io/magic-media-host:latest
docker push skycontainers.azurecr.io/magic-media-host:latest

Remove-Item .\src\services\Api.Host\wwwroot\ -Recurse -Force

#docker build -t magic-media-face:dev -f  .

#docker tag magic-media-face:dev skycontainers.azurecr.io/magic-media-face:v1
#docker push skycontainers.azurecr.io/magic-media-face:v1
