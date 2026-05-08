Aplikacja typu REST API służąca do zarządzania procesem rezerwacji wizyt lekarskich. Jest to pierwszy etap projektu skupiający się na implementacji domeny, poprawnej strukturze folderowej oraz separacji warstw za pomocą obiektów DTO.

Kluczowe Funkcjonalności (Etap 1)
Zarządzanie Pacjentami: Pełny CRUD (tworzenie, odczyt, aktualizacja, usuwanie).

Rezerwacja Wizyt: System zapobiegania nakładaniu się terminów (blokada rezerwacji w odstępie mniejszym niż 30 min).

Bezpieczeństwo danych: Walidacja formatu email oraz unikalności adresów w bazie.

Architektura: Wykorzystanie wzorca DTO (Data Transfer Objects) oraz wstrzykiwania zależności (Dependency Injection).

Technologie
Platforma: .NET 8.0

Język: C# 12

Typ API: ASP.NET Core Web API

Przechowywanie danych: In-Memory Database (Singleton Service z obsługą thread-safety).


Wymagania do uruchomienia
SDK: .NET 8.0 SDK (lub nowszy)

IDE: Visual Studio 2022 (v17.8 lub nowszy) lub VS Code.

Instrukcja uruchomienia
Pobierz kod źródłowy lub sklonuj repozytorium z GitHub.

Otwórz plik rozwiązania VisitBookingSystem.sln (lub plik projektu .csproj) w programie Visual Studio.

Poczekaj na przywrócenie pakietów NuGet (odbywa się to automatycznie).

Uruchom aplikację, naciskając przycisk F5 lub klikając zieloną strzałkę "VisitBookingSystem".

Po uruchomieniu aplikacji należy do adresu w przeglądarce dopisać na końcu /swagger (np. https://localhost:7001/swagger), aby zobaczyć interfejs Swagger UI i przetestować wszystkie endpointy API.**
