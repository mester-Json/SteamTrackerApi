

#  Steam Tracker API

Une API Web développée avec **.NET 9 / ASP.NET Core** permettant de consommer les données publiques de l'API Steam Web. Le projet permet de récupérer des informations sur les profils de joueurs ainsi que les statistiques et pourcentages de déblocage des succès (trophées) de jeux Steam.

---

##  Fonctionnalités

*  **Gestion des Joueurs :** Récupération du résumé du profil (pseudo, avatar, nombre de jeux possédés, temps de jeu total et sur les 2 dernières semaines).
*  **Gestion des Succès (Achievements) :**
* Liste globale de tous les succès d'un jeu avec leur taux de déblocage (pourcentage global).
* Consultation d'un succès spécifique pour un jeu donné.



---

##  Stack Technique

* **Framework :** .NET 9 (ASP.NET Core Web API)
* **Client HTTP :** Typed HttpClients (`IHttpClientFactory`)
* **Désérialisation :** `System.Text.Json`
* **Documentation API :** Swagger UI (Swashbuckle)

---

##  Configuration

Avant de démarrer l'application, tu dois configurer ta clé d'API Steam Web (Steam API Key).

1. Obtiens une clé API sur le portail officiel : [https://steamcommunity.com/dev/apikey](https://steamcommunity.com/dev/apikey)
2. Dans le fichier `appsettings.json` (ou `appsettings.Development.json`), ajoute la clé comme suit :

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "SteamApiKey": "TA_CLE_API_STEAM_ICI",
  "Steam": {
    "BaseUrl": "https://api.steampowered.com/",
    "Endpoints": {
      "PlayerSummary": "ISteamUser/GetPlayerSummaries/v2/",
      "OwnedGames": "IPlayerService/GetOwnedGames/v1/",
      "GlobalAchievement": "ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/"
    }
  }
}

```

---

##  Démarrage du projet

**Restaurer les dépendances et lancer l'application :**
```bash
dotnet run
```


**Accès au projet :**
* **API Base :** `http://localhost:5037` *(ou le port attribué dans `launchSettings.json`)*
* **Documentation Swagger UI :** [http://localhost:5037/swagger](http://localhost:5037/swagger)


---

##  Structure du projet

```text
SteamTrackerApi/
│
├── Controllers/
│   ├── AchievementsController.cs
│   └── PlayersController.cs
│
├── Dto/
│   ├── AchievementDto.cs
│   └── PlayerDto.cs
│
├── Services/
│   ├── IAchievementService.cs
│   ├── AchievementService.cs
│   ├── ISteamService.cs
│   └── SteamService.cs
│
├── Program.cs
└── appsettings.json

```