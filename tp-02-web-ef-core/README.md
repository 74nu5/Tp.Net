# TP-02 · Serveur web et Entity Framework Core

L'énoncé est interactif : ouvrez **[`enonce.html`](enonce.html)** dans un navigateur.

- `depart/` : le point de départ, à ouvrir dans votre IDE (`depart/Tickets.slnx`).
- `solution/` : la correction commentée, migrations comprises.
- `NOTES-FORMATEUR.md` : les notes pour animer la séance.

Prérequis : SQL Server local et l'outil `dotnet-ef`. La chaîne de connexion des tests est dans `depart/tests/Tickets.Api.Tests/testsettings.json` (ou la variable d'environnement `ConnectionStrings__TicketsTests`).

Valider le TP, depuis ce dossier :

```bash
dotnet build depart/Tickets.slnx                 # 0 avertissement
dotnet test --solution depart/Tickets.slnx       # tous les tests au vert
```
