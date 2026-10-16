# TP-02 · Serveur web et Entity Framework Core

> Demi-journée · mardi 20 octobre, matin · **énoncé à rédiger**

Comment tourne une application web .NET (host, injection de dépendances, configuration, Minimal APIs), puis la persistance avec EF Core : la moitié de la séance y est consacrée.

## Avant de commencer

- Ouvrir `depart/Tickets.slnx` dans votre IDE.
- Point de départ : `dotnet new webapi -n Tickets.Api, puis dotnet tool install --global dotnet-ef et dotnet add package Microsoft.EntityFrameworkCore.SqlServer`.

## Exercices

Environ 3 h 00 de manipulation.

| # | Exercice | Durée |
|---|---|---|
| 1 | Lire le Program.cs d'un projet webapi et ajouter un middleware qui journalise chaque requête | 15 min |
| 2 | Enregistrer un service en Singleton, Scoped et Transient et observer la différence | 15 min |
| 3 | Lier une section d'appsettings.json à une classe d'options, la surcharger par variable d'environnement et user-secrets | 15 min |
| 4 | Écrire une Minimal API CRUD de tickets en mémoire, avec TypedResults, validation et OpenAPI | 35 min |
| 5 | EF Core : DbContext, entités Ticket et Commentaire (relation 1-n), provider SQL Server sur l'instance locale, chaîne de connexion dans user-secrets, première migration et dotnet ef database update ; vérifier le schéma dans SSMS | 35 min |
| 6 | EF Core : requêtes LINQ et SQL généré dans les logs ; Include, projection, AsNoTracking ; repérer et corriger un N+1 | 30 min |
| 7 | EF Core : écritures, suivi des modifications et SaveChanges, une migration qui ajoute une colonne | 20 min |
| 8 | Un test d'intégration de l'API avec WebApplicationFactory | 15 min |

*Le détail de chaque exercice (consigne, indices) sera ajouté ici.*

## Valider son travail

```bash
dotnet test --solution depart/Tickets.slnx
```

Quand tous les tests passent au vert, l'exercice est terminé. Comparez ensuite votre code avec `solution/` : il y a souvent plusieurs bonnes réponses, la correction en commente une.

**Livrable :** Une API « tickets » persistée dans SQL Server par EF Core, avec deux migrations, réutilisée par les demi-journées Blazor.

## Pont depuis Java, Rust et JavaScript

ASP.NET Core ≈ Spring Boot, tout se déclare dans Program.cs ; Minimal APIs ≈ Express ou Axum. EF Core ≈ JPA/Hibernate ; les migrations ≈ Flyway/Liquibase ou celles de diesel/sqlx ; LINQ to Entities ≈ Criteria API, typé à la compilation.

## Notions abordées

Generic Host, Kestrel, middlewares, injection de dépendances, IConfiguration / IOptions, Minimal APIs, OpenAPI, DbContext, migrations, LINQ to Entities, Include et projections, change tracking, N+1, Microsoft.EntityFrameworkCore.SqlServer, chaîne de connexion.
