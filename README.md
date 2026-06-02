# GameLogBack

> Backend REST API dla aplikacji **GameLog** — osobistego dziennika gier, który pozwala śledzić i katalogować własną bibliotekę.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-EF_Core_8-336791?logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-ready-2496ED?logo=docker)

---

## Spis treści

- [Funkcjonalności](#funkcjonalności)
- [Stos technologiczny](#stos-technologiczny)
- [Architektura projektu](#architektura-projektu)
- [Dokumentacja API](#dokumentacja-api)
- [Pierwsze kroki](#pierwsze-kroki)
- [Uruchomienie przez Docker](#uruchomienie-przez-docker)
- [Zmienne środowiskowe](#zmienne-środowiskowe-produkcja)
- [Testy](#testy)

---

## Funkcjonalności

- **Uwierzytelnianie JWT** — tokeny dostępowe i odświeżające (access + refresh token)
- **Zarządzanie użytkownikami** — rejestracja, potwierdzenie konta e-mailem, odzyskiwanie hasła
- **Biblioteka gier** — dodawanie, edycja, usuwanie gier z okładkami
- **Kategorie** — własna organizacja gier w kategorie
- **Przechowywanie plików** — obrazy gier na magazynie zgodnym z S3
- **Zewnętrzne dane** — integracja z GameBrain API (metadane gier)
- **Paginacja** — wszystkie endpointy listujące obsługują stronicowanie

---

## Stos technologiczny

| Warstwa        | Technologia                        |
|----------------|------------------------------------|
| Framework      | ASP.NET Core 8.0                   |
| ORM            | Entity Framework Core 8            |
| Baza danych    | PostgreSQL (Npgsql)                |
| Autentykacja   | JWT Bearer                         |
| Walidacja      | FluentValidation                   |
| Magazyn plików | AWS S3 / kompatybilne (np. Tigris) |
| E-mail         | SMTP (Brevo)                       |
| Dokumentacja   | Swagger / OpenAPI                  |
| Testy          | xUnit v3, Moq, FluentAssertions    |

---

## Architektura projektu

```
GameLogBack/
├── Controllers/        # Warstwa HTTP — routing, walidacja żądań
├── Services/           # Logika biznesowa
├── Interfaces/         # Kontrakty serwisów
├── Entities/           # Modele EF Core (tabele bazy danych)
├── Dtos/               # Obiekty transferu danych (request/response)
├── Validators/         # Reguły walidacji FluentValidation
├── Middlewares/        # Globalna obsługa błędów
├── Migrations/         # Migracje EF Core
├── Settings/           # Klasy konfiguracyjne (JWT, SMTP, S3)
└── DbContext/          # Kontekst bazy danych

GameLogBack.Tests/      # Projekt testów jednostkowych
```

---

## Dokumentacja API

Swagger UI dostępny pod adresem `http://localhost:8080/swagger` po uruchomieniu aplikacji.

### Autentykacja — `/api/auth`

| Metoda   | Ścieżka          | Autoryzacja | Opis                                        |
|----------|------------------|-------------|---------------------------------------------|
| `POST`   | `/login`         | —           | Logowanie — zwraca access token i refresh token |
| `POST`   | `/refresh-token` | Bearer      | Odświeżenie tokenu dostępowego              |
| `DELETE` | `/logout`        | Bearer      | Wylogowanie i unieważnienie sesji           |
| `GET`    | `/verify`        | Bearer      | Weryfikacja ważności tokenu                 |

### Użytkownicy — `/api/user`

| Metoda | Ścieżka                      | Autoryzacja | Opis                                         |
|--------|------------------------------|-------------|----------------------------------------------|
| `POST` | `/register`                  | —           | Rejestracja nowego konta                     |
| `GET`  | `/get-user`                  | Bearer      | Pobranie profilu zalogowanego użytkownika    |
| `PUT`  | `/update`                    | Bearer      | Aktualizacja danych użytkownika              |
| `POST` | `/resend-code`               | —           | Ponowne wysłanie kodu potwierdzającego       |
| `POST` | `/confirm-user`              | —           | Aktywacja konta za pomocą kodu              |
| `POST` | `/recovery-password`         | —           | Wysyłka e-maila z linkiem do resetu hasła   |
| `POST` | `/recovery-update-password`  | —           | Ustawienie nowego hasła przy użyciu kodu    |

### Kategorie — `/api/category`

> Wszystkie endpointy wymagają Bearer tokenu, chyba że zaznaczono inaczej.

| Metoda   | Ścieżka                              | Autoryzacja | Opis                                            |
|----------|--------------------------------------|-------------|-------------------------------------------------|
| `GET`    | `/get-user-categories`               | Bearer      | Kategorie zalogowanego użytkownika (paginacja)  |
| `GET`    | `/get-categories-by-userId/{userId}` | —           | Publiczne kategorie wskazanego użytkownika      |
| `GET`    | `/get-category/{categoryId}`         | Bearer      | Pobranie pojedynczej kategorii                  |
| `POST`   | `/create-category`                   | Bearer      | Utworzenie kategorii                            |
| `PUT`    | `/update/{categoryId}`               | Bearer      | Aktualizacja kategorii                          |
| `DELETE` | `/delete/{categoryId}`               | Bearer      | Usunięcie kategorii                             |

### Gry — `/api/games`

> Wszystkie endpointy wymagają Bearer tokenu, chyba że zaznaczono inaczej.

| Metoda   | Ścieżka                                   | Autoryzacja | Opis                                        |
|----------|-------------------------------------------|-------------|---------------------------------------------|
| `GET`    | `/get-games`                              | Bearer      | Gry zalogowanego użytkownika (paginacja)    |
| `GET`    | `/get-games_by_categoryId/{categoryId}`   | Bearer      | Gry w danej kategorii                       |
| `GET`    | `/get-games-by-userId/{userId}`           | —           | Publiczne gry wskazanego użytkownika        |
| `GET`    | `/get-game/{gameId}`                      | Bearer      | Pobranie pojedynczej gry                    |
| `POST`   | `/create-game`                            | Bearer      | Dodanie gry                                 |
| `PUT`    | `/update/{gameId}`                        | Bearer      | Aktualizacja gry                            |
| `DELETE` | `/delete/{gameId}`                        | Bearer      | Usunięcie gry                               |

---

## Pierwsze kroki

### Wymagania

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Instancja PostgreSQL
- Magazyn S3-compatible (np. [Tigris](https://www.tigrisdata.com/), MinIO, AWS S3)
- Konto SMTP (np. [Brevo](https://www.brevo.com/))

### Instalacja

1. Sklonuj repozytorium:

   ```bash
   git clone <repo-url>
   cd GameLogBack
   ```

2. Skonfiguruj `GameLogBack/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "Postgres": "Host=localhost;Database=gamelogdb;User Id=postgres;Password=twoje-haslo;"
     },
     "Authentication": {
       "JwtKey": "<sekret-min-64-znaki>",
       "JwtTokenExpireMinutes": 1,
       "JwtAccessTokenExpireMinutes": 3,
       "JwtIssuer": "https://localhost:8080"
     },
     "SmtpSettings": {
       "Address": "twoj@adres-smtp.com",
       "Password": "haslo-smtp",
       "SmtpServer": "smtp-relay.brevo.com",
       "Port": 587
     },
     "BasicAWSCredentials": {
       "AccessKey": "twoj-access-key",
       "SecretKey": "twoj-secret-key"
     },
     "AmazonS3Config": {
       "ServiceURL": "https://endpoint-s3",
       "ForcePathStyle": true
     },
     "BucketName": "nazwa-bucketa"
   }
   ```

3. Zastosuj migracje bazy danych:

   ```bash
   dotnet ef database update --project GameLogBack
   ```

4. Uruchom aplikację:

   ```bash
   dotnet run --project GameLogBack
   ```

5. Swagger UI dostępny pod: `http://localhost:8080/swagger`

---

## Uruchomienie przez Docker

**Obraz deweloperski:**

```bash
docker build -f Dockerfile.dev -t gamelogback:dev .
docker run -p 8080:8080 gamelogback:dev
```

**Obraz produkcyjny:**

```bash
docker build -f Dockerfile -t gamelogback:prod .
docker run -p 8080:8080 gamelogback:prod
```

---

## Zmienne środowiskowe (produkcja)

W środowisku produkcyjnym konfiguracja pochodzi ze zmiennych środowiskowych zamiast z `appsettings.json`.

| Zmienna                        | Opis                                          |
|-------------------------------|-----------------------------------------------|
| `CONNECTION_STRING`           | Connection string do PostgreSQL               |
| `JWT_KEY`                     | Sekret podpisywania JWT                       |
| `JWT_TOKEN_EXPIRE_MINUTES`    | Czas życia refresh tokenu (minuty)            |
| `JWT_ACCESS_TOKEN_EXPIRE_DAYS`| Czas życia access tokenu (dni)                |
| `JWT_ISSUER`                  | URL wystawcy JWT                              |
| `BASIC_AWS_ACCESS_TOKEN`      | Access key do S3                              |
| `BASIC_AWS_SECRET_KEY`        | Secret key do S3                              |
| `BASIC_AWS_SERVICE_URL`       | URL endpointu S3                              |
| `AWS_FORCE_PATH_STYLE`        | `true` dla path-style URL w S3                |
| `BUCKET_NAME`                 | Nazwa bucketa S3                              |
| `GAME_BRAIN_API_URL`          | URL bazowy GameBrain API                      |
| `GAME_BRAIN_API_KEY`          | Klucz API GameBrain                           |
| `GENERATE_FILTER_OPTIONS`     | Włączenie opcji filtrowania GameBrain (`true`/`false`) |

---

## Testy

Projekt testów korzysta z xUnit v3, Moq oraz FluentAssertions.

```bash
dotnet test GameLogBack.Tests
```
