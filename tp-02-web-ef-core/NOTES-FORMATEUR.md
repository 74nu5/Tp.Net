# TP-02 · notes formateur

## À préparer avant la séance

- **SQL Server sur son poste**, avec SSMS. Noter le nom de l'instance (`localhost`, `.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`) et le mode d'authentification. La chaîne par défaut des tests (`testsettings.json`) suppose `localhost` en authentification Windows : à adapter sinon.
- `dotnet tool install --global dotnet-ef` (version 10).
- Vérifier une fois `dotnet test --solution tp-02-web-ef-core/solution/Tickets.slnx` sur son poste : c'est le test le plus sûr que SQL Server, la chaîne de connexion et les droits de création de base sont bons. Attendu : 36 tests verts en une dizaine de secondes.
- Les tests créent et suppriment eux-mêmes la base `TicketsTests` ; l'alternant crée `TicketsDev` à l'exercice 5 avec `dotnet ef database update`. L'identifiant user-secrets (`tp-net-tickets-api`) est partagé par les TP 02 à 04.

## Déroulé et timing

| Exercice | Prévu | Réel |
|---|---|---|
| 1 · Program.cs et middleware | 15 min | |
| 2 · Durées de vie de l'injection de dépendances | 15 min | |
| 3 · Configuration et options | 15 min | |
| 4 · Minimal APIs | 35 min | |
| 5 · EF Core : modèle, connexion, migration | 35 min | |
| 6 · EF Core : requêtes et N+1 | 30 min | |
| 7 · EF Core : écritures et 2ᵉ migration | 20 min | |
| 8 · Ses tests d'intégration | 15 min | |

Une démo de 5 minutes avant l'exercice 5 (le cycle modèle → migration → base) aide beaucoup. L'exercice 5 est le plus long et celui où l'on bloque sur l'environnement : y passer en premier.

## Pièges attendus

- **Chaîne de connexion lue trop tôt** : si elle est lue dans `Program.cs` avant `builder.Build()`, les tests ne peuvent pas la remplacer et tous les tests de base échouent avec « Connection string 'Tickets' is missing » ou visent la base de dev. D'où la consigne de la lire dans la lambda d'`AddDbContext`.
- **`TrustServerCertificate=True` oublié** : erreur de certificat à la première connexion. L'expliquer, c'est un vrai sujet de prod.
- **Dépôt laissé en singleton** : `EfTicketRepository` dépend du `DbContext` scoped ; en développement l'application refuse de démarrer (captive dependency). Bon moment pour revenir sur l'exercice 2.
- **`SaveChangesAsync` oublié** : rien n'est écrit, sans erreur.
- **`dotnet ef` lancé depuis le mauvais dossier** : il faut être dans `src/Tickets.Api` (ou passer `--project`). Le secret doit être défini avant `migrations add`, car l'outil démarre l'application.
- **Migration mal nommée** : le test de l'exercice 7 attend une dernière migration qui se termine par `_AddClosedAt`.
- **JSON invalide → 500 au lieu de 400** : déjà corrigé dans `depart/` par le `StatusCodeSelector` d'`UseExceptionHandler`. À lui faire lire, c'est une particularité du mode Development.
- **Le test N+1 compte les commandes SQL** : une projection avec `t.Comments.Count` passe ; un `Include` puis un `Count()` en mémoire passe aussi, mais charge tous les commentaires. En discuter.

## Questions à lui poser

- Dans quel ordre les middlewares s'exécutent-ils, et pourquoi le chronomètre doit-il être le premier ?
- Pourquoi le `DbContext` est-il scoped et pas singleton ?
- Où vit le secret user-secrets, et pourquoi pas dans `appsettings.json` ?
- Que contient `__EFMigrationsHistory`, et que se passe-t-il au déploiement si une migration manque ?
- Quelle différence entre une entité suivie et une entité lue avec `AsNoTracking` ?
- Comment repérerait-il un N+1 sur une de nos applis en production ?
