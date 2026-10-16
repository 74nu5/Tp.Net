# TP-05 · notes formateur

## À préparer avant la séance

- Le SDK .NET 11 sur son poste, à côté du SDK .NET 10 : RC2 (`11.0.100-rc.2.26504.105`) à la rédaction. `net11/global.json` accepte toute version 11.0.1xx plus récente, GA comprise (novembre 2026). Vérifier `cd tp-05-bonus-debogage/net11 && dotnet --version`.
- Un débogueur qui fonctionne sur les tests : Rider, Visual Studio, ou VS Code avec C# Dev Kit. Le faire tourner une fois avant la séance (lancer un test en débogage).
- SQL Server et le secret `ConnectionStrings:Tickets` de l'API (les mêmes qu'aux TP précédents), base `TicketsDev` migrée.
- Aspire n'a besoin ni de Docker ni du CLI `aspire` : l'orchestrateur et le tableau de bord viennent des packages NuGet (`AspireUseCliBundle=false` dans l'AppHost). Le premier `dotnet run` de l'AppHost télécharge ces packages.
- Solution : 26 tests verts (`dotnet test --solution tp-05-bonus-debogage/solution/Tickets.slnx`), et 3 pour `net11/solution`. Au départ, 22 tests sur 26 échouent.

## Déroulé et timing

| Exercice | Prévu | Réel |
|---|---|---|
| 1 · Le débogueur : points d'arrêt conditionnels et Hot Reload | 30 min | |
| 2 · Lire une exception et sa stack trace | 15 min | |
| 3 · Régler les logs : niveaux par catégorie et logs structurés | 15 min | |
| 4 · Suivre une requête avec Aspire | 40 min | |
| 5 · Aperçu de .NET 11 : les unions de C# 15 | 20 min | |
| 6 · Trois anomalies à corriger | 60 min | |

Total : 3 h. L'exercice 6 est celui qui ressemble le plus à son travail de l'après-midi : s'il reste peu de temps, sauter le point de trace (exercice 1) et les « Structured logs » (exercice 4) plutôt que l'exercice 6. Le faire raconter sa démarche pour chaque anomalie compte plus que la correction.

## Ce qui est semé dans `depart/`

| Où | Quoi | Trouvé avec |
|---|---|---|
| `Import/TicketCsvParser.cs` | découpage naïf sur `,` : les lignes 87, 143 et 201 sont perdues sans un mot ; les guillemets de la ligne 30 restent dans le titre | point d'arrêt conditionnel |
| `EfTicketRepository.GetStatsAsync` | `Sum(t => t.CommentCount)` : propriété non mappée, EF Core ne la traduit pas en SQL | stack trace |
| `appsettings.json` | le SQL d'EF Core loggé en production | — |
| `EfTicketRepository.CloseAsync` | log par chaîne interpolée | — |
| `EfTicketRepository.UpdateAsync` (anomalie 1) | `AsNoTracking()` : `SaveChanges` n'envoie aucun `UPDATE`, l'API répond 204 | logs SQL |
| `CachedTicketRepository` (anomalie 2) | clé `"tickets:summaries"` en lecture, `"tickets:summary"` à l'invalidation | logs SQL, débogueur |
| `TicketStatistics.Compute` (anomalie 3) | `Average` sur une séquence vide (base sans ticket fermé) | stack trace, test unitaire |

Les réponses attendues dans `tests/Tickets.Tests/Answers.cs` (87 et `EfTicketRepository.GetStatsAsync`) sont comparées à des empreintes SHA-256 : lire les tests ne les donne pas.

## Choix de conception à connaître

- **Pas de bug de concurrence sur le `DbContext`.** Un premier jet de l'exercice 2 lançait deux requêtes avec `Task.WhenAll` sur le même contexte. L'erreur « A second operation was started on this context » apparaît à chaque appel dans l'appli lancée, mais pas de façon fiable sous `WebApplicationFactory`. Un test qui dépend du timing ne vaut rien pour l'autovalidation, d'où l'erreur de traduction LINQ, déterministe. Le sujet reste bon pour une discussion.
- **Le `TicketsApi__BaseUrl` passé par `WithEnvironment`** plutôt que la découverte de services (`WithReference` et `http://api`) : le front continue de démarrer seul, sans Aspire, avec son `appsettings.json`.
- **SQL Server n'est pas une ressource Aspire** : l'API garde sa chaîne de connexion en user-secrets. `AddSqlServer` (conteneur) est en « pour aller plus loin », pour ne pas exiger Docker.
- **Un seul projet de tests** (`Tickets.Tests`), qui référence l'API et l'AppHost. Les deux ont une classe `Program` : la référence à l'AppHost porte un alias (`Aliases="AppHost"`, puis `extern alias AppHost;` dans `Exercise04Tests.cs`).
- **Les tests de l'AppHost ne lancent rien** : `DistributedApplicationTestingBuilder.CreateAsync` construit le modèle d'application, et les tests lisent ses ressources et ses annotations. Ils tournent en CI sans orchestrateur.
- **Exercice 5** : CS8509 (switch non exhaustif) est une erreur dans `Tickets.Preview.csproj`, pour retrouver le comportement du `match` de Rust. Les unions de C# 15 ne demandent pas `LangVersion preview` avec le SDK RC2.

## Pièges attendus

- **Point d'arrêt conditionnel sur la mauvaise ligne** : sur `var fields = …`, la variable n'existe pas encore et la condition échoue (silencieusement dans certains IDE).
- **`AsEnumerable()` pour faire taire l'erreur de l'exercice 2** : la requête passe, mais `Comments` n'est pas chargé et le total vaut 0. Excellent sujet de discussion.
- **Niveau de log dans le mauvais fichier** : les tests vérifient Development **et** Production.
- **Tableau de bord Aspire inaccessible** : il faut l'URL avec le jeton `?t=…` affichée dans la console. Avertissement sur le certificat de développement en http : sans conséquence.
- **L'API ne démarre pas sous Aspire** : regarder l'onglet *Console* de la ressource `api` (souvent la chaîne de connexion absente).
- **Jeton refusé à l'import** : le jeton `user-jwts` doit avoir le rôle `admin`.
- **`dotnet --version` donne 10 dans `net11/`** : SDK .NET 11 absent, ou `global.json` d'un dossier parent pris en compte parce que la commande est lancée ailleurs.

## Questions à lui poser

- Quelle différence entre un point d'arrêt conditionnel et un point de trace ? Quand préférer un log ?
- Dans une stack trace, quelle frame regarder en premier ? Que signifie `--->` ?
- Pourquoi une requête EF Core qui compile peut-elle échouer à l'exécution ? Comparer avec JPQL ou une requête SQL écrite en chaîne.
- Pourquoi un log structuré plutôt qu'une chaîne interpolée ? Qu'est-ce que l'exploitation peut faire de plus ?
- Qu'apporte Aspire en développement ? Qu'est-ce qui resterait en production (OpenTelemetry, health checks) ?
- Union de C# 15, `sealed interface` de Java, `enum` de Rust : qu'est-ce qui change, qu'est-ce qui est pareil ?
- Pour chaque anomalie : comment l'auriez-vous évitée ? Quel test l'aurait attrapée plus tôt ?
