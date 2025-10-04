## Rework of the legacy project ZenithBeep-Legacy on the DSharp library

<div align="center" width="100%">
<img src="https://i.imgur.com/ovLUlWm.png" alt="logo" width="128" height="128" align="center">
</div>

The project is still in the early stages of development!

A dashboard is also being developed for this project.

# About

My personal Discord bot, which was rewritten from my old work under .NET-8
The project started on October 27, 2022. Under the old name LunaBot, especially for the IB workshop server

Briefly about the bot, at the moment it can create private voice channels, has a system for issuing roles and, of course, the bot has musical capabilities and can play music from YouTube in a voice channel.

Moderation features will be available in the future.

# Deploy via Docker

Edit the file `docker-compose.yml`
```
version: '3'

services:
  bot:
    build: .
    container_name: discord-bot
    depends_on:
      - db
    environment:
      - DOTNET_RUNNING_IN_CONTAINER=true
      - DOTNET_USE_POLLING_FILE_WATCHER=true
      - DOTNET_HOST_PATH=/usr/share/dotnet/dotnet
      - DOTNET_VERSION=8.0
      - TOKEN=your_bot_token_here // Ваш токен бота
      - AUDIO_SERVICES=true // Включить музыкальные сервисы
      - LAVALINK_PASSWORD=youshallnotpass
      - LAVALINK_ADDRESS=http://lavalink:2333
      - LAVALINK_WEBSOCKET=ws://lavalink:2333/v4/websocket
    restart: unless-stopped

  db: 
    image: postgres:16
    container_name: postgres-db
    restart: unless-stopped
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: zenith
      POSTGRES_DB: zenith
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  lavalink: // Лавалинк
    image: ghcr.io/lavalink-devs/lavalink:4-alpine
    container_name: lavalink
    restart: unless-stopped
    volumes:
      - ./application.yml:/opt/Lavalink/application.yml
    expose:
      - "2333"
    ports:
      - "2333:2333"

networks:
  lavalink:
    name: lavalink

volumes:
  postgres_data:
```
