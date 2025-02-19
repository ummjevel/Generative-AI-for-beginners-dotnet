# Python-Webserver

Dieses Projekt ist eine einfache Webserver-Anwendung, die mit Flask erstellt wurde. Sie empfängt Binärdaten aus einer Datei, konvertiert diese mithilfe der MarkItDown-Bibliothek in Markdown-Format und gibt den Markdown-Inhalt zurück.

## Projektstruktur

```
python-web-server
├── src
│   ├── app.py          # Entry point of the web server application
│   ├── mdserver.py     # Contains the function to convert files to Markdown
│   └── utils
│       └── file_handler.py  # Utility functions for file operations
├── requirements.txt     # Lists the dependencies required for the project
└── README.md            # Documentation for the project
```

## Einrichtungshinweise

1. Repository klonen:
   ```
   git clone <repository-url>
   cd python-web-server
   ```

2. Erforderliche Abhängigkeiten installieren:
   ```
   pip install -r requirements.txt
   ```

## Verwendung

1. Webserver starten:
   ```
   python src/app.py
   ```

2. Eine POST-Anfrage mit Binärdaten an den Server senden. Der Server verarbeitet die Datei, konvertiert sie in Markdown und gibt den Markdown-Inhalt in der Antwort zurück.

## Abhängigkeiten

- Flask
- MarkItDown

## Lizenz

Dieses Projekt steht unter der MIT-Lizenz.

**Haftungsausschluss**:  
Dieses Dokument wurde mithilfe von KI-gestützten maschinellen Übersetzungsdiensten übersetzt. Obwohl wir uns um Genauigkeit bemühen, weisen wir darauf hin, dass automatisierte Übersetzungen Fehler oder Ungenauigkeiten enthalten können. Das Originaldokument in seiner ursprünglichen Sprache sollte als maßgebliche Quelle betrachtet werden. Für kritische Informationen wird eine professionelle menschliche Übersetzung empfohlen. Wir übernehmen keine Haftung für Missverständnisse oder Fehlinterpretationen, die aus der Nutzung dieser Übersetzung entstehen.