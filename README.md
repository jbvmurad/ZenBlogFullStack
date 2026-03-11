<div align="center">

# ✨ ZenBlogFullStack

<p align="center">
  <strong>Modern, Docker-first, full-stack blog platform</strong><br/>
  <sub>Angular frontend • .NET API • PostgreSQL • Redis • RabbitMQ • LocalStack S3</sub>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Angular-20-red?style=for-the-badge&logo=angular" alt="Angular">
  <img src="https://img.shields.io/badge/.NET-10-purple?style=for-the-badge&logo=dotnet" alt=".NET">
  <img src="https://img.shields.io/badge/PostgreSQL-17-blue?style=for-the-badge&logo=postgresql" alt="PostgreSQL">
  <img src="https://img.shields.io/badge/Redis-7-red?style=for-the-badge&logo=redis" alt="Redis">
  <img src="https://img.shields.io/badge/RabbitMQ-3-orange?style=for-the-badge&logo=rabbitmq" alt="RabbitMQ">
  <img src="https://img.shields.io/badge/LocalStack-S3-00bcd4?style=for-the-badge&logo=amazonaws" alt="LocalStack">
  <img src="https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker" alt="Docker">
</p>

<p align="center">
  <a href="#-azərbaycan-dilində">Azərbaycan dili</a> •
  <a href="#-english">English</a>
</p>

</div>

---

# 🇦🇿 Azərbaycan dilində

## Layihə nədir?

**ZenBlogFullStack** ZenBlog platformasının həm **frontend**, həm də **backend** hissəsini birləşdirən tam layihədir.

Bu repository aşağıdakı hissələri əhatə edir:

- **ZenBlogClient** → Angular frontend
- **ZenBlogServer** → .NET API backend
- **docker-compose.yml** → bütün servisləri birlikdə ayağa qaldıran vahid compose faylı

Bu layihə ilə bir komanda vasitəsilə aşağıdakı stack birlikdə işləyir:

- Angular client
- .NET API
- PostgreSQL
- Redis
- RabbitMQ
- LocalStack S3

---

## Niyə bu layihə rahatdır?

- **Single command startup** → `docker compose up --build`
- **Bütün servisler bir compose içindədir**
- **JWT auth + Google login**
- **Role-based admin panel**
- **Async email flow (RabbitMQ + Wolverine)**
- **S3-compatible media storage (LocalStack)**
- **Docker-first development**
- **Modern SPA frontend + modular backend**

---

## Layihə strukturu

```text
ZenBlogFullStack/
│
├─ ZenBlogClient/              # Angular frontend
├─ ZenBlogServer/              # .NET backend
├─ docker-compose.yml          # vahid compose faylı
├─ .env                        # lokal secret/config
├─ .env.example                # example config
└─ README.md                   # bu fayl
```

---

## Arxitektura baxışı

```text
 Browser
    │
    ▼
 Angular Client (ZenBlogClient)
    │
    ▼
 Nginx Reverse Proxy
    │
    ▼
 .NET API (ZenBlogServer)
    │
    ├── PostgreSQL   → əsas məlumat bazası
    ├── Redis        → cache
    ├── RabbitMQ     → queue / background jobs
    └── LocalStack   → S3 uyğun media storage
```

---

## Əsas imkanlar

### Frontend
- **Angular 20** əsaslı SPA
- login / register / Google sign-in
- email verify və reset-password axınları
- profile / settings səhifələri
- admin panel
- JWT token əsaslı auth
- interceptor və guard strukturu
- responsive və modern UI strukturu

### Backend
- **.NET API**
- layer-based architecture
- authentication / authorization
- role management
- blog, category, comment, contact info, social, message modulları
- RabbitMQ queue ilə async email işləmə
- LocalStack S3 ilə media saxlama
- Scalar / OpenAPI sənədləşmə

---

## Tez Başlama

### 0) Tələblər

Sistemdə bunlar olmalıdır:

- **Docker Desktop**
- **Docker Compose v2**

---

### 1) `.env` hazırla

Root qovluqda `.env.example` faylını `.env` kimi kopyala:

```bash
# Windows PowerShell
copy .env.example .env

# macOS / Linux
cp .env.example .env
```

Sonra `.env` içində dəyərləri doldur.

Minimum lazım olanlar:

