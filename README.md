# ZenithBeep — DisCatSharp

Новая реализация бота на DisCatSharp. Бизнес-логика находится в `ZenithBeep.DisCatSharp`, а доступ к данным — в отдельной библиотеке `ZenithBeep.Data`. Состояние хранится в SQLite через EF Core: лобби, временные войс-каналы и селекторы ролей переживают перезапуск.

## Запуск

1. Скопируйте `.env.example` в `.env` и задайте `TOKEN`.
2. Запустите `dotnet run --project ZenithBeep.DisCatSharp`.

Или запустите только бота: `docker compose up -d --build`. Для бота вместе с Lavalink: `docker compose -f docker-compose.yml -f docker-compose.music.yml up -d --build`, после чего в `.env` установите `AUDIO_SERVICES=true`.

Прокси включается только при `PROXY_ENABLED=true`; иначе `HTTP_PROXY`/`HTTPS_PROXY` игнорируются. База по умолчанию — `data/zenithbeep.db`.
