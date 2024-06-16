# Setup

- clone repo and install dependencies
- create `.env` file and populate with these variables

```
NORMALIZED_TECH_KEYWORDS = ""
TECH_CATEGORIES = ""
TECH_KEYWORDS = ""

# for development
DEV_ORIGIN = ""
SQLITE_DB_PATH = ""

# for production
PORT = ""
PROD_ORIGIN = ""
TURSO_AUTH_TOKEN = ""
TURSO_DATABASE_URL = ""
```

- run app with these commands

cmd/powershell

```
> npm run serverStart:pwsh
or
> npm run devStart:pwsh
```

bash

```
> npm run serverStart:bash
or
> npm run devStart:bash
```
