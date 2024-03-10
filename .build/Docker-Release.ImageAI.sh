$env:DOCKER_BUILDKIT = 1

docker build -t magic-media-imageai:dev -f .\src\ImageAI\Dockerfile .\src\ImageAI\
docker tag magic-media-imageai:dev skycontainers.azurecr.io/magic-media-imageai:latest
docker push skycontainers.azurecr.io/magic-media-imageai:latest
