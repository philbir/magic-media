$env:DOCKER_BUILDKIT = 1 
docker build -t magic-media-identity:dev -f .\.docker\identity.Dockerfile .

docker tag magic-media-identity:dev skycontainers.azurecr.io/magic-media-identity:latest
docker push skycontainers.azurecr.io/magic-media-identity:latest

