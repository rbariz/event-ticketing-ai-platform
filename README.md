# Event Ticketing AI Platform

Real-time ticket validation platform focused on antifraud, high-concurrency validation, AI-assisted risk scoring, and operator workflows.

---

## Vision

Build a production-grade event ticketing validation system beyond CRUD:

- real-time scan validation
- duplicate scan detection
- single-use ticket enforcement
- full scan audit trail
- AI-assisted risk scoring
- operator dashboard (Ops Center)
- mobile scanner app for field operations

---

## 🚀 What’s implemented

### Ticket validation engine
- Real-time validation API
- Duplicate scan detection
- Expired / cancelled / already-used ticket handling
- Concurrency-safe validation logic

### Antifraud & AI layer
- Rule-based risk scoring (0–100)
- Risk levels (Low / Medium / High)
- Fraud signals detection:
  - duplicate_scan
  - expired_ticket
  - already_used
  - multi_device
  - unknown_ticket

### AI explanation system
- OpenAI integration (optional)
- Automatic fallback (no dependency on external AI)
- Bilingual support (EN / FR)

### API layer
- Clean REST endpoints
- ProblemDetails error handling
- Integration-tested endpoints

### Dashboard backend
- KPI aggregation
- Scan history with filters
- Risk analysis endpoint
- Fraud scenario aggregation

### Frontend (Ops Center)
- React + Vite + Tailwind
- Dashboard with KPIs
- Scan history with advanced filters
- Risk analysis panel with AI explanations
- Ticket lookup
- Manual scan simulation
- Bilingual UI (EN / FR) with dynamic language switching

### 📱 Mobile Scanner (MAUI)
- Manual scan (ticket code input)
- Camera scan (QR / barcode)
- Real-time validation from backend API
- AI risk analysis integration
- Ticket lookup
- Recent scans monitoring
- Mobile-first UI (Blazor Hybrid)

---

## 🧠 AI & Risk Engine

Each scan produces:

- `RiskScore` (0–100)
- `RiskLevel` (Low / Medium / High)
- `RecommendedAction` (Allow / Monitor / ManualReview)
- `RiskSignals`

AI explanation:
- Uses OpenAI when quota is available
- Falls back to deterministic logic otherwise

---

## 🏗️ Architecture

- ASP.NET Core Web API (.NET 8)
- PostgreSQL + Entity Framework Core
- Clean Architecture (Domain / Application / Infrastructure / API)
- React frontend (Ops Center)
- .NET MAUI Blazor Hybrid mobile app
- InMemory + PostgreSQL interchangeable infrastructure

---

## 📡 API Overview

### Scan validation

POST /api/scans/validate

### Risk analysis

GET /api/scans/{id}/risk?lang=en|fr

### Dashboard

GET /api/dashboard/summary

### Scan history

GET /api/scans  
GET /api/scans/recent  
GET /api/scans/{id}

---

## 📊 Ops Center

Located in `/ops-center`

Features:
- KPI dashboard
- Scan history with filters
- Risk analysis panel
- Ticket lookup
- Manual scan simulation

---

## 📱 Mobile Scanner (MAUI)

Located in `/mobile/EventTicketingAiPlatform.Mobile.Scanner`

Designed for field agents to validate tickets in real-time.

### Features
- Dual scan modes:
  - Manual input
  - Camera (QR / barcode)
- Real-time validation
- Fraud detection & risk scoring
- AI explanation (OpenAI optional, fallback included)
- Ticket lookup
- Recent scans

### Technical
- .NET MAUI Blazor Hybrid
- ZXing.Net.Maui (camera scanning)
- Shared contracts with backend API

---

## 🧪 Demo Fraud Scenarios

The system includes realistic seeded scenarios:

- valid ticket
- expired ticket
- cancelled ticket
- already used ticket
- duplicate scans across devices
- unknown tickets

---

## ⚙️ Configuration

### Database

ConnectionStrings:DefaultConnection

### OpenAI (optional)

OpenAI__Enabled=true  
OpenAI__ApiKey=your_key

---

## 🛡️ Fault Tolerance

- OpenAI failures → automatic fallback
- API returns ProblemDetails
- Safe frontend rendering (no crashes on undefined data)

---

## 📸 Screenshots

### Dashboard
![Dashboard](docs/screenshots/dashboard.png)

### Scan History
![Scan History](docs/screenshots/scan-history.png)

### Risk Analysis
![Risk Analysis](docs/screenshots/risk-analysis.png)

### Ticket Lookup
![Ticket Lookup](docs/screenshots/ticket-lookup.png)

### Manual Scan
![Manual Scan](docs/screenshots/manual-scan.png)

---

## 📱 Mobile Screenshots

### Home
![Mobile Home](docs/screenshots/mobile/mobile-home.png)

### Manual Scan
![Manual Scan](docs/screenshots/mobile/mobile-manual.png)

### Camera Scan
![Camera Scan](docs/screenshots/mobile/mobile-camera.png)

### Ticket Lookup
![Ticket Lookup](docs/screenshots/mobile/mobile-ticket-lookup.png)

### Recent Scans
![Recent Scans](docs/screenshots/mobile/mobile-recent.png)

---

## 🚧 Roadmap

1. Real-time notifications (SignalR)  
2. Advanced fraud detection (behavioral patterns)  
3. Multi-event / multi-tenant support  
4. Analytics & reporting  

---

## Author

Rachid Bariz  
Senior Full-Stack .NET Architect