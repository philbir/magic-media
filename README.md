# Magic Media

Magic media is a photo library to mange all your family photos and videos in a central place.
It uses features like face detection/recognition, object detection, geo encoding and metadata extraction to make your media accessible.

It is build with privacy in mind, you can enjoy features you know from tools like Google Photos but without giving all you precious data away.

> Project state: is work in process

## Demo
A preview of the running application can be found here:
[https://magic-media-preview.birbaum.me](https://magic-media-preview.birbaum.me)

## Overview
![Magic Media](https://github.com/philbir/magic-media/blob/main/images/ui_screenshot.jpg?raw=true)

## Starting the the projects

### UI project

The UI needs a running API running on http://localhost:5000, first start `API.Host` project and then
you can start the UI in `src/UI/media-ui` using `yarn serve`

You can also use the preview api withhout having to run an API locally, just use:
`yarn serve --mode preview`

### Face detection

To work on the face detection project most convinient way is to use [Visual Studio Code Remote Containers](https://code.visualstudio.com/docs/remote/containers-tutorial)

Then just open `src/Face` in a remote container.

## Architecture

![Magic Media](https://github.com/philbir/magic-media/blob/main/images/architecture.jpg?raw=true)


## Open source

The project is build on top of some awsome opensource platforms and libraries.

- [Hot Chocolate GraphQL](https://github.com/ChilliCream/hotchocolate)
- [Face Recognition](https://github.com/ageitgey/face_recognition)
- [Image AI](https://github.com/OlafenwaMoses/ImageAI)
- [MongoDB](https://www.mongodb.com/)
- [Identity Server](https://identityserver.io/)
- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)
- [MassTransit](https://masstransit-project.com/)
- [Vue.js](https://vuejs.org/)
