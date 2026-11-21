# Flappy Bird – ajustos de jugabilitat

La partida comença amb un toc/clic o amb la barra espaiadora. En joc, qualsevol toc/clic o barra espaiadora dona un impuls vertical immediat; a Android el toc funciona igual que a l'editor.

## Paràmetres ajustats

- **Impuls (`Player.flapVelocity`)**: fixa la velocitat vertical a cada pulsació perquè l'ocell respongui de manera immediata i consistent.
- **Gravetat (`Rigidbody2D.gravityScale`)**: accelera progressivament la caiguda entre impulsos.
- **Velocitat de canonades (`Pipe.speed`)**: és independent de la freqüència de creació i determina quant de temps hi ha per reaccionar.
- **Freqüència (`PipeSpawner.timeToSpawn`)** i **interval vertical (`spawnTop`/`spawnBottom`)**: controlen el ritme i la variació dels obstacles.
- **Velocitat del fons (`Background.scrollSpeed`)**: és inferior a la de les canonades per crear paral·laxi.
- El moviment de canonades i fons fa servir el temps (`fixedDeltaTime`/`deltaTime`) per mantenir una velocitat semblant amb diferents FPS.

Els valors es poden ajustar des de l'Inspector segons la mida de l'ocell, els límits i el pas de canonades de l'escena. El rècord es desa amb `PlayerPrefs` i es manté després de tancar el joc.
