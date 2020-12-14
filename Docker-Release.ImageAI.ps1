$env:DOCKER_BUILDKIT = 1

docker build -t magic-media-imageai:dev -f .\src\ImageAI\Dockerfile .\src\ImageAI\
