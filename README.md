# VisitBookingSystem

REST API do zarządzania rezerwacjami wizyt lekarskich, zbudowane w ASP.NET Core. Projekt realizowany w trzech etapach — od prostego CRUD przez reguły biznesowe aż po trwałą bazę danych i eksport danych.

## Technologie

| Element | Wersja |
|---|---|
| Platforma | .NET 10.0 |
| Język | C# 12 |
| Framework | ASP.NET Core Web API |
| Baza danych | SQLite (Entity Framework Core 9) |
| Dokumentacja | Swagger / OpenAPI (Swashbuckle 6.5) |

## Funkcjonalności

### Zarządzanie pacjentami
- Pełny CRUD (tworzenie, odczyt, aktualizacja, usuwanie)
- Walidacja unikalności adresu e-mail
- Usunięcie pacjenta kaskadowo usuwa jego wizyty
- Sortowanie listy po: `lastName` (domyślnie), `firstName`, `email`
- Eksport listy pacjentów do pliku CSV

### Zarządzanie wizytami
- Pełny CRUD
- Sortowanie listy po: `visitDate` (domyślnie), `patientId`, `isCancelled`
- Eksport listy wizyt do pliku CSV (z imieniem i nazwiskiem pacjenta)

### Reguły biznesowe
1. **Blokada kolizji terminów** — nowa wizyta nie może być zaplanowana w oknie ±30 minut od istniejącej aktywnej wizyty.
2. **Limit aktywnych rezerwacji** — pacjent może mieć maksymalnie 3 aktywne (nieanulowane) wizyty jednocześnie.
3. **Blokada anulowania "last minute"** — anulowanie wizyty jest niemożliwe na mniej niż 2 godziny przed jej terminem.
4. **Zakaz rezerwacji wstecznych** — data wizyty musi być w przyszłości.

### Architektura
- Wzorzec DTO (Data Transfer Objects) — separacja modeli domenowych od danych API
- Dependency Injection — serwisy rejestrowane jako `Scoped`
- Middleware globalnej obsługi wyjątków — spójne odpowiedzi błędów (400, 404)
- Automatyczne stosowanie migracji EF Core przy starcie aplikacji

## Struktura projektu

```
VisitBookingSystem/
├── Controllers/
│   ├── AppointmentsController.cs   # Endpointy wizyt
│   └── PatientsController.cs       # Endpointy pacjentów
├── Data/
│   └── AppDbContext.cs             # Kontekst EF Core
├── DTOs/
│   ├── AppointmentDto.cs
│   ├── CreateAppointmentDto.cs
│   ├── UpdateAppointmentDto.cs
│   ├── CreatePatientDto.cs
│   └── PatientDto.cs
├── Exceptions/                     # Własne klasy wyjątków
├── Middleware/
│   └── ExceptionMiddleware.cs      # Globalna obsługa błędów
├── Migrations/                     # Migracje EF Core
├── Models/
│   ├── Appointment.cs
│   └── Patient.cs
├── Services/
│   ├── IServices.cs                # Interfejsy serwisów
│   ├── AppointmentService.cs       # Logika biznesowa wizyt
│   └── PatientService.cs           # Logika biznesowa pacjentów
├── appsettings.json                # Konfiguracja (connection string)
├── Program.cs                      # Punkt wejścia, konfiguracja DI
└── visits.db                       # Plik bazy danych SQLite (generowany)
```

## Wymagania

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)

## Uruchomienie (dotnet run)

```bash
# 1. Sklonuj repozytorium
git clone <adres-repozytorium>
cd VisitBookingSystem

# 2. Przywróć pakiety NuGet
dotnet restore

# 3. Uruchom aplikację
dotnet run
```

> Migracje bazy danych są stosowane **automatycznie przy każdym starcie** — plik `visits.db` zostanie utworzony w katalogu projektu, jeśli nie istnieje. Nie trzeba ręcznie uruchamiać `dotnet ef database update`.

Po uruchomieniu terminal wyświetli adres nasłuchiwania, np.:

```
Now listening on: https://localhost:7001
Now listening on: http://localhost:5000
```

Otwórz w przeglądarce:

```
https://localhost:7001/swagger
```

## Endpointy API

### Pacjenci — `/api/patients`

| Metoda | Ścieżka | Opis |
|---|---|---|
| GET | `/api/patients` | Lista pacjentów (parametry: `sortBy`, `sortOrder`) |
| GET | `/api/patients/{id}` | Dane pojedynczego pacjenta |
| POST | `/api/patients` | Utwórz pacjenta |
| PUT | `/api/patients/{id}` | Zaktualizuj pacjenta |
| DELETE | `/api/patients/{id}` | Usuń pacjenta (i jego wizyty) |
| GET | `/api/patients/export` | Pobierz plik `patients.csv` |

### Wizyty — `/api/appointments`

| Metoda | Ścieżka | Opis |
|---|---|---|
| GET | `/api/appointments` | Lista wizyt (parametry: `sortBy`, `sortOrder`) |
| GET | `/api/appointments/{id}` | Dane pojedynczej wizyty |
| POST | `/api/appointments` | Utwórz wizytę |
| PUT | `/api/appointments/{id}` | Zaktualizuj / anuluj wizytę |
| DELETE | `/api/appointments/{id}` | Usuń wizytę |
| GET | `/api/appointments/export` | Pobierz plik `appointments.csv` |

### Parametry sortowania

```
GET /api/patients?sortBy=firstName&sortOrder=desc
GET /api/appointments?sortBy=visitDate&sortOrder=asc
```

**Pacjenci** — `sortBy`: `lastName` (domyślnie), `firstName`, `email`  
**Wizyty** — `sortBy`: `visitDate` (domyślnie), `patientId`, `isCancelled`  
**sortOrder**: `asc` (domyślnie) lub `desc`
