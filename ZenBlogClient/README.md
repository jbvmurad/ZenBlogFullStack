# ZenBlogClient 🚀
A Docker-ready **Angular frontend** for the ZenBlog platform. It connects to **ZenBlogServer API** through proxy configuration in development and through **Nginx reverse proxy** in Docker.

---

# 🇦🇿 Azərbaycan dilində

## Layihə nədir?
**ZenBlogClient** ZenBlog platformasının frontend hissəsidir. Angular ilə yazılıb və həm lokal development rejimində, həm də Docker içində işləmək üçün hazırlanıb.

Bu client aşağıdakı hissələri təqdim edir:
- public blog ana səhifəsi
- login / register / Google sign-in
- email verification və password reset axınları
- istifadəçi profile və settings səhifələri
- admin panel (category, blog, comment, contact info, message, social, user roles)

---

## Əsas imkanlar
- **Angular 20** əsaslı SPA
- **JWT auth** ilə session idarəetməsi
- **Google Identity Services** ilə Google login/register
- **HTTP interceptor** ilə token göndərilməsi və error handling
- **Role-based admin access** (`AuthGuard` + admin yoxlaması)
- **Proxy config** ilə lokalda backend API-yə rahat bağlantı
- **Docker + Nginx** ilə container içində işləmə
- **Profile / settings** səhifələrində istifadəçi məlumatlarının yenilənməsi

---

## Texnologiyalar
- **Angular 20.1.x**
- **TypeScript 5.8.x**
- **RxJS 7.8.x**
- **Bootstrap 5.3.x**
- **AlertifyJS**
- **SweetAlert2**
- **Font Awesome**
- **AOS**
- **Swiper**
- **Nginx** (Docker runtime üçün)

---

## Qovluq strukturu
```text
src/app/
  _admin-components/   # admin səhifələri
  _main-components/    # public və auth səhifələri
  _layouts/            # main və admin layout-lar
  _services/           # API service-lər
  _guards/             # route protection
  _interceptors/       # token / error interceptor-lər
  _models/             # DTO və model-lər
  _configs/            # Google auth config və s.
```

---

## Route-lar

### Public route-lar
- `/` → Home
- `/login` → Login
- `/register` → Register
- `/forgot-password` → Forgot Password
- `/reset-password` → Reset Password
- `/verify-email` → Verify Email
- `/blogdetails/:id` → Blog detail
- `/contact` → Contact
- `/profile` → Profile (**auth tələb edir**)
- `/settings` → Settings (**auth tələb edir**)

### Email link alias-ləri
Aşağıdakı route alias-lər də dəstəklənir:
- `/verifyEmail`
- `/VerifyEmail`
- `/confirm-email`
- `/confirmEmail`
- `/resetPassword`
- `/ResetPassword`

### Admin route-lar
- `/admin/category`
- `/admin/blog`
- `/admin/comment`
- `/admin/contactinfo`
- `/admin/message`
- `/admin/social`
- `/admin/users`

> Admin route-lar üçün həm login, həm də admin role tələb olunur.

---

## Tez Başlama (Local Development)

### 0) Tələblər
- **Node.js 20+**
- **npm**
- işləyən **ZenBlogServer** backend-i

### 1) Paketləri quraşdır
```bash
npm install
```

### 2) Development server-i başlat
```bash
npm start
```

Bu komanda faktiki olaraq bunu işə salır:
```bash
ng serve --proxy-config proxy.conf.json
```

Browser-də aç:
```text
http://localhost:4200
```

---

## Backend bağlantısı necə işləyir?
Local development zamanı Angular birbaşa `/api` və `/uploads` request-lərini backend-ə proxy edir.

### `proxy.conf.json`
```json
{
  "/api": {
    "target": "http://localhost:18080",
    "secure": false,
    "changeOrigin": true,
    "logLevel": "warn"
  },
  "/uploads": {
    "target": "http://localhost:18080",
    "secure": false,
    "changeOrigin": true,
    "logLevel": "warn"
  }
}
```

Yəni development rejimində:
- frontend → `http://localhost:4200`
- backend API → `http://localhost:18080`

Əgər backend başqa portda işləyirsə, `proxy.conf.json` içində `target` dəyərini dəyiş.

---

## Build
```bash
npm run build
```

