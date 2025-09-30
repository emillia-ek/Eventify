<div align="center">
  <h1 style="color:#db65ba;">🎉 EVENTIFY 🎉</h1>
  <h3 style="color:#b04692;">Transforming Events Into Seamless Experiences</h3>
</div>

---

## Table of Contents
- [Overview](#overview)
- [Why Eventify?](#why-eventify)
- [Getting Started](#getting-started)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Features](#features)

---

## Overview
**Eventify** is a event management framework built with **.NET 9.0**, designed to streamline scalable, role-based scheduling applications. It offers a rich console interface and modular architecture supporting diverse event types and reservation workflows.  

---

## Why Eventify?
Eventify simplifies complex event workflows while ensuring **secure, role-based access**.  

### Core Features
- 🎯 **Role-Based Access Control**: Admins, Managers, and Users have specific permissions.  
- 🗓️ **Event & Reservation Management**: Multiple event types with persistent tracking.  
- 💻 **Console UI Utilities**: Styled, user-friendly command-line interactions.  
- ⚙️ **Modular & Event-Driven Architecture**: Scalable, decoupled components for easy extension.  
- 🔐 **Secure Authentication**: Password hashing and user management.

---

## Getting Started

### Prerequisites
- **Programming Language:** C#  
- **Environment:** .NET 9.0  
- **Package Manager:** NuGet  
- **Libraries:** Spectre.Console, Figgle, System.Text.Json

---

### Installation

1. Clone the repository
```bash
git clone https://github.com/emillia-ek/Eventify
```
2. Navigate to the project directory:
```bash
cd Eventify
```
3. Install dependencies:
```bash
dotnet restore
```
4. Build and run the project:
```bash
dotnet build
dotnet run
```

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

[⬆ Back to top](#table-of-contents)