- `JWT_ISSUER`
- `JWT_AUDIENCE`
- `JWT_SECRET_KEY`
- `GMAIL_USER`
- `GMAIL_APP_PASSWORD`
- `GOOGLE_AUTH_CLIENT_ID` (opsional)

> Qeyd: Gmail üçün normal şifrə deyil, **App Password** istifadə olunmalıdır.

---

### 2) Layihəni ayağa qaldır

Root qovluqda işlət:

```bash
docker compose up --build
```

Arxa fonda işlətmək istəsən:

```bash
docker compose up -d --build
```

---

### 3) Default URL-lər

Compose işə düşəndən sonra:

#### Frontend
- `http://localhost:4200`

#### Backend API
- `http://localhost:18080`

#### API docs
- `http://localhost:18080/scalar`
- `http://localhost:18080/openapi/v1.json`

#### Health
- `http://localhost:18080/health`

#### RabbitMQ UI
- `http://localhost:15682`

Default login:
- user: `zenblog`
- pass: `zenblog`

#### LocalStack
- `http://localhost:14566`

---

## Auth axını necə işləyir?

### Login sonrası header davranışı
Bu layihədə login zamanı:

1. istifadəçi login olur
2. token yazılır
3. success alert görünür
4. bundan **sonra** header içində account hissəsi görünür

Yəni account bölməsi login request göndərilən kimi yox, **uğurlu giriş bildirimi çıxandan sonra** görünəcək şəkildə qurulub.

---

## Route-lar

### Public səhifələr
- `/`
- `/login`
- `/register`
- `/forgot-password`
- `/reset-password`
- `/verify-email`
- `/blogdetails/:id`
- `/contact`

### Auth tələb edən səhifələr
- `/profile`
- `/settings`

### Admin səhifələri
- `/admin/category`
- `/admin/blog`
- `/admin/comment`
- `/admin/contactinfo`
- `/admin/message`
- `/admin/social`
- `/admin/users`

---

## Docker servisləri

Bu full-stack compose içində aşağıdakılar var:

### `web`
Angular client + Nginx

### `api`
.NET backend

### `postgres`
Əsas relational database

### `redis`
Cache layer

### `rabbitmq`
Queue və background processing

### `localstack`
S3-compatible local media storage

### `localstack-init`
Bucket yaratmaq üçün one-shot init service

---

## İstifadə olunan texnologiyalar

### Frontend
- Angular 20
- TypeScript
- RxJS
- Bootstrap
- SweetAlert / Alertify
- AOS
- Swiper
- Nginx

### Backend
- .NET
- EF Core
- PostgreSQL
- Redis
- RabbitMQ
- Wolverine
- LocalStack S3
- Scalar / OpenAPI

---

## Faydalı komandalar

### Servisləri dayandır
```bash
docker compose down
```

### Həcmlərlə birlikdə sil
```bash
docker compose down -v
```

### Log-lara bax
```bash
docker compose logs -f
```

### Tək servis log-u
```bash
docker compose logs -f api
docker compose logs -f web
docker compose logs -f rabbitmq
```

### RabbitMQ queue-ları yoxla
```bash
docker compose exec rabbitmq rabbitmqctl list_queues name messages
```

### LocalStack bucket-ları yoxla
```bash
docker compose exec localstack awslocal s3 ls
```

### Upload fayllarını yoxla
```bash
docker compose exec localstack awslocal s3 ls s3://zenblog-local/uploads/
```

---

## Tez-tez rast gəlinən problemlər

<details>
<summary><strong>1) Frontend açılır, amma API işləmirmiş kimi görünür</strong></summary>

Backend container-in ayaqda olduğunu yoxla:

```bash
docker compose ps
docker compose logs -f api
```

API portunun düzgün olduğunu yoxla:
- `18080`

</details>

<details>
<summary><strong>2) Gmail SMTP error verir</strong></summary>

Bunları yoxla:

- `GMAIL_USER` düzdürmü?
- `GMAIL_APP_PASSWORD` App Password-dürmü?
- Google hesabında 2FA aktivdirmi?
- şəbəkə `smtp.gmail.com:587` çıxışını bloklamır ki?

</details>

<details>
<summary><strong>3) LocalStack / S3 media işləmirsə</strong></summary>

Bucket-i yoxla:

```bash
docker compose exec localstack awslocal s3 ls
```

Əgər bucket yoxdursa, `localstack-init` log-larına bax:

```bash
docker compose logs localstack-init
```

</details>

<details>
<summary><strong>4) Port conflict varsa</strong></summary>

`.env` içində bu portları dəyişə bilərsən:

- `ZENBLOG_API_PORT`
- `ZENBLOG_POSTGRES_PORT`
- `ZENBLOG_REDIS_PORT`
- `ZENBLOG_RABBITMQ_AMQP_PORT`
- `ZENBLOG_RABBITMQ_UI_PORT`
- `ZENBLOG_LOCALSTACK_PORT`

</details>

---

## Development qeydləri

- Root-da **vahid `docker-compose.yml`** istifadə olunur
- ayrıca `ZenBlogServer/docker-compose.yml` artıq lazım deyil
- `.env` artıq root qovluqda saxlanılır
- `.env.example` repo içində qalır
- `.env` isə git ignore olunmalıdır

---

## Git üçün tövsiyə

Repo-da bunlar commit edilməməlidir:

- `.env`
- `.vs/`
- `bin/`
- `obj/`
- `node_modules/`
- `dist/`

---

## Nəticə

**ZenBlogFullStack** həm development, həm də demo üçün rahat qurulan, modern, modul və Docker-first full-stack blog platformasıdır.

Əgər məqsədin:
- full-stack portfolio layihəsi,
- blog platforması bazası,
- auth + admin panel + media upload + async processing öyrənməkdirsə,

bu repository buna çox uyğun strukturdadır.

---

# 🇬🇧 English

## What is this project?

**ZenBlogFullStack** is the complete full-stack version of the ZenBlog platform.

It includes:

- **ZenBlogClient** → Angular frontend
- **ZenBlogServer** → .NET backend API
- **docker-compose.yml** → one unified compose file for the whole system

With a single command, the project runs:

- Angular client
- .NET API
- PostgreSQL
- Redis
- RabbitMQ
- LocalStack S3

---

## Why this project is practical

- **Single command startup** → `docker compose up --build`
- **One compose file for the entire stack**
- **JWT auth + Google login**
- **Role-based admin panel**
- **Async email processing**
- **S3-compatible local media storage**
- **Docker-first setup**
- **Modern SPA frontend + modular backend**

---

## Project structure

```text
ZenBlogFullStack/
│
├─ ZenBlogClient/              # Angular frontend
├─ ZenBlogServer/              # .NET backend
├─ docker-compose.yml          # unified compose file
├─ .env                        # local secrets/config
├─ .env.example                # example config
└─ README.md                   # this file
```

---

## Architecture overview

```text
 Browser
    │
    ▼
 Angular Client (ZenBlogClient)
    │
    ▼
 Nginx Reverse Proxy
    │
    ▼
 .NET API (ZenBlogServer)
    │
    ├── PostgreSQL   → primary database
    ├── Redis        → cache
    ├── RabbitMQ     → queue / background jobs
    └── LocalStack   → S3-compatible media storage
```

---

## Core features

### Frontend
- **Angular 20** SPA
- login / register / Google sign-in
- email verification and password reset flows
- profile / settings pages
- admin panel
- JWT-based authentication
- interceptor and route guard structure
- responsive and modern UI

### Backend
- **.NET API**
- layered architecture
- authentication / authorization
- role management
- blog, category, comment, contact info, social and message modules
- async email flow via RabbitMQ + Wolverine
- media storage with LocalStack S3
- Scalar / OpenAPI documentation

---

## Quick start

### 0) Requirements

You need:

- **Docker Desktop**
- **Docker Compose v2**

---

### 1) Prepare `.env`

Copy `.env.example` to `.env` in the root folder:

```bash
# Windows PowerShell
copy .env.example .env

# macOS / Linux
cp .env.example .env
```

Then fill the required values.

Minimum required values:

- `JWT_ISSUER`
- `JWT_AUDIENCE`
- `JWT_SECRET_KEY`
- `GMAIL_USER`
- `GMAIL_APP_PASSWORD`
- `GOOGLE_AUTH_CLIENT_ID` (optional)

> Note: For Gmail, use an **App Password**, not your normal account password.

---

### 2) Start the project

Run from the root folder:

```bash
docker compose up --build
```

Or in detached mode:

```bash
docker compose up -d --build
```

---

### 3) Default URLs

After the stack starts:

#### Frontend
- `http://localhost:4200`

#### Backend API
- `http://localhost:18080`

#### API docs
- `http://localhost:18080/scalar`
- `http://localhost:18080/openapi/v1.json`