Build nəticəsi `dist/ZenBlogClient/browser` altında yaranır.

Production build Angular config içində aktivdir və output hashing istifadə olunur.

---

## Test
```bash
npm test
```

Bu layihədə unit test üçün Karma/Jasmine konfiqurasiyası mövcuddur.

---

## Docker ilə işlətmə
Bu frontend multi-stage Dockerfile istifadə edir.

### Docker image build et
```bash
docker build -t zenblog-client .
```

### Container run et
```bash
docker run -p 4200:80 zenblog-client
```

Sonra browser-də aç:
```text
http://localhost:4200
```

---

## Docker içində API bağlantısı
Docker runtime zamanı frontend **Nginx reverse proxy** istifadə edir.

### `nginx.conf`
- `/api/` → `http://api:8080/api/`
- `/uploads/` → `http://api:8080/uploads/`

Bu o deməkdir ki:
- frontend container backend-i `api` host adı ilə görməlidir
- adətən bu Docker Compose service name olur

Əgər server container adı fərqlidirsə, `nginx.conf` içində `proxy_pass` dəyərlərini uyğunlaşdır.

---

## Authentication axını
Client JWT token-i `localStorage` daxilində saxlayır:
- `token`
- `currentUserProfile`
- `currentUserIsAdmin`

### AuthService nə edir?
- login / register request-ləri göndərir
- Google login request-i göndərir
- current user profilini cache edir
- admin status-u refresh edir
- profile update sonrası user state-i yeniləyir
- logout zamanı session-u təmizləyir

---

## Google Login / Register
Google button-lar aşağıdakı component-lərdə render olunur:
- `login`
- `register`

Client ID burada saxlanılır:
```ts
src/app/_configs/google-auth.ts
```

Nümunə:
```ts
export const GOOGLE_CLIENT_ID = '...';
```

### Vacib qeyd
Bu dəyər backend-dəki Google auth config ilə **eyni** olmalıdır.
Server tərəfdə uyğun `GoogleAuth:ClientId` fərqli olsa:
- Google button görünə bilər
- amma sign-in zamanı audience / `aud` mismatch xətası ala bilərsən

---

## Guards və admin giriş
`AuthGuard` iki tip qoruma edir:
- normal auth tələb edən səhifələr
- admin-only route-lar

Admin route üçün əlavə `data: { adminOnly: true }` istifadə olunur.
Guard lazım olduqda backend-dən user role-larını yoxlayır.

---

## İstifadəçi profili və settings
Settings səhifəsində istifadəçi aşağıdakı məlumatları yeniləyə bilər:
- full name
- email
- phone number
- image
- password (istəyə bağlı)

Profil update zamanı:
- normal update və ya media ilə update işləyə bilər
- update sonrası current user yenidən refresh olunur
- dəyişikliklərin UI-də görünməsi üçün session state yenilənir

---

## Tez-tez rast gəlinən problemlər

### Google button görünmür
Yoxla:
- browser console error varmı
- Google script düzgün yüklənibmi
- `GOOGLE_CLIENT_ID` boş deyilmi
- component DOM içində target element varmı (`googleBtn`, `googleBtnRegister`)

### Google sign-in zamanı audience mismatch
Səbəb adətən budur:
- client tərəfdəki `GOOGLE_CLIENT_ID`
- backend tərəfdəki `GoogleAuth:ClientId`

bir-birinə uyğun deyil.

### API-yə qoşulmur (`status 0` və ya network error)
Yoxla:
- backend işləyirmi
- `proxy.conf.json` target düzgündürmü
- backend `http://localhost:18080` üzərində açıqdırmı

### `413 Request Entity Too Large`
Bu xəta adətən böyük şəkil və ya media upload edəndə olur.
Bu Angular xətası deyil; request ölçüsü server və ya proxy limitini keçir.

### Admin dashboard görünmür
Yoxla:
- user həqiqətən admin role alıbmı
- JWT içində role claim varmı
- və ya `UserRole` endpoint düzgün cavab qaytarırmı

---

## Faydalı komandalar
```bash
npm install
npm start
npm run build
npm test
```

---

# 🇬🇧 English

## What is this project?
**ZenBlogClient** is the Angular frontend for the ZenBlog platform. It is designed to work both in local development and inside Docker.

