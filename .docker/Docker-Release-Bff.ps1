export DOCKER_BUILDKIT=1 
yarn --cwd ./src/UI/media-ui build

cp -R ./src/UI/media-ui/dist/ ./src/services/Bff.Host/wwwroot/

docker build -t magic-media-bff:dev -f ./.docker/bff.Dockerfile .

docker tag magic-media-bff:dev skycontainers.azurecr.io/magic-media-bff:latest
docker push skycontainers.azurecr.io/magic-media-bff:latest

