# Eventify 🎫

**Eventify** to konsolowa aplikacja do zarządzania wydarzeniami i rezerwacjami. Pozwala użytkownikom przeglądać wydarzenia, rezerwować miejsca, a menedżerom i administratorom — zarządzać wydarzeniami oraz użytkownikami.

## 🔧 Funkcjonalności

- Rejestracja i logowanie użytkowników
- Różne role: Użytkownik, Menedżer, Administrator
- Przeglądanie dostępnych wydarzeń
- Rezerwacja i anulowanie wydarzeń
- Zarządzanie wydarzeniami (dodawanie, edytowanie, usuwanie)
- Zarządzanie użytkownikami (tylko admin)
- Generowanie prostych raportów (dla menedżerów/adminów)

## 🖥️ Wymagania systemowe

- .NET SDK 9.0
- System operacyjny: Windows, macOS lub Linux

## 🚀 Uruchomienie aplikacji

### 1. Pobranie projektu

Pobierz paczkę `.zip` z repozytorium lub strony projektu i wypakuj ją do wybranego katalogu.

### 2. Otwórz folder w terminalu

Otwórz terminal i przejdź do wypakowanego folderu projektu, np.:
cd Eventify

3. Budowanie aplikacji
Zbuduj projekt poleceniem:

dotnet build
4. Uruchomienie aplikacji
Aby uruchomić aplikację, wpisz:

dotnet run
👥 Role i dostęp

Użytkownik – przegląda wydarzenia i dokonuje rezerwacji
Menedżer – zarządza wydarzeniami, generuje raporty
Administrator – pełny dostęp do użytkowników i wydarzeń
