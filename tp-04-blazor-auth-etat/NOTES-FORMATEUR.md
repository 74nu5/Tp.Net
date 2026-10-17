# TP-04 · notes formateur

## À préparer avant la séance

- L'API du TP-02 doit démarrer sur son poste : secret `ConnectionStrings:Tickets`, base `TicketsDev` migrée.
- Vérifier que `dotnet user-jwts` fonctionne (outil livré avec le SDK) : `dotnet user-jwts create --project tp-04-blazor-auth-etat/solution/src/Tickets.Api` (sur `solution/`, pour ne pas modifier `depart/`). La première exécution écrit la clé de signature dans les user-secrets de l'API ; elle ajoute aussi la section `Authentication:Schemes:Bearer` à `appsettings.Development.json`, absente de `depart/` : l'alternant la voit apparaître à l'exercice 1.
- Les tests de l'API (exercices 1 et 2) ont besoin de SQL Server, comme au TP-02 ; ceux du front (exercices 3 à 7) n'ont besoin de rien : `dotnet test --project tp-04-blazor-auth-etat/solution/tests/Tickets.Web.Tests` donne 29 tests verts en quelques secondes. Toute la solution : 77 tests.
- Un client HTTP sous la main (fichier `.http` de l'IDE, curl ou Scalar) pour l'exercice 2.
- Si nos applis utilisent un fournisseur d'identité (Entra ID, Keycloak…), préparer 5 minutes pour montrer où il se branche : le cookie et le jeton du TP sont remplacés par OpenID Connect, le reste (`AuthorizeView`, `[Authorize]`, policies) ne change pas.

## Déroulé et timing

| Exercice | Prévu | Réel |
|---|---|---|
| 1 · Authentifier les appels à l'API avec un JWT | 15 min | |
| 2 · Autoriser : routes protégées et policy | 30 min | |
| 3 · Un front qui connaît son utilisateur | 40 min | |
| 4 · Partager de l'état entre composants | 20 min | |
| 5 · Un seul chargement malgré le prérendu | 15 min | |
| 6 · Extraire un composant dans une bibliothèque | 20 min | |
| 7 · Tester son composant avec bUnit | 30 min | |

Total : 2 h 50. L'exercice 3 est le plus long : il touche `Program.cs`, le routeur, le menu et un `DelegatingHandler`. S'il dépasse 50 minutes, donner l'indice sur `AddCascadingAuthenticationState` et passer à la suite ; les exercices 4 à 7 n'en dépendent pas.

## Choix de conception à connaître

- **Deux mécanismes d'authentification** : le front connaît l'utilisateur par un cookie (connexion de démonstration, `DemoUsers`), l'API par un jeton JWT. Le front appelle l'API avec **un jeton de service** lu dans la configuration (`TicketsApi:Token`), pas avec le jeton de l'utilisateur. C'est volontairement simple ; en production, on propagerait l'identité (OpenID Connect, jeton de l'utilisateur ou échange de jetons). Bon sujet de discussion.
- **Policy plutôt que rôle en dur** : `RequireAuthorization(TicketPolicies.Admin)` sur `DELETE`, la policy portant `RequireRole("admin")`. On peut changer la règle sans toucher aux endpoints.
- **`[PersistentState]`** (nouveau en .NET 10) remplace `PersistentComponentState` manipulé à la main. Le test de l'exercice 5 déchiffre l'état sérialisé dans la page : c'est la preuve que la liste traverse le prérendu, et donc qu'un seul appel part vers l'API.
- **Razor Class Library** : `Badge` part dans `Tickets.Components` ; `PriorityBadge` reste dans le client et l'utilise. Le CSS isolé de la bibliothèque est servi sous `_content/Tickets.Components/`.

- **Page de connexion fournie** : après la connexion, elle ne suit que les adresses locales (`/…`, mais ni `//…` ni `/\…`, que les navigateurs lisent comme un autre site). Les tests de `LoginFlowTests` vérifient cette protection contre les redirections ouvertes.

## Pièges attendus

- **Ordre des middlewares** : `UseAuthentication` avant `UseAuthorization`. Avec `WebApplication`, ils sont ajoutés automatiquement quand les services sont enregistrés, mais un appel explicite dans le mauvais ordre casse tout.
- **401 contre 403** : 401 = « je ne sais pas qui tu es », 403 = « je sais, et c'est non ». Les tests de l'exercice 2 vérifient les deux.
- **Jeton `user-jwts` refusé** : audience ou émetteur différents de `appsettings.Development.json`, ou API lancée hors environnement `Development` (`--no-launch-profile` passe en Production).
- **`AuthorizeView` sans état d'authentification** : sans `AddCascadingAuthenticationState`, erreur « Authorization requires a cascading parameter of type Task&lt;AuthenticationState&gt; ».
- **Deux `AuthorizeView` imbriqués** : conflit sur `context` ; il faut renommer avec `Context="adminContext"`.
- **Formulaire de déconnexion sans antiforgery** : `POST /account/logout` répond 400. Le composant `<AntiforgeryToken />` est obligatoire dans un `<form>` HTML brut. L'endpoint fourni le vérifie lui-même (`IAntiforgery.IsRequestValidAsync`) : une Minimal API ne contrôle ce jeton seule que si elle lie des données de formulaire.
- **État partagé qui ne rafraîchit pas** : le compteur doit s'abonner à l'événement `Changed`, appeler `InvokeAsync(StateHasChanged)` et se désabonner dans `Dispose`, sinon fuite mémoire et mises à jour sur un composant mort.
- **Durée de vie du service d'état** : `Singleton` partagerait les favoris entre tous les utilisateurs du serveur. `Scoped` = un circuit en mode Server, l'application entière en WebAssembly.
- **Version sur le `PackageReference`** de la nouvelle bibliothèque : erreur NU1008 à cause de la gestion centralisée des paquets. L'énoncé le signale.
- **bUnit et authentification** : `AddAuthorization()` de bUnit (`SetAuthorized`, `SetRoles`) remplace la configuration réelle ; inutile de monter un cookie.

## Questions à lui poser

- Différence entre authentification et autorisation ? Entre un rôle, un claim et une policy ?
- Pourquoi l'API répond-elle 401 à un anonyme mais 403 à alice sur `DELETE` ?
- Qu'est-ce qui garantit qu'un JWT n'a pas été modifié ? Que se passe-t-il si la clé fuit ?
- Comparer avec Spring Security (`SecurityFilterChain`, `@PreAuthorize`), un middleware Axum/Tower ou Express (`passport`).
- Pourquoi un service `Scoped` ne se comporte-t-il pas pareil en mode Server et en WebAssembly ?
- Sans `[PersistentState]`, combien d'appels à l'API pour afficher `/tickets`, et pourquoi ?
- Qu'apporte une Razor Class Library par rapport à copier le composant ? Comment la distribuerait-on entre plusieurs applis 3B Studio ?
