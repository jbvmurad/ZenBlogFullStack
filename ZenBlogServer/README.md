# ZenBlogServer 🚀  
A Docker-first **.NET API** with **PostgreSQL**, **Redis**, **RabbitMQ (Wolverine)**, and **LocalStack S3** for media storage.

---

# 🇦🇿 Azərbaycan dilində

## Layihə nədir?
**ZenBlogServer** blog platforması üçün backend-dir. Docker Compose ilə bir komanda ilə bütün infrastruktur (DB, cache, queue, S3) ayağa qalxır.

### Əsas imkanlar
- **Clean-ish architecture**: `Domain / Application / Infrastructure / Persistence / API`
- **EF Core migrations**: istəyə görə startup-da avtomatik tətbiq olunur
- **RabbitMQ + Wolverine**: background işlər (xüsusən email) üçün
- **LocalStack (S3)**: blog/social şəkilləri üçün S3-uyğun storage
- **Scalar UI + OpenAPI**: API sənədləşməsi

---

## Texnologiyalar
- **.NET (Docker image: 10.0)**  
- **PostgreSQL 17**
- **Redis 7**
- **RabbitMQ 3 (Management UI ilə)**
- **LocalStack 3 (S3 service)**

---

## Tez Başlama (Docker Compose)

### 0) Tələblər
- Docker Desktop + Docker Compose v2

### 1) Konfiqurasiya
`.env.example` faylını `.env` kimi kopyala və doldur:

```bash
# Windows PowerShell:
copy .env.example .env
# macOS/Linux:
cp .env.example .env
```

Minimum olaraq bunlar lazımdır:
- `GMAIL_USER`
- `GMAIL_APP_PASSWORD` (Gmail **App Password**, normal şifrə deyil)
- `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_SECRET_KEY`
- (istəyə görə) `GOOGLE_AUTH_CLIENT_ID`

> Gmail üçün: Google hesabında **2FA** aktiv et → **App passwords** bölməsindən 16 simvolluq app password yarat.

### 2) Start
Layihə qovluğunda:

```bash
docker compose up -d --build
```

### 3) URL-lər
Default portlarla:
- **API Scalar UI:** `http://localhost:18080/scalar`
- **OpenAPI JSON:** `http://localhost:18080/openapi/v1.json`
- **Health:** `http://localhost:18080/health`
- **RabbitMQ UI:** `http://localhost:15682`  
  - user: `zenblog`  
  - pass: `zenblog`
- **LocalStack endpoint:** `http://localhost:14566`

Portları `.env` içində bu dəyişənlərlə dəyişə bilərsən:
`ZENBLOG_API_PORT`, `ZENBLOG_POSTGRES_PORT`, `ZENBLOG_REDIS_PORT`, `ZENBLOG_RABBITMQ_UI_PORT`, `ZENBLOG_LOCALSTACK_PORT` və s.

---

## RabbitMQ burada nə üçün istifadə olunur?

### ✅ Email axınları async (queue ilə)
Bu layihədə email göndərmə request-in içində “SMTP gözləmə” etməsin deyə **RabbitMQ** üzərindən işləyir.

- Queue: **`zenblog.email`**
- Producer: register/forgot-password/delete kimi auth axınları
- Consumer: Wolverine handler-lər email göndərir
- Fail olarsa: Wolverine retry edə bilər və mesajlar **dead-letter** queue-ya düşə bilər

RabbitMQ queue-ları yoxlama:
```bash
docker compose exec rabbitmq rabbitmqctl list_queues name messages
```

---

## Media (Blog/Social şəkilləri) harada saxlanılır?

Docker rejimində storage provider **S3** kimi işləyir (real AWS deyil, **LocalStack S3**):
- Bucket: `zenblog-local`
- Prefix: `uploads/`

### S3 test (AWS CLI qurmadan)
Windows-da hostda `aws` yoxdursa, LocalStack container-ın içindən yoxla:

```bash
# Bucket-ları gör
docker compose exec localstack awslocal s3 ls

# uploads/ prefix-i altında olan fayllar
docker compose exec localstack awslocal s3 ls s3://zenblog-local/uploads/

# Test upload
docker compose exec localstack sh -lc "echo hello > /tmp/test.txt && awslocal s3 cp /tmp/test.txt s3://zenblog-local/uploads/test.txt"

# Yenə list
docker compose exec localstack awslocal s3 ls s3://zenblog-local/uploads/
```

> Qeyd: `localstack-init` container-ının `make_bucket: zenblog-local` yazıb **Exited(0)** olması normaldır. O “one-shot init” kimi işləyir.

