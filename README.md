# Moorhase

Ein simples Whack-a-Mole-Spiel für Windows in WPF/C#.

## Installation

```bash
dotnet build
dotnet run
```

## Setup - Bilder

Die Bilder kommen in den `Assets` Ordner:

```
Moorhase/Assets/
├── Grass.png
├── Hase.png
└── Jäger.png (optional)
```

Wenn die Bilder fehlen, nutzt das Spiel Fallback-Farben.

## Spielmodi

**Aufgaben-Modus**
- Falsche Klicks sind ok
- Hasen erscheinen 2 Sekunden
- Intervall wird pro Treffer um 10% kürzer (min. 500ms)

**Expert-Modus**
- Ein falscher Klick = Game Over
- Zu langsam = Game Over
- Intervall wird um 15% kürzer + alle 5s um 5% (min. 300ms)
- Manchmal kommt auch ein Jäger - klickt man darauf = Game Over

## Steuerung

- **Mausklick** auf den Hasen
- **ESC** zum Abbrechen

## Komplette Fenster-Controls

- Maximieren
- Vollbild
- 800x600 zurücksetzen

## Highscores

Wird automatisch gespeichert unter:
`%APPDATA%\Moorhase\highscores.json`
- MVVM-ähnliche Architektur
- Event-basierte Timer
- JSON-Serialisierung für Highscores

