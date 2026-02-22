# ZenBlogServer (Docker) — Redis + RabbitMQ + LocalStack(S3) + Gmail SMTP

Bu proje Docker Compose ile tek komutla ayağa kalkır:

- **PostgreSQL**
- **Redis**
- **RabbitMQ (Management UI dahil)**
- **LocalStack (AWS S3 emulyatoru)**
- **ZenBlog.API (.NET)**

> **MailHog YOK.** Register olunduğu an doğrulama maili **Gmail** SMTP ilə real Gmail inbox-a gedəcək.

---

## 1) Gmail SMTP üçün lazım olanlar

Gmail artıq “less secure apps” icazəsi vermir. Ona görə **Gmail App Password** istifadə etməlisən.

1. Gmail hesabında **2-Step Verification** aktiv et.
2. Google Account → **Security** → **App passwords** → yeni app password yarat.
3. Aşağıdakı iki env dəyişəni hazırla:

- `GMAIL_USER` → sənin gmail ünvanın (məs: `jabiyevmurad02@gmail.com`)
- `GMAIL_APP_PASSWORD` → yaratdığın app password (16 simvol olur)

---

## 2) Proyektin işə salınması

Layihə qovluğunda:

### Windows (PowerShell)

```powershell
$env:GMAIL_USER="you@gmail.com"
$env:GMAIL_APP_PASSWORD="xxxx xxxx xxxx xxxx"

# Port dəyişmək istəsən:
# $env:API_PORT="8080"

docker compose up -d --build
```

### Windows (CMD)

```bat
set GMAIL_USER=you@gmail.com
set GMAIL_APP_PASSWORD=xxxx xxxx xxxx xxxx

REM Port dəyişmək istəsən:
REM set API_PORT=8080

docker compose up -d --build
```

### macOS/Linux

```bash
export GMAIL_USER="you@gmail.com"
export GMAIL_APP_PASSWORD="xxxx xxxx xxxx xxxx"

# Port dəyişmək istəsən:
# export API_PORT="8080"

docker compose up -d --build
```

> İstəsən `.env` faylı da yarada bilərsən (Docker Compose avtomatik oxuyur):
>
> ```env
> GMAIL_USER=you@gmail.com
> GMAIL_APP_PASSWORD=xxxx xxxx xxxx xxxx
> API_PORT=8080
> ```

---

## 3) URL-lər

- **API (Scalar UI):** `http://localhost:${API_PORT:-8080}/scalar`
- **OpenAPI JSON:** `http://localhost:${API_PORT:-8080}/openapi/v1.json`
- **RabbitMQ UI:** `http://localhost:15672`
  - user: `zenblog`
  - pass: `zenblogpass`
- **LocalStack (AWS endpoint):** `http://localhost:4566`

---

## 4) AWS (LocalStack) S3 test

S3 işləyir/işləmir test etmək üçün:

```bash
docker compose exec localstack awslocal s3 mb s3://zenblog-local
docker compose exec localstack awslocal s3 cp /etc/hosts s3://zenblog-local/hosts.txt
docker compose exec localstack awslocal s3 ls s3://zenblog-local/
```

---

## 5) Gmail-ə mailin getdiyini necə yoxlayım?

1. API-də **Register** endpointinə request at.
2. Gmail inbox-da (və ya Spam-da) “Verify Your Account” tipli mail görünməlidir.

Əgər gəlmirsə:

- `docker compose logs -f api` içində SMTP error görəcəksən.
- Şəbəkə/firewall **smtp.gmail.com:587** çıxışını bloklamamalıdır.
- `GMAIL_APP_PASSWORD` mütləq App Password olmalıdır (normal gmail şifrə işləmir).

---

## Stop / Cleanup

```bash
docker compose down
```

Həcmləri də silmək istəsən (DB, rabbitmq data silinir):

```bash
docker compose down -v
```