---

## Stop / Cleanup
```bash
docker compose down
```

Həcmləri də silmək istəsən (DB data silinir):
```bash
docker compose down -v
```

---

## Tez-tez rast gəlinən problemlər

### “aws is not recognized” (Windows)
Hostda AWS CLI yoxdursa bu normaldır. `awslocal` istifadə et:
```bash
docker compose exec localstack awslocal s3 ls
```

### Gmail SMTP auth error
- `GMAIL_APP_PASSWORD` mütləq **App Password** olmalıdır
- Firewall `smtp.gmail.com:587` çıxışını bloklamamalıdır
- `docker compose logs -f api` ilə SMTP error-u görəcəksən

---

# 🇬🇧 English

## What is this project?
**ZenBlogServer** is a backend for a blog platform. It’s designed to run “infra + API” with a single Docker Compose command.

### Highlights
- **Clean-ish architecture**: `Domain / Application / Infrastructure / Persistence / API`
- **EF Core migrations** can run automatically on startup
- **RabbitMQ + Wolverine** for background work (especially email)
- **LocalStack (S3)** for storing blog/social images in a local S3-compatible bucket
- **Scalar UI + OpenAPI** for interactive API docs

---

## Tech Stack
- **.NET (Docker image: 10.0)**
- **PostgreSQL 17**
- **Redis 7**
- **RabbitMQ 3 (with Management UI)**
- **LocalStack 3 (S3)**

---

## Quick Start (Docker Compose)

### 0) Prerequisites
- Docker Desktop + Docker Compose v2

### 1) Configuration
Copy `.env.example` to `.env` and fill the values:

```bash
# Windows PowerShell:
copy .env.example .env
# macOS/Linux:
cp .env.example .env
```

Minimum required:
- `GMAIL_USER`
- `GMAIL_APP_PASSWORD` (**Gmail App Password**, not your normal password)
- `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_SECRET_KEY`
- (optional) `GOOGLE_AUTH_CLIENT_ID`

> Gmail note: Enable **2FA**, then create an **App Password** (16 chars) from your Google Account security settings.

### 2) Start
```bash
docker compose up -d --build
```

### 3) Useful URLs (defaults)
- **API Scalar UI:** `http://localhost:18080/scalar`
- **OpenAPI JSON:** `http://localhost:18080/openapi/v1.json`
- **Health:** `http://localhost:18080/health`
- **RabbitMQ UI:** `http://localhost:15682`  
  - user: `zenblog`  
  - pass: `zenblog`
- **LocalStack endpoint:** `http://localhost:14566`

You can override ports in `.env` (e.g., `ZENBLOG_API_PORT`, `ZENBLOG_POSTGRES_PORT`, `ZENBLOG_RABBITMQ_UI_PORT`, etc.).

---

## Why RabbitMQ here?

### ✅ Async email flows (queue-driven)
Email sending is handled asynchronously so API requests don’t block on SMTP.

- Queue: **`zenblog.email`**
- Producers: register / forgot-password / delete-account flows
- Consumers: Wolverine handlers send emails
- Failures: can be retried and may land in a dead-letter queue

Check queues:
```bash
docker compose exec rabbitmq rabbitmqctl list_queues name messages
```

---

## Where are media files stored?

In Docker mode the storage provider is **S3** (backed by **LocalStack**, not real AWS):
- Bucket: `zenblog-local`
- Prefix: `uploads/`

### S3 test (no AWS CLI required on Windows)
Use `awslocal` inside the LocalStack container:

```bash
docker compose exec localstack awslocal s3 ls
docker compose exec localstack awslocal s3 ls s3://zenblog-local/uploads/

docker compose exec localstack sh -lc "echo hello > /tmp/test.txt && awslocal s3 cp /tmp/test.txt s3://zenblog-local/uploads/test.txt"
docker compose exec localstack awslocal s3 ls s3://zenblog-local/uploads/
```

> Note: It’s expected that `localstack-init` prints `make_bucket: zenblog-local` and exits with code 0 — it’s a one-off init container.

---

## Stop / Cleanup
```bash
docker compose down
```

Remove volumes too (wipes DB/cache data):
```bash
docker compose down -v
```

---

## Troubleshooting

### “aws is not recognized” on Windows
That’s fine. Use:
```bash
docker compose exec localstack awslocal s3 ls
```

### Gmail SMTP auth failures
- `GMAIL_APP_PASSWORD` must be an **App Password**
- Ensure outbound access to `smtp.gmail.com:587`
- Check logs:
```bash
docker compose logs -f api
```
