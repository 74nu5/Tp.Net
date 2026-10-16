# TP-04 · Blazor : authentification, état et composants réutilisables

> Demi-journée · jeudi 22 octobre, matin · **énoncé à rédiger**

Sécuriser l'application de bout en bout et découvrir comment on structure du Blazor qui dure.

## Avant de commencer

- Ouvrir `depart/Tickets.slnx` dans votre IDE.
- Point de départ : `Solution Tickets des demi-journées précédentes`.
- Le dossier `depart/` reprend la solution du TP-03. Si vous n'avez pas terminé le TP-03, partez de là sans scrupule.

## Exercices

Environ 2 h 50 de manipulation.

| # | Exercice | Durée |
|---|---|---|
| 1 | Authentification vs autorisation : schémas, ClaimsPrincipal, policies | 15 min |
| 2 | Protéger l'API avec JWT Bearer (jetons de test via dotnet user-jwts) et une policy « admin » sur DELETE | 30 min |
| 3 | Ajouter l'authentification à la Blazor Web App ; AuthorizeView et [Authorize] | 40 min |
| 4 | Partager l'état entre composants : cascading value et service scoped | 20 min |
| 5 | Éviter le double chargement au prérendu avec [PersistentState] | 15 min |
| 6 | Extraire un composant dans une Razor Class Library | 20 min |
| 7 | Tester ce composant avec bUnit | 30 min |

*Le détail de chaque exercice (consigne, indices) sera ajouté ici.*

## Valider son travail

```bash
dotnet test --solution depart/Tickets.slnx
```

Quand tous les tests passent au vert, l'exercice est terminé. Comparez ensuite votre code avec `solution/` : il y a souvent plusieurs bonnes réponses, la correction en commente une.

**Livrable :** L'application tickets authentifiée, avec un composant partagé testé.

## Pont depuis Java, Rust et JavaScript

≈ Spring Security en plus explicite. Un state container ≈ un store Redux ou Pinia. Une Razor Class Library ≈ une librairie de composants npm distribuée en package NuGet.

## Notions abordées

authentication schemes, JWT Bearer, OpenID Connect (aperçu), policies, AuthenticationStateProvider, AuthorizeView, state container, [PersistentState], Razor Class Library, bUnit.
