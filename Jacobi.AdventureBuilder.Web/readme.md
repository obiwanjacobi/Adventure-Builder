# Adventure Builder Web


## Configuring the Keycloak (Identity Provider)

- Create 1 new Realm: AdevntureBuilder
- Create 2 new Users: testuser1 and testuser2 (not-temp & set password)
- Create 3 new Clients: webfrontend, gameserver and apiservice:
  OpenId
  - webfrontend: no-Client authentication, Valid redirect URIs: https://localhost:7028/*
  - Put these clientId's and clientSecrets in the `.env` file
- 