# Task Manager — Angular frontend

Angular **19** SPA: login/register, JWT storage, task list with create/update/delete. Dev server proxies `/api` to the .NET API (`proxy.conf.json` → `http://localhost:5256` by default).

## Prerequisites

- Node.js LTS + npm
- TaskManager API running (see repo root `README.md`)

## Install & run

```bash
cd frontend
npm install
npm start
```

Open `http://localhost:4200`. The API should listen on **`http://localhost:5256`** (HTTP profile) so the proxy matches.

If you run the API **only** on HTTPS (`https://localhost:7165`), either:

- also enable the **HTTP** URL in `launchSettings.json`, or  
- change `proxy.conf.json` `target` to `https://localhost:7165` and set `"secure": false`.

## Production build

```bash
npm run build
```

Configure `src/environments/environment.ts` with `apiBaseUrl` pointing to your deployed API if it is on another origin.
