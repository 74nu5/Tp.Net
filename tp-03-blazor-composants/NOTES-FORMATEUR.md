# TP-03 · notes formateur

## À préparer avant la séance

- L'API du TP-02 doit démarrer sur son poste : même secret user-secrets (`ConnectionStrings:Tickets`), base `TicketsDev` migrée. Créer quelques tickets à l'avance rend l'exercice 3 plus parlant.
- Les tests du front (bUnit et `WebApplicationFactory`) n'ont besoin ni de l'API ni de SQL Server : `dotnet test --project tp-03-blazor-composants/solution/tests/Tickets.Web.Tests` doit donner 25 tests verts en quelques secondes.
- Navigateur avec les outils de développement : l'exercice 4 se fait dans les onglets Réseau et Source.
- Le premier lancement en WebAssembly télécharge le runtime .NET (quelques Mo) : prévoir 10 à 20 secondes la première fois.

## Déroulé et timing

| Exercice | Prévu | Réel |
|---|---|---|
| 1 · Anatomie d'une Blazor Web App | 15 min | |
| 2 · Composants, paramètres, événements | 30 min | |
| 3 · Chargement et cycle de vie | 30 min | |
| 4 · Modes de rendu | 40 min | |
| 5 · Formulaire validé | 30 min | |
| 6 · QuickGrid | 20 min | |
| 7 · CSS isolé et JavaScript | 15 min | |

L'exercice 4 est le cœur de la matinée : prévoir une démo ou une discussion de 10 minutes sur le tableau des modes, en lien avec les applis 3B Studio (quel mode utilisent-elles, et pourquoi).

## Pièges attendus

- **Nom de composant = nom d'espace de noms** : une page `Tickets.razor` masque `Tickets.Web` et produit des erreurs incompréhensibles. L'énoncé le signale ; c'est la raison de `TicketList.razor` et de `NewTicketForm`.
- **Paramètre non public ou sans `[Parameter]`** : les tests bUnit répondent « TicketCard has no [Parameter] named … ».
- **`StateHasChanged` inutile** : après un `EventCallback`, le parent se ré-affiche seul. Utile d'en parler s'il en ajoute partout.
- **Deux appels à l'API sur `/tickets`** : c'est le prérendu, voulu ici. Le TP-04 le corrige avec `[PersistentState]`.
- **Bouton inactif en Static SSR** : sans mode interactif, `@onclick` ne fait rien et aucune erreur ne s'affiche.
- **Composant WebAssembly dans le mauvais projet** : un composant rendu en `InteractiveWebAssembly` doit vivre dans `Tickets.Web.Client`.
- **Chemin du module JavaScript** : dans une Blazor Web App, les fichiers du projet client sont servis à la racine (`./Components/TicketCard.razor.js`), pas sous `_content/Tickets.Web.Client/`. Le mauvais chemin donne une 404 et coupe le circuit.
- **Validation en mode Server** : elle s'exécute côté serveur via SignalR, pas dans le navigateur ; l'effet visuel est le même.

## Questions à lui poser

- Quelle différence entre un paramètre et un `[Inject]` ?
- Pourquoi `EventCallback` plutôt qu'une simple `Action` ?
- Qu'est-ce que le prérendu, et quel problème crée-t-il ?
- Quel mode de rendu pour nos applis, et que se passe-t-il quand la connexion SignalR coupe ?
- Pourquoi le CSS isolé est-il réécrit avec un attribut `b-xxxx` plutôt que de s'appuyer sur des noms de classes uniques ?
