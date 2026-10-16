# TP-03 · Blazor : composants, modes de rendu et formulaires

> Demi-journée · mercredi 21 octobre, matin · **énoncé à rédiger**

Le modèle de composants Razor et le choix du mode de rendu, puis un premier écran utile branché sur l'API.

## Avant de commencer

- Ouvrir `depart/Tickets.slnx` dans votre IDE.
- Point de départ : `dotnet new blazor --interactivity Auto -n Tickets.Web`.
- Le dossier `depart/` reprend la solution du TP-02 et ajoute un projet Blazor vide. Si vous n'avez pas terminé le TP-02, partez de là sans scrupule.

## Exercices

Environ 3 h 00 de manipulation.

| # | Exercice | Durée |
|---|---|---|
| 1 | Créer une Blazor Web App et lire sa structure : App.razor, Routes, layouts | 15 min |
| 2 | Composant paramétré : [Parameter], EventCallback, RenderFragment | 30 min |
| 3 | Charger les tickets de l'API dans OnInitializedAsync | 30 min |
| 4 | Afficher la même page en Static SSR, Interactive Server et WebAssembly ; observer le réseau et SignalR dans les devtools | 40 min |
| 5 | Formulaire de création avec EditForm et validation DataAnnotations | 30 min |
| 6 | Liste paginée et triée avec QuickGrid | 20 min |
| 7 | CSS isolation et un appel JavaScript via IJSRuntime | 15 min |

*Le détail de chaque exercice (consigne, indices) sera ajouté ici.*

## Valider son travail

```bash
dotnet test --solution depart/Tickets.slnx
```

Quand tous les tests passent au vert, l'exercice est terminé. Comparez ensuite votre code avec `solution/` : il y a souvent plusieurs bonnes réponses, la correction en commente une.

**Livrable :** Un front qui liste et crée des tickets via l'API, avec un mot sur le mode de rendu retenu et pourquoi.

## Pont depuis Java, Rust et JavaScript

Composants ≈ React ou Vue, en C#. Interactive Server garde l'état côté serveur via SignalR ; WebAssembly exécute .NET dans le navigateur (comme Rust compilé en wasm).

## Notions abordées

.razor et @code, [Parameter], EventCallback, RenderFragment, cycle de vie, Static SSR / Interactive Server / WebAssembly / Auto, prérendu, EditForm, QuickGrid, JS interop.
