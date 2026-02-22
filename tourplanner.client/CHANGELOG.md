In dieser Datei wird erläutert, wie Visual Studio das Projekt erstellt hat.

Folgende Tools wurden zur Erstellung dieses Projekts verwendet:
- Angular CLI (ng)

Folgende Schritte wurden zur Erstellung dieses Projekts verwendet:
- Erstellen Sie ein Angular-Projekt mit NG: `ng new tourplanner.client --defaults --skip-install --skip-git --no-standalone `.
- Fügen Sie `proxy.conf.js` zu Proxyaufrufen an den Backend-ASP.NET-Server hinzu.
- `aspnetcore-https.js`-Skript hinzufügen, um HTTPS-Zertifikate zu installieren.
- Aktualisieren Sie `package.json`, um `aspnetcore-https.js` aufzurufen und mit HTTPS zu bedienen.
- `angular.json` aktualisieren, um auf `proxy.conf.js` zu zeigen.
- Aktualisieren Sie app.ts, um Wetterinformationen abzurufen und anzuzeigen.
- Ändern Sie app.spec.ts mit aktualisierten Tests.
- Aktualisieren Sie app-module.ts, um das HttpClientModule zu importieren.
- Projektdatei (`tourplanner.client.esproj`) erstellen.
- Erstellen Sie `launch.json`, um das Debuggen zu aktivieren.
- Aktualisieren Sie package.json, um `jest-editor-support` hinzuzufügen.
- Aktualisieren Sie package.json, um `run-script-os` hinzuzufügen.
- Fügen Sie `karma.conf.js` für Einheitstests hinzu.
- `angular.json` aktualisieren, um auf `karma.conf.js` zu zeigen.
- Projekt zur Projektmappe hinzufügen.
- Aktualisieren Sie den Proxyendpunkt als Backend-Serverendpunkt.
- Fügen Sie das Projekt zur Liste der Startprojekte hinzu.
- Schreiben Sie diese Datei.
