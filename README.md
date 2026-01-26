## 🛠️ Tech Stack
### **Frontend**
- **Framework:** Angular 19
- **Language:** TypeScript
- **Styling:** CSS3 with animations
- **HTTP Client:** Angular HttpClient
- **Routing:** Angular Router
### **Backend**
- **Framework:** ASP.NET Core 8 Web API
- **Language:** C#
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Real-time:** Polling (ตอนนี้มีผู้เล่นไม่เกิน 10 คน)
### **DevOps**
- **Tunnel:** Cloudflare Tunnel
- **Version Control:** Git

## 📁 Project Structure
```
KingsCup/
├── KingsCup.API/              # Backend (.NET Core)
│   ├── Controllers/           # API endpoints
│   ├── Models/               # Data models
│   ├── Data/                 # Database context
│   └── Migrations/           # EF Core migrations
│
├── KingsCup.Web/             # Frontend (Angular)
│   ├── src/
│   │   ├── app/
│   │   │   ├── pages/       # Page components
│   │   │   │   ├── login/
│   │   │   │   ├── lobby/
│   │   │   │   ├── game-room/
│   │   │   │   └── game-board/
│   │   │   └── app.routes.ts
│   │   └── assets/
│   │       ├── audio/       # Background music
│   │       └── sounds/      # Sound effects
│   └── angular.json
│
└── README.md

## 🔮 Future Features
- [ ] 💬 In-game chat
- [ ] 📊 Leaderboard system
- [ ] 🍺 Total sips tracking
- [ ] 🎨 Multiple themes
- [ ] 🌍 Multi-language support
- [ ] 📱 Progressive Web App (PWA)
- [ ] 🎥 Video chat integration
- [ ] 🏆 Achievements system
- [ ] SignalR replaces Polling 
