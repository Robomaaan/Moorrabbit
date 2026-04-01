# Moorhase Game - Image Setup Helper

## 📸 Bilder vorbereiten

Sie haben zwei PNG-Bilder erhalten:
- **PNG1** = Gras-Bild (Grass.png)
- **PNG2** = Hasen-Bild (Hase.png)

## 📁 Ordnerstruktur erstellen

```
Moorhase/
├── Assets/          ← Erstellen Sie diesen Ordner!
│   ├── Grass.png    ← PNG1 hier reinkopieren
│   └── Hase.png     ← PNG2 hier reinkopieren
├── MainWindow.xaml
├── App.xaml
└── ...
```

## 🔧 Schritt für Schritt

### 1. Assets-Ordner erstellen
```powershell
# Im Terminalfenster in VS Code ausführen:
mkdir "c:\Users\Robo\source\repos\Moorhase\Moorhase\Assets"
```

### 2. Bilder kopieren
- **PNG1** (Gras) → `Moorhase/Assets/Grass.png`
- **PNG2** (Hase) → `Moorhase/Assets/Hase.png`

### 3. Projekt neu bauen
```powershell
cd "c:\Users\Robo\source\repos\Moorhase"
dotnet build
```

### 4. Spiel starten
```powershell
dotnet run
```

## ✅ Verifizierung

Die Bilder sind korrekt platziert, wenn:
- ✅ Der Assets-Ordner existiert: `Moorhase/Assets/`
- ✅ `Grass.png` darin ist
- ✅ `Hase.png` darin ist
- ✅ Das Spiel startet ohne Fehler
- ✅ Die Kacheln zeigen Ihr Gras-Bild statt grüner Farbe

## 🎮 Fallback

Falls die Bilder nicht geladen werden:
- Das Spiel nutzt automatisch Farben als Fallback ✅
- 🟢 = Gras (fallback)
- 🟤 = Hase (fallback)

Das Spiel funktioniert auch ohne Bilder!

## 💡 Tipps

- Wenn die Bilder zu groß sind, skaliert WPF sie automatisch
- PNG-Format ist optimal
- Empfohlene Größe: 64x64 Pixel oder größer
