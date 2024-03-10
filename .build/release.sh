export DOCKER_DEFAULT_PLATFORM=linux/amd64
export TAG=0.0.1

if [ "$1" != "skip-ui" ]; then
    yarn --cwd ./src/UI/media-ui build
    cp -R ./src/UI/media-ui/dist/ ./src/Services/Api.Host/wwwroot/
fi

dotnet publish  src/Services/Api.Host/Api.Host.csproj --os linux --arch x64 /t:PublishContainer -c Release -p:ContainerImageTag=$TAG
dotnet publish  src/Services/Worker/Worker.csproj --os linux --arch x64 /t:PublishContainer -c Release -p:ContainerImageTag=$TAG
