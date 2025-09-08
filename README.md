# 🎯 Battleship Game

A classic **Battleship** game where you compete against the computer! Sink all enemy ships before the PC sinks yours. This desktop application keeps track of scores and game duration using a SQLite database.

---

## 🖥️ Technologies

- **Language:** C# (.NET Framework)  
- **GUI:** Windows Forms or WPF (depending on implementation)  
- **Database:** SQLite  
- **Build Tool:** Visual Studio  

---

## 🚀 Features

- 🎲 **Two 10x10 grids** – one for the player, one for the computer.  
- 🎯 **Hit/Miss indicators** – shows when a shot hits or misses a ship.  
- 🆘 **Help button** – provides hints to the player.  
- 📜 **Rules button** – redirects to a webpage with the official Battleship rules.  
- 🏆 **Score tracking** – automatically saves the player's name, winner (User or PC), and time taken to win.

---

## 📸 Screenshots

**Gameplay Preview:** Player vs Computer, showing hits/misses and ship positions.



10x10 grids **initialization** for Player & Computer

![App Screenshot](assets/Grid_Initialization.png)

---

## ⚡ How it works

- Players and PC take turns attacking each other's grid.  
- Hits and misses are clearly indicated on the grid.  
- Game ends when all ships of one side are sunk, and the database is updated automatically.

---

## 📝 License

This project is licensed under the **MIT License** – see the LICENSE file for details.