It provides:
- public blog pages
- login / register / Google sign-in
- email verification and password reset flows
- user profile and settings pages
- admin dashboard for content and user-role management

---

## Highlights
- **Angular 20 SPA**
- **JWT-based auth flow**
- **Google Identity Services** integration
- **HTTP interceptors** for token/error handling
- **Role-based admin protection**
- **Proxy config** for local API calls
- **Docker + Nginx** support
- **Profile/settings update flows** with user-state refresh

---

## Tech Stack
- **Angular 20.1.x**
- **TypeScript 5.8.x**
- **RxJS 7.8.x**
- **Bootstrap 5.3.x**
- **AlertifyJS**
- **SweetAlert2**
- **Font Awesome**
- **AOS**
- **Swiper**
- **Nginx**

---

## Project Structure
```text
src/app/
  _admin-components/
  _main-components/
  _layouts/
  _services/
  _guards/
  _interceptors/
  _models/
  _configs/
```

---

## Routes

### Public routes
- `/`
- `/login`
- `/register`
- `/forgot-password`
- `/reset-password`
- `/verify-email`
- `/blogdetails/:id`
- `/contact`
- `/profile` (**auth required**)
- `/settings` (**auth required**)

### Email-link aliases
Supported aliases:
- `/verifyEmail`
- `/VerifyEmail`
- `/confirm-email`
- `/confirmEmail`
- `/resetPassword`
- `/ResetPassword`

### Admin routes
- `/admin/category`
- `/admin/blog`
- `/admin/comment`
- `/admin/contactinfo`
- `/admin/message`
- `/admin/social`
- `/admin/users`

Admin routes require authenticated access and admin role.

---

## Quick Start (Local Development)

### 0) Requirements
- **Node.js 20+**
- **npm**
- a running **ZenBlogServer** backend

### 1) Install dependencies
```bash
npm install
```

### 2) Start development server
```bash
npm start
```

This runs:
```bash
ng serve --proxy-config proxy.conf.json
```

Open:
```text
http://localhost:4200
```

---

## How backend connectivity works
In development mode, Angular proxies `/api` and `/uploads` requests to the backend.

If your backend runs on a different port, update `proxy.conf.json`.

Default dev setup:
- frontend → `http://localhost:4200`
- backend → `http://localhost:18080`

---

## Build
```bash
npm run build
```

Output folder:
```text
dist/ZenBlogClient/browser
```

---

## Test
```bash
npm test
```

Karma/Jasmine is configured for unit testing.

---

## Docker usage
### Build image
```bash
docker build -t zenblog-client .
```

### Run container
```bash
docker run -p 4200:80 zenblog-client
```

Then open:
```text
http://localhost:4200
```

---

## API proxy inside Docker
At runtime, Nginx forwards requests:
- `/api/` → `http://api:8080/api/`
- `/uploads/` → `http://api:8080/uploads/`

So your backend container should be reachable as `api` in the same Docker network.

---

## Authentication
The client stores session data in `localStorage`:
- `token`
- `currentUserProfile`
- `currentUserIsAdmin`

`AuthService` is responsible for:
- login / register / forgot-password / reset-password calls
- Google login
- current-user caching
- admin-status refresh
- session cleanup on logout
- refreshing user state after profile updates

---

## Google login/register
Google button rendering depends on:
- `src/app/_configs/google-auth.ts`
- Google Identity Services being loaded correctly
- matching backend Google auth configuration

The frontend `GOOGLE_CLIENT_ID` must match the backend `GoogleAuth:ClientId`.

---

## Common issues

### Google button does not appear
Check:
- browser console errors
- Google script loading
- `GOOGLE_CLIENT_ID`
- target DOM element existence

### Audience / `aud` mismatch on Google sign-in
The frontend and backend Google client IDs do not match.

### API unreachable / `status 0`
Check:
- backend is running
- `proxy.conf.json` target is correct
- backend is accessible on `http://localhost:18080`

### `413 Request Entity Too Large`
Usually caused by uploading a media file that exceeds the server/proxy request-size limit.

### Admin dashboard not appearing
Check:
- whether the user actually has admin role
- whether role claims exist in JWT
- whether the `UserRole` endpoint returns the expected data

---

## Useful commands
```bash
npm install
npm start
np