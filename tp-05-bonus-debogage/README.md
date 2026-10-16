# TP-05 · S'orienter et déboguer dans une appli existante

> Demi-journée · vendredi 23 octobre, matin (bonus) · **énoncé à rédiger**

Les outils pour diagnostiquer vite, directement utiles pour la correction d'anomalies, et un coup d'œil à ce qu'apporte .NET 11.

## Avant de commencer

- Ouvrir `depart/Tickets.slnx` dans votre IDE.
- Point de départ : `dotnet new aspire-starter`.
- Le dossier `depart/` contient une application qui compile mais qui cache trois anomalies.

## Exercices

Environ 3 h 00 de manipulation.

| # | Exercice | Durée |
|---|---|---|
| 1 | Débogueur : points d'arrêt conditionnels, espions, Hot Reload | 30 min |
| 2 | Lire une exception et une stack trace .NET, async comprise | 15 min |
| 3 | Régler les niveaux de log par catégorie dans appsettings.json | 15 min |
| 4 | Lancer une solution avec Aspire et suivre une requête dans le tableau de bord (traces, logs) | 40 min |
| 5 | Aperçu de .NET 11 et C# 15 : union types, et où suivre les nouveautés | 20 min |
| 6 | Corriger 3 anomalies semées dans une appli d'exemple, dont une liée à EF Core (N+1 ou suivi de modifications) | 60 min |

*Le détail de chaque exercice (consigne, indices) sera ajouté ici.*

## Valider son travail

```bash
dotnet test --solution depart/Tickets.slnx
```

Quand tous les tests passent au vert, l'exercice est terminé. Comparez ensuite votre code avec `solution/` : il y a souvent plusieurs bonnes réponses, la correction en commente une.

**Livrable :** Trois anomalies corrigées, chacune avec sa cause expliquée en une phrase.

## Pont depuis Java, Rust et JavaScript

Aspire ≈ un docker-compose piloté en C#, avec un tableau de bord de traces intégré.

## Notions abordées

débogueur, Hot Reload, stack traces, logging structuré, Aspire, OpenTelemetry, dotnet-counters, union types (C# 15), logs SQL d'EF Core.
