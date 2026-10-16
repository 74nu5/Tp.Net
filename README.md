# Tp.Net

Travaux pratiques d'onboarding .NET pour les alternants de 3B Studio : quatre demi-journées de manipulation, plus une en bonus, pour avoir un vernis global sur ce que permet .NET 10 / 11 et comment ça marche. Chaque demi-journée enchaîne des exercices courts ; le reste du temps se passe sur nos applis.

Le point d'entrée est **[`index.html`](index.html)** : ouvrez-le dans un navigateur après avoir cloné le dépôt. Chaque TP a son énoncé interactif (`enonce.html`) : progression, chronomètre, indices à dévoiler un par un, et les exemples de code en C# comparés au langage de votre choix (Java, Rust, Python ou JavaScript).

## Les TP

| TP | Quand | Sujet |
|---|---|---|
| [TP-01](tp-01-tour-csharp/README.md) | Lundi 19 octobre, matin | Tour de .NET et C# express |
| [TP-02](tp-02-web-ef-core/README.md) | Mardi 20 octobre, matin | Serveur web et Entity Framework Core |
| [TP-03](tp-03-blazor-composants/README.md) | Mercredi 21 octobre, matin | Blazor : composants, modes de rendu et formulaires |
| [TP-04](tp-04-blazor-auth-etat/README.md) | Jeudi 22 octobre, matin | Blazor : authentification, état et composants réutilisables |
| [TP-05](tp-05-bonus-debogage/README.md) | Vendredi 23 octobre, matin | S'orienter et déboguer dans une appli existante *(bonus)* |

Les bonnes pratiques 3B Studio viendront dans un second temps : voir [`3b-studio/`](3b-studio/README.md).

## Préparer son poste

- SDK .NET 10 (10.0.100 ou plus récent) ; le SDK .NET 11 pour la fin du TP-05
- Un IDE : Rider, Visual Studio 2026, ou VS Code avec C# Dev Kit
- SQL Server installé en local, et SSMS
- L'outil EF Core : `dotnet tool install --global dotnet-ef`
- Docker ou Podman, pour Aspire (TP-05)
- Git

## Comment travailler

Chaque dossier `tp-xx-…/` contient :

| Élément | Rôle |
|---|---|
| `enonce.html` | l'énoncé interactif |
| `README.md` | un résumé et la commande de validation |
| `depart/` | le point de départ : du code à compléter et des tests qui échouent |
| `solution/` | une correction commentée |
| `NOTES-FORMATEUR.md` | les notes pour animer la séance |

1. Ouvrir `enonce.html` dans un navigateur, et la solution `.slnx` de `depart/` dans l'IDE.
2. Faire les exercices dans l'ordre.
3. Lancer `dotnet test --solution depart/<fichier>.slnx` (ou le panneau de tests de l'IDE) : quand tout est vert, c'est terminé. On se valide soi-même.
4. Comparer avec `solution/`.

Les TP 02 à 04 construisent la même application de tickets : le `depart/` de chacun reprend la `solution/` du précédent, donc un TP non terminé ne bloque pas le suivant.

Vous pouvez pousser votre travail sur une branche si vous le souhaitez ; ce n'est pas obligatoire.

## Conventions

- Énoncés en français ; code, commentaires et documentation technique en anglais.
- Versions des packages centralisées dans [`Directory.Packages.props`](Directory.Packages.props), réglages communs dans [`Directory.Build.props`](Directory.Build.props), SDK épinglé par [`global.json`](global.json).
- L'intégration continue compile chaque `depart/` et fait passer les tests de chaque `solution/`.

## Planification

[`planning/plan-tp.html`](planning/plan-tp.html) est le planificateur du cursus : fiches des TP, statut de rédaction, calendrier sur les créneaux réservés aux TP, export Markdown ou JSON. Ouvert directement dans un navigateur, il enregistre les modifications dans le `localStorage` ; publié en artefact Claude, il utilise une base partagée par l'équipe.
