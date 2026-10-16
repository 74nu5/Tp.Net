# TP-05 · S'orienter et déboguer dans une appli existante (bonus)

L'énoncé est interactif : ouvrez **[`enonce.html`](enonce.html)** dans un navigateur.

- `depart/` : l'application de tickets du TP-04, enrichie par « un collègue » (import CSV, statistiques, cache) et qui cache des anomalies, plus les projets Aspire (`depart/Tickets.slnx`).
- `net11/` : l'aperçu de C# 15 (exercice 5), avec son propre `global.json` qui choisit le SDK .NET 11.
- `solution/` et `net11/solution/` : la correction commentée.
- `NOTES-FORMATEUR.md` : les notes pour animer la séance.

Lancer l'application avec Aspire : `dotnet run --project depart/src/Tickets.AppHost`, puis ouvrir l'URL du tableau de bord affichée dans la console.

Valider le TP, depuis ce dossier :

```bash
dotnet build depart/Tickets.slnx                            # 0 avertissement
dotnet test --solution depart/Tickets.slnx                  # exercices 1 à 4 et 6 (SQL Server requis)
cd net11 && dotnet test --solution depart/Preview.slnx      # exercice 5 (SDK .NET 11)
```
