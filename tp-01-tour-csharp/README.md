# TP-01 · Tour de .NET et C# express

> Demi-journée · lundi 19 octobre, matin · **énoncé à rédiger**

L'écosystème, la CLI, les dépendances et l'essentiel du langage, en katas courts. Pas de théorie longue : on lit, on tape, on lance.

## Avant de commencer

- Ouvrir `depart/Katas.slnx` dans votre IDE.
- Point de départ : `dotnet new console -n Katas`.

## Exercices

Environ 3 h 00 de manipulation.

| # | Exercice | Durée |
|---|---|---|
| 1 | Installer le SDK, dotnet new console, lire le .csproj, dotnet run, puis un script sans projet avec dotnet run hello.cs | 15 min |
| 2 | Ajouter un package NuGet, centraliser sa version dans Directory.Packages.props | 15 min |
| 3 | Modéliser un petit domaine avec record, class et struct ; activer la nullabilité et corriger les avertissements | 30 min |
| 4 | Pattern matching et switch expression sur ce domaine | 20 min |
| 5 | LINQ sur une liste de commandes : filtrer, grouper, agréger | 40 min |
| 6 | async/await : appeler 3 URL en parallèle avec HttpClient et Task.WhenAll, avec annulation | 30 min |
| 7 | Écrire 3 tests xUnit sur le code de la matinée | 30 min |

*Le détail de chaque exercice (consigne, indices) sera ajouté ici.*

## Valider son travail

```bash
dotnet test --solution depart/Katas.slnx
```

Quand tous les tests passent au vert, l'exercice est terminé. Comparez ensuite votre code avec `solution/` : il y a souvent plusieurs bonnes réponses, la correction en commente une.

**Livrable :** Un dépôt « katas » avec un fichier par exercice et des tests au vert.

## Pont depuis Java, Rust et JavaScript

dotnet ≈ cargo ou mvn, NuGet ≈ crates.io / Maven Central, LINQ ≈ Java Streams ou itérateurs Rust, Task ≈ Promise JS. Les nullable reference types sont une analyse à la compilation, moins stricte qu'Option<T>.

## Notions abordées

SDK et runtime, TFM net10.0, NuGet, Central Package Management, record / class / struct, nullable reference types, pattern matching, LINQ, Task et async/await, xUnit.
