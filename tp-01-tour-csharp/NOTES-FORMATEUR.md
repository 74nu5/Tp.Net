# TP-01 · notes formateur

## À préparer avant la séance

- Sur son poste : SDK .NET 10 (`dotnet --list-sdks`), un IDE avec C# (Rider, Visual Studio 2026 ou VS Code + C# Dev Kit), Git, et le dépôt cloné.
- Lancer une fois `dotnet test --solution tp-01-tour-csharp/depart/Katas.slnx` : la restauration NuGet doit passer (accès à nuget.org ou au feed interne). Attendu : 56 tests, 51 en échec, 3 avertissements de build.
- Le dépôt active la gestion centralisée des packages à la racine. `depart/Directory.Packages.props` importe le fichier racine et accueille la version de Humanizer de l'exercice 2 ; `dotnet add package` y écrit bien la version (vérifié).

## Déroulé et timing

| Exercice | Prévu | Réel |
|---|---|---|
| 1 · SDK et CLI | 15 min | |
| 2 · NuGet et CPM | 15 min | |
| 3 · class, struct, record, nullabilité | 30 min | |
| 4 · Pattern matching | 20 min | |
| 5 · LINQ | 40 min | |
| 6 · async/await | 30 min | |
| 7 · Ses propres tests | 30 min | |

Une courte démo de 5 minutes avant l'exercice 3 (le tableau class / struct / record) et avant l'exercice 6 (démarrer les tâches puis `Task.WhenAll`) aide à lancer. Le reste se fait en autonomie, avec passage régulier.

## Pièges attendus

- **Exercice 3** : sans culture explicite, les formats de nombres dépendent des réglages régionaux. Sur un poste en français, `Money.ToString()` sort `12,50 EUR` ; le test `ToString_IgnoresTheRegionalSettings` tourne en fr-FR exprès. D'où l'indice sur `CultureInfo.InvariantCulture`.
- **Exercice 3** : confusion entre `record` (classe, référence) et `record struct` (valeur). Le test `Money_IsAValueType` l'attrape. Autre réflexe à casser : transformer `Order` en record « parce que c'est plus court ». Le test `Order_IsNotARecord` le signale.
- **Exercice 3** : passer `Email` en `string?` fait apparaître un nouvel avertissement dans `EmailDomain`. C'est le moment de montrer que le compilateur guide la correction.
- **Exercice 3** : avec `required`, `new Customer()` sans `Name` ne compile plus. Il faut le lui faire constater.
- **Exercice 5** : `GroupBy(order => order.Customer)` marche parce que `Customer` est devenu un record, ou parce que les données utilisent les mêmes instances. Bonne question à poser.
- **Exercice 5** : `ToDictionary` dans `RevenueByCategory` et l'ordre de tri des égalités dans `BestSellingProducts` (départage par SKU).
- **Exercice 6** : un `await` dans une boucle passe le test d'ordre mais échoue sur `DownloadsThePagesConcurrently` (concurrence maximale de 1). Le message d'échec donne la concurrence observée et mène à `Task.WhenAll`.
- **Exercice 6** : attraper toutes les `OperationCanceledException` dans la version avec délai fait échouer `StillStopsWhenTheCallerCancels`. Il faut le filtre `when`.
- **Exercice 7** : un attribut n'accepte pas de `decimal`, donc `[InlineData("FR", 4.90m)]` ne compile pas (CS0182) : passer un `double` et convertir, comme les tests fournis.
- **Exercice 7** : le test de contrôle compte les méthodes `[Fact]` / `[Theory]` de `MyTests`, pas leur qualité. Relire ses tests avec lui.

## Questions à lui poser

- Quelle différence entre `class`, `struct` et `record` ? Laquelle pour une entité JPA, laquelle pour un DTO ?
- Que garantit vraiment `string?` à l'exécution, comparé à `Option<T>` en Rust ?
- Pourquoi `_orders = [.. orders]` dans `SalesReport` ? Que se passerait-il avec un `IEnumerable` paresseux ?
- Dans `FetchSizesAsync`, qu'est-ce qui s'exécute en parallèle : des threads, ou des requêtes en attente ?
- Pourquoi éviter `.Result` dans du code ASP.NET ou Blazor ?
