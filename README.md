# 🚀 Securities Incidents Event Platform built using AI - Ollama, Phi-3, RabbitMQ

> Event-driven .NET proof of concept combining RabbitMQ messaging with local AI-powered incident assessment.

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/)
[![RabbitMQ](https://img.shields.io/badge/Messaging-RabbitMQ-FF6600)](https://www.rabbitmq.com/)
[![Ollama](https://img.shields.io/badge/AI-Ollama-black)](https://ollama.com/)
[![Phi--3](https://img.shields.io/badge/Model-Phi--3-blue)](https://ollama.com/library/phi3)

---

## 📖 Overview

This project simulates securities-servicing incidents and processes them through independent services. RabbitMQ carries events between each stage, while Ollama runs the Phi-3 model locally to generate an AI assessment.

## ✨ Highlights

### 🧠 AI with Ollama and Phi-3

The AI analyzer sends incident details and the initial risk level to Ollama's local `phi3` model. The model returns a business-impact summary, confidence, and recommended action.

### 🐇 Event-driven processing with RabbitMQ

MassTransit and RabbitMQ connect the simulator, processor, AI analyzer, and notification service through asynchronous events. Each service handles its own stage without direct service-to-service calls.

### 🔄 End-to-end incident flow

```text
Incident Simulator
       │ AssetServicingIncident
       ▼
Incident Processor ── assigns risk level
       │ RiskAssessmentProcessed
       ▼
AI Analyzer ── Ollama / Phi-3
       │ AIAssessmentCompleted
       ▼
Notification Service
```

> **Note:** The notification service currently displays the notification action in the console; it does not send real email or SMS.

---

## 🛠️ Technology

- .NET 10
- RabbitMQ and MassTransit
- Ollama with the Phi-3 (`phi3`) model
- xUnit and GitHub Actions

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- RabbitMQ available at `localhost:5672`
- Ollama available at `localhost:11434`

Download the model:

```bash
ollama pull phi3
```

For a quick local RabbitMQ instance using Docker:

```bash
docker run -d --name securities-rabbitmq -p 5672:5672 rabbitmq:3
```

### Run the services

Start each service in its own terminal from the repository root:

```bash
dotnet run --project SecuritiesEventPlatform.Processor/SecuritiesEventPlatform.Consumer.csproj
dotnet run --project SecuritiesEventPlatform.AIAnalyzer/SecuritiesEventPlatform.AIAnalyzer.csproj
dotnet run --project SecuritiesEventPlatform.Notification/SecuritiesEventPlatform.Notification.csproj
dotnet run --project SecuritiesEventPlatform.Simulator/SecuritiesEventPlatform.Simulator.csproj
```

Choose an incident in the simulator menu to follow it through the processing pipeline.

Run the tests:

```bash
dotnet test
```

## 📂 Project Structure

```text
SecuritiesEventPlatform
├── SecuritiesEventPlatform.Simulator       # Publishes sample incidents
├── SecuritiesEventPlatform.Processor       # Assigns an initial risk level
├── SecuritiesEventPlatform.AIAnalyzer      # Gets an AI assessment from Ollama
├── SecuritiesEventPlatform.Notification    # Displays the assessment and action
├── SecuritiesEventPlatform.Contracts       # Shared event contracts
└── SecuritiesEventPlatform.Processor.Tests # Processor tests
```

---

# 👨‍💻 Author

**Rameshkumar Gudala**

Engineering Manager | Cloud Architecture | AI | Azure & AWS  | .NET

Passionate about AI, Engineering Management, Cloud-Native Architecture, .Net, Developer Experience, and Infrastructure Automation.
