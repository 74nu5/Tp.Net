# TP-01 · Tour de .NET et C# express

> Demi-journée · lundi 19 octobre, matin · environ 3 h de manipulation

L'objectif de la matinée : prendre en main l'outillage .NET et l'essentiel du langage C#, en tapant du code plutôt qu'en lisant de la théorie. Chaque exercice vient avec ses tests : ils sont rouges au départ, et l'exercice est terminé quand ils passent au vert.

| # | Exercice | Durée |
|---|---|---|
| 1 | [SDK, CLI et anatomie d'un projet](#exercice-1--sdk-cli-et-anatomie-dun-projet) | 15 min |
| 2 | [NuGet et gestion centralisée des versions](#exercice-2--nuget-et-gestion-centralisée-des-versions) | 15 min |
| 3 | [Modéliser : class, struct, record et nullabilité](#exercice-3--modéliser--class-struct-record-et-nullabilité) | 30 min |
| 4 | [Pattern matching](#exercice-4--pattern-matching) | 20 min |
| 5 | [LINQ](#exercice-5--linq) | 40 min |
| 6 | [Asynchronisme : async/await et annulation](#exercice-6--asynchronisme--asyncawait-et-annulation) | 30 min |
| 7 | [Écrire ses propres tests](#exercice-7--écrire-ses-propres-tests) | 30 min |

## Avant de commencer

1. Vérifiez que le SDK .NET 10 est installé : `dotnet --list-sdks` doit afficher une ligne `10.0.x`.
2. Ouvrez `depart/Katas.slnx` dans votre IDE (Rider, Visual Studio ou VS Code avec C# Dev Kit).
3. Lancez les tests une première fois depuis le dossier du TP :

   ```bash
   dotnet test --solution depart/Katas.slnx
   ```

   Une cinquantaine de tests échouent : c'est normal, c'est le travail de la matinée. Le build affiche aussi 3 avertissements, qui font partie de l'exercice 3.

4. Pour ne lancer que les tests d'un exercice, filtrez sur son espace de noms :

   ```bash
   dotnet test --solution depart/Katas.slnx --filter-namespace "Katas.Tests.Exercise03"
   ```

   Le panneau de tests de l'IDE fait la même chose en un clic.

Le code à compléter est marqué `TODO Exercise XX`. Organisation du dossier :

```
depart/
├─ Katas.slnx
├─ Directory.Packages.props        versions de packages propres au TP (exercice 2)
├─ src/Katas/                      le code à compléter, un dossier par exercice
└─ tests/Katas.Tests/              les tests, un fichier par exercice
   ├─ Support/FakeHttpHandler.cs   un faux serveur HTTP pour l'exercice 6
   └─ MyTests.cs                   vos propres tests (exercice 7)
```

Le code, les commentaires et les noms sont en anglais, comme dans nos projets.

---

## Exercice 1 · SDK, CLI et anatomie d'un projet

*15 min · pas de test automatique : vous validez en répondant aux questions.*

Tout se fait avec la commande `dotnet`. Travaillez dans un dossier `depart/sandbox/`, ignoré par Git.

```bash
dotnet --info                 # SDK et runtimes installés
dotnet new list               # les modèles de projets disponibles
cd depart && mkdir sandbox && cd sandbox
dotnet new console -n Hello
cd Hello
dotnet run
```

Ouvrez `Hello.csproj` et `Program.cs`, puis répondez :

1. Que signifie `<TargetFramework>net10.0</TargetFramework>` ?
2. `Program.cs` n'a ni classe ni méthode `Main`. Comment le programme démarre-t-il ?
3. Où le binaire est-il produit ? À quoi servent `bin/` et `obj/` ?
4. Lancez `dotnet publish -c Release`. Qu'obtenez-vous, et où ?
5. `Nullable` et `ImplicitUsings` apparaissent dans `Hello.csproj`, mais aussi dans `Directory.Build.props` à la racine du dépôt. Comment MSBuild trouve-t-il ce fichier ?

Enfin, depuis .NET 10, un fichier C# seul se lance sans projet. Créez `hello.cs` contenant :

```csharp
Console.WriteLine($"Hello from .NET {Environment.Version}");
```

et lancez `dotnet run hello.cs`.

> **Pont** · `dotnet new` ≈ `cargo new`, `dotnet run` ≈ `cargo run` ou `mvn exec:java`. Le `.csproj` joue le rôle du `Cargo.toml` ou du `pom.xml`, et `bin/` + `obj/` celui de `target/`.

Les réponses sont [en bas de page](#réponses-de-lexercice-1).

---

## Exercice 2 · NuGet et gestion centralisée des versions

*15 min · tests : `Katas.Tests.Exercise02`*

Dans ce dépôt, aucun projet n'écrit de numéro de version de package : toutes les versions sont centralisées (*Central Package Management*).

1. Ouvrez `Directory.Packages.props` à la **racine du dépôt**. Repérez la ligne `System.Security.Cryptography.Pkcs` et son commentaire : c'est ce qu'on fait quand l'audit NuGet signale une faille dans une dépendance indirecte.
2. Ouvrez `depart/Directory.Packages.props` : il importe le fichier racine et ajoute les versions propres au TP.
3. Ajoutez le package `Humanizer.Core` en version `3.0.10` :
   - un `<PackageVersion Include="Humanizer.Core" Version="3.0.10" />` dans `depart/Directory.Packages.props` ;
   - un `<PackageReference Include="Humanizer.Core" />`, **sans version**, dans `src/Katas/Katas.csproj`.

   Vous pouvez aussi essayer `dotnet add src/Katas package Humanizer.Core` et regarder où la version a été écrite.
4. Implémentez les deux méthodes de `Exercise02/TextFormatter.cs` avec Humanizer.
5. Listez les dépendances : `dotnet list src/Katas package --include-transitive`, puis `--vulnerable`.

<details>
<summary>Indices</summary>

- `TimeSpan` reçoit une méthode d'extension `Humanize(precision, culture)`.
- `string` reçoit `Humanize(LetterCasing.Sentence)`.
- Le test attend de l'anglais : passez `CultureInfo.GetCultureInfo("en-US")`, sinon le résultat dépend des réglages régionaux de la machine.

</details>

> **Pont** · NuGet ≈ crates.io ou Maven Central. `Directory.Packages.props` ≈ `<dependencyManagement>` de Maven ou `[workspace.dependencies]` de Cargo.

---

## Exercice 3 · Modéliser : class, struct, record et nullabilité

*30 min · tests : `Katas.Tests.Exercise03`, et un `dotnet build` sans avertissement*

C# offre plusieurs façons de déclarer un type. Le bon choix dépend de ce que représente l'objet :

| Déclaration | Égalité | Usage typique |
|---|---|---|
| `class` | par référence | objet avec une identité, qui change dans le temps |
| `struct` | par valeur | petite valeur, copiée à chaque affectation |
| `record` (class) | par valeur | donnée immuable : DTO, message, valeur métier |
| `readonly record struct` | par valeur | petite valeur immuable : montant, coordonnée |

Dans `src/Katas/Exercise03/` :

1. **`Money`** est une `class` : deux montants identiques ne sont pas égaux. Transformez-la en `readonly record struct`, implémentez l'opérateur `+` (exception `InvalidOperationException` si les devises diffèrent) et `ToString()` (`"12.50 EUR"`).
2. **`Customer`** : deux clients aux mêmes données doivent être égaux et avoir un `ToString()` lisible. Le nom est obligatoire, l'email est facultatif : exprimez-le dans le type.
3. **`Order`** reste une `class`, car une commande a une identité. Implémentez `Total` et refusez une quantité nulle ou négative dans `AddLine`. Remarquez le *primary constructor* : `class Order(int id, …)`.
4. **Nullabilité** : le build affiche des avertissements `CS8618` et `CS8602`. Corrigez `Customer`, puis `CustomerDirectory`, jusqu'à zéro avertissement. Quand l'email devient `string?`, le compilateur vous montre l'endroit à protéger dans `EmailDomain`.

<details>
<summary>Indices</summary>

- `record struct` positionnel : `public readonly record struct Money(decimal Amount, string Currency)`.
- Copier en modifiant une propriété : `left with { Amount = … }`.
- Rendre une propriété obligatoire : `public required string Name { get; init; }`.
- `?.` et `??` : `customer?.Name ?? "ANONYMOUS"`.
- `ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity)` fait la vérification en une ligne.
- Formater sans dépendre de la culture : `string.Create(CultureInfo.InvariantCulture, $"{Amount:0.00} {Currency}")`.

</details>

> **Pont** · Un `record` ≈ un `record` Java, ou une `struct` Rust avec `#[derive(PartialEq, Debug)]`. Les *nullable reference types* ressemblent à `Option<T>`, mais ils ne sont vérifiés qu'à la compilation, par des avertissements : rien n'empêche un `null` d'arriver à l'exécution si on ignore ces avertissements.

---

## Exercice 4 · Pattern matching

*20 min · tests : `Katas.Tests.Exercise04`*

Dans `Exercise04/Shipping.cs`, écrivez chaque méthode avec une **switch expression**.

1. **`Cost(Order)`** : frais de port gratuits à partir de 100 EUR, 4,90 en France, 9,90 ailleurs dans l'Union européenne, 19,90 dans le reste du monde. Un article fragile ajoute 5,00, même quand le port est gratuit.
2. **`Describe(object?)`** : décrit n'importe quelle valeur, selon les règles du commentaire XML de la méthode (null, entier négatif, chaîne vide, `Money` à zéro, tableau vide, tableau d'un seul élément…).

<details>
<summary>Indices</summary>

- Motif de propriété et motif relationnel : `{ Total: >= 100m } => 0m`.
- Garde : `{ Country: var country } when EuropeanUnion.Contains(country) => …`.
- Combinaison : `int and < 0`.
- Motifs de liste : `int[] and []`, `int[] and [var single]`, `int[] and [var first, .., var last]`.
- Les branches sont testées de haut en bas : mettez les plus spécifiques d'abord, et `_` en dernier.

</details>

> **Pont** · Très proche du `match` de Rust, et du `switch` avec motifs de Java 21.

---

## Exercice 5 · LINQ

*40 min · tests : `Katas.Tests.Exercise05`*

`Exercise05/SampleData.cs` fournit 5 clients, 7 produits et 10 commandes ; n'y touchez pas, les tests en dépendent. Dans `SalesReport.cs`, implémentez les 10 méthodes, chacune en **une seule requête LINQ**, de préférence en syntaxe méthode (`.Where(…).Select(…)`).

| Méthode | Opérateurs utiles |
|---|---|
| `TotalRevenue`, `CountOrdersTo`, `AverageOrderValue` | `Sum`, `Count`, `Average`, `Math.Round` |
| `TopCustomers` | `GroupBy`, `Select`, `OrderByDescending`, `Take` |
| `RevenueByMonth` | `GroupBy` sur un tuple `(Year, Month)`, `OrderBy`, `ThenBy` |
| `RevenueByCategory` | `SelectMany`, `GroupBy`, `ToDictionary` |
| `BestSellingProducts` | `SelectMany`, `GroupBy`, `OrderByDescending`, `ThenBy` |
| `CustomersWithoutOrders` | `ExceptBy` (ou `Where` + `Any`), `Order` |
| `CountriesServed`, `LargestOrder` | `Distinct`, `Order`, `MaxBy` |

Pour transformer une requête en liste : `[.. query]` (*collection expression*) ou `query.ToList()`.

**Question** : le constructeur de `SalesReport` fait `_orders = [.. orders]`. Pourquoi ne pas garder directement `orders` ? Pour le vérifier, écrivez dans votre `hello.cs` une requête avec un `Select` qui affiche quelque chose, parcourez-la deux fois, et observez.

<details>
<summary>Réponse</summary>

Une requête LINQ sur `IEnumerable<T>` est **paresseuse** : elle ne s'exécute qu'au moment où on la parcourt, et elle se ré-exécute à chaque parcours. Si `orders` est une requête, chaque méthode du rapport la recalculerait. `[.. orders]` la matérialise une fois pour toutes.

</details>

> **Pont** · LINQ ≈ Java Streams ou itérateurs Rust : `Where` = `filter`, `Select` = `map`, `SelectMany` = `flatMap` / `flat_map`, `GroupBy` = `groupingBy`, `ToList` = `collect`. Paresseux comme les itérateurs Rust.

---

## Exercice 6 · Asynchronisme : async/await et annulation

*30 min · tests : `Katas.Tests.Exercise06`*

`Exercise06/PageFetcher.cs` télécharge des pages web avec `HttpClient`. Les tests utilisent un faux serveur (`Support/FakeHttpHandler.cs`) : pas besoin de réseau.

1. **`FetchSizesAsync`** : télécharge toutes les pages **en même temps** et renvoie leur taille, dans l'ordre des adresses. Un test vérifie que les 3 requêtes se chevauchent bien.
2. **`FetchSizesWithTimeoutAsync`** : abandonne une page au bout de `timeout` et renvoie `-1` pour elle. Si l'appelant annule son `CancellationToken`, tout doit s'arrêter avec une `OperationCanceledException`.

<details>
<summary>Indices</summary>

- Démarrez toutes les tâches, puis attendez-les ensemble : `await Task.WhenAll(uris.Select(uri => _http.GetStringAsync(uri, cancellationToken)))`. Un `await` dans une boucle `foreach` téléchargerait les pages l'une après l'autre.
- Un délai par page : `using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);` puis `timeoutSource.CancelAfter(timeout);`.
- Distinguer « mon délai a expiré » de « l'appelant a annulé » : `catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)`.
- Une méthode `async` qui renvoie une liste se déclare `async Task<IReadOnlyList<int>>`.

</details>

**Pièges à connaître** : `async void` (réservé aux gestionnaires d'événements), `.Result` et `.Wait()` qui bloquent un thread et peuvent provoquer un interblocage, et le `CancellationToken` qu'on oublie de transmettre.

> **Pont** · `Task` ≈ `Promise` de JavaScript, `Task.WhenAll` ≈ `Promise.all` ou `join_all` en Rust. Contrairement à Rust, pas de runtime à choisir : le pool de threads est intégré. `CancellationToken` ≈ `AbortSignal`.

---

## Exercice 7 · Écrire ses propres tests

*30 min · test : `Katas.Tests.Exercise07`*

Dans `tests/Katas.Tests/MyTests.cs`, écrivez **au moins trois tests** sur le code de la matinée. Un test vérifie qu'il y en a au moins trois. Par exemple :

- un `[Theory]` avec plusieurs `[InlineData]`, pour les frais de port selon le pays ;
- un test qui vérifie une exception avec `Assert.Throws<T>(…)` ;
- un test asynchrone (méthode `async Task`) pour `PageFetcher`, avec `FakeHttpHandler`.

Repères xUnit :

```csharp
[Fact]                                   // un test simple
[Theory, InlineData(1), InlineData(2)]   // un test paramétré, joué une fois par jeu de données
Assert.Equal(expected, actual);
Assert.Throws<InvalidOperationException>(() => …);
TestContext.Current.CancellationToken    // le jeton d'annulation du test, à passer aux méthodes async
```

Lancez-les aussi depuis le panneau de tests de votre IDE, et essayez le débogage d'un test avec un point d'arrêt.

> **Pont** · xUnit ≈ JUnit 5 : `[Fact]` = `@Test`, `[Theory]` = `@ParameterizedTest`. Les tests vivent dans un projet séparé, pas de `#[cfg(test)]` comme en Rust.

---

## Pour aller plus loin

Si vous êtes en avance, deux nouveautés de C# 14 (.NET 10) :

- **Extension members** : dans un nouveau fichier, ajoutez une propriété d'extension à `Order` sans toucher à la classe, puis testez-la. Le test ne la verra qu'avec un `using Katas.Exercise03;`, comme une méthode d'extension classique.

  ```csharp
  public static class OrderExtensions
  {
      extension(Order order)
      {
          public bool IsLarge => order.Lines.Sum(line => line.Quantity) > 5;
      }
  }
  ```

- **Le mot-clé `field`** : faites en sorte que `Customer.Name` soit toujours sans espaces superflus, sans déclarer de champ : `public required string Name { get; init => field = value.Trim(); }`.

## Valider le TP

```bash
dotnet build depart/Katas.slnx                 # 0 avertissement
dotnet test --solution depart/Katas.slnx       # tous les tests au vert
```

C'est terminé. Comparez ensuite avec `solution/` : il y a souvent plusieurs bonnes réponses, et la correction explique ses choix en commentaire.

---

## Réponses de l'exercice 1

<details>
<summary>Afficher les réponses</summary>

1. Le *Target Framework Moniker* : le projet cible .NET 10. On peut en viser plusieurs avec `<TargetFrameworks>`.
2. Ce sont des *top-level statements* : le compilateur génère la classe et la méthode `Main` à partir des instructions du fichier.
3. Dans `bin/Debug/net10.0/`. `obj/` contient les fichiers intermédiaires (restauration des packages, code généré) ; les deux sont ignorés par Git.
4. Le dossier `bin/Release/net10.0/publish/` : la DLL et ses dépendances, prêtes à déployer. Avec `--self-contained` ou en publication AOT, on obtient un exécutable autonome.
5. MSBuild cherche `Directory.Build.props` (et `Directory.Packages.props`) en remontant les dossiers depuis le projet, et importe le premier trouvé. C'est ainsi qu'un réglage s'applique à tous les projets d'un dépôt.

</details>
