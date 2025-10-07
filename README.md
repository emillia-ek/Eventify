<div align="center">
  <h1 style="color:#db65ba;">EVENTIFY</h1>
</div>

---
Konsolowa aplikacja napisana w języku C# z wykorzystaniem platformy .NET 9.0, zaprojektowana zgodnie z zasadami programowania obiektowego (OOP).


Program oferuje przyjazny dla użytkownika, intuicyjny interfejs tekstowy (menu), umożliwiający wykonywanie różnych operacji w zależności od roli użytkownika. System obsługuje logowanie, rejestrację nowych kont oraz autoryzację opartą na rolach (RBAC), wyróżniając trzy typy użytkowników: administrator, menedżer i użytkownik – każdy z dostępem do odrębnych funkcjonalności.


W aplikacji można:
- przeglądać dostępne wydarzenia,
- rezerwować bilety na wybrane wydarzenia,
- anulować rezerwacje,
- (dla menedżerów i administratorów) dodawać nowe wydarzenia,
- (dla administratorów) przeglądać raporty dotyczące wydarzeń i rezerwacji.
Całość została zaprojektowana z myślą o czytelności, modularności i łatwej rozbudowie

---

## Features

### User Roles
- **Admin**: Manage users, events, and generate reports.  
- **Manager**: Manage events and generate reports.  
- **Regular User**: Browse events, make/cancel reservations.

### Event Management
- Add, remove, edit events (Concerts, Conferences).  
- Persistent storage in `events.json`.  

### Reservations
- Users can reserve or cancel spots.  
- Stored in `reservations.json`.

### Reports
- **Participation Report**: Reservations per user.  
- **Financial Report**: Revenue from events.  
- **Popularity Report**: Most reserved events.

---

