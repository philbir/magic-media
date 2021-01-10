# Magic Media

Magic media is a photo library to mange all your family photos and videos in a central place.
It uses features like face detection/recognition, object detection, geo encoding and metadata extraction to make your media accessible and share it with your friends and family.

It is build with privacy in mind, you can enjoy features you know from tools like Google Photos but without giving all you precious data away.

> Project state: alpha

## Demo
A demo of the running application can be found here:
[https://demo.magic-media.io](https://demo.magic-media.io)

## Overview
![Magic Media](https://github.com/philbir/magic-media/blob/main/images/ui_screenshot.jpg?raw=true)

## Features

- [x] Rich Filtering (Persons, Geo, Date, Objects, Tags)
- [x] Local face detection / prediction
- [x] Manually manage persons and faces
- [ ] Face clustering
- Object detection / photo tagging
    - [x] Using local ImageAI
    - [x] Using Azure Computer vision
    - [ ] Using Google Vision AI
- Geo Decoding
    - [x] Using Bing maps api
    - [ ] Using Google maps api
- [x] Albums
  - [x] Share with friends and family
  - [ ] Sync with Google Photos
- [x] Duplicate detection
- [x] Maps integration
- [x] Person timeline
- [x] PWA Support
- Media discovery sources
  - [x] Local File System
  - [ ] Google Drive
  - [ ] OneDrive
  - [ ] Dropbox
- Media Strores
  - [x] Local File System
  - [ ] Azure Storage
- Identity providers
  - [x] Google
  - [x] GitHub
  - [x] Microsoft Account

## Starting the the projects

### UI project

The UI needs a running API running on http://localhost:5000, first start the  `API.Host` project and then
you can start the UI in `src/UI/media-ui` using `yarn serve`

You can also use the preview api withhout having to run an API localy, just use:
`yarn serve --mode preview`

### Face detection

To work on the face detection project the most convinient way is to use [VS Code Remote Containers](https://code.visualstudio.com/docs/remote/containers-tutorial)

Then just open the `src/Face` directory in a remote container.

## Architecture

![Magic Media](https://github.com/philbir/magic-media/blob/main/images/architecture.jpg?raw=true)

## Installation

The application is composed of 5 microservices, the easiest way to install the application is Docker.

- Host -> .NET Core container with GraphQL API und UI
- Worker -> .NET Core container for background jobs
- Identity -> .NET Core with Duende IdentityServer
- Face -> Python container for face recognizion (Optional)
- ImageAI -> Python container for object detection and tagging (Optional)

> Docker containers will be availlable very soon on Docker Hub.

### Prerequisite

- MongoDB
- RabbitMQ
- Google Maps Key
- OAuth client registration(s)
- Bing Maps API Key (Optional)
- Azure Computer Vision API Key (Optional)

### Install using docker-compose

- TODO: Provide `docker-compose` sample an manual
- TODO: Install on a Raspberry Pi
- TODO: Install on a Synology NAS

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
