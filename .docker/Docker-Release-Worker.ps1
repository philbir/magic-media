$env:DOCKER_BUILDKIT = 1

docker build -t magic-media-worker:dev -f .\.docker\worker.Dockerfile .

docker tag magic-media-worker:dev skycontainers.azurecr.io/magic-media-worker:latest
docker push skycontainers.azurecr.io/magic-media-worker:latest