#### Health
- `http://localhost:18080/health`

#### RabbitMQ UI
- `http://localhost:15682`

Default login:
- user: `zenblog`
- pass: `zenblog`

#### LocalStack
- `http://localhost:14566`

---

## How auth flow works

### Header behavior after login
In this project, after login:

1. the user signs in
2. token is stored
3. success alert is shown
4. **then** the account section appears in the header

So the account area does not appear immediately on request start; it becomes visible **after successful login feedback**.

---

## Routes

### Public pages
- `/`
- `/login`
- `/register`
- `/forgot-password`
- `/reset-password`
- `/verify-email`
- `/blogdetails/:id`
- `/contact`

### Auth-required pages
- `/profile`
- `/settings`

### Admin pages
- `/admin/category`
- `/admin/blog`
- `/admin/comment`
- `/admin/contactinfo`
- `/admin/message`
- `/admin/social`
- `/admin/users`

---

## Docker services

This full-stack setup includes:

### `web`
Angular client + Nginx

### `api`
.NET backend

### `postgres`
Primary relational database

### `redis`
Caching layer

### `rabbitmq`
Queue and background processing

### `localstack`
S3-compatible local media storage

### `localstack-init`
One-shot init service for bucket creation

---

## Tech stack

### Frontend
- Angular 20
- TypeScript
- RxJS
- Bootstrap
- SweetAlert / Alertify
- AOS
- Swiper
- Nginx

### Backend
- .NET
- EF Core
- PostgreSQL
- Redis
- RabbitMQ
- Wolverine
- LocalStack S3
- Scalar / OpenAPI

---

## Useful commands

### Stop services
```bash
docker compose down
```

### Remove with volumes
```bash
docker compose down -v
```

### Follow logs
```bash
docker compose logs -f
```

### Follow specific service logs
```bash
docker compose logs -f api
docker compose logs -f web
docker compose logs -f rabbitmq
```

### Check RabbitMQ queues
```bash
docker compose exec rabbitmq rabbitmqctl list_queues name messages
```

### Check LocalStack buckets
```bash
docker compose exec localstack awslocal s3 ls
```

### Check uploaded files
```bash
docker compose exec localstack awslocal s3 ls s3://zenblog-local/uploads/
```

---

## Common issues

<details>
<summary><strong>1) Frontend opens but API looks unavailable</strong></summary>

Check whether the backend container is running:

```bash
docker compose ps
docker compose logs -f api
```

Make sure the API port is correct:
- `18080`

</details>

<details>
<summary><strong>2) Gmail SMTP errors</strong></summary>

Check:

- Is `GMAIL_USER` correct?
- Is `GMAIL_APP_PASSWORD` really an App Password?
- Is 2FA enabled on the Google account?
- Is outbound access to `smtp.gmail.com:587` blocked?

</details>

<details>
<summary><strong>3) LocalStack / S3 media issue</strong></summary>

Check the bucket:

```bash
docker compose exec localstack awslocal s3 ls
```

If the bucket does not exist, inspect:

```bash
docker compose logs localstack-init
```

</details>

<details>
<summary><strong>4) Port conflicts</strong></summary>

You can change these in `.env`:

- `ZENBLOG_API_PORT`
- `ZENBLOG_POSTGRES_PORT`
- `ZENBLOG_REDIS_PORT`
- `ZENBLOG_RABBITMQ_AMQP_PORT`
- `ZENBLOG_RABBITMQ_UI_PORT`
- `ZENBLOG_LOCALSTACK_PORT`

</details>

---

## Development notes

- The project uses a **single root `docker-compose.yml`**
- `ZenBlogServer/docker-compose.yml` is no longer needed
- `.env` is now stored in the root folder
- `.env.example` stays in the repository
- `.env` should be gitignored

---

## Git recommendation

These should not be committed:

- `.env`
- `.vs/`
- `bin/`
- `obj/`
- `node_modules/`
- `dist/`

---

## Final note

**ZenBlogFullStack** is a practical, modern, Docker-first full-stack blog platform that is suitable for both development and demonstration.

If your goal is to build or showcase:

- a full-stack portfolio project,
- a reusable blog platform base,
- auth + admin panel + media upload + async processing,

this repository provides a solid structure for that.

---

<div align="center">
  <sub>Built with focus, structure and a little obsession for clean setup ✨</sub>
</div>

