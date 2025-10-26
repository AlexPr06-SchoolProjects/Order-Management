Perfect 👍 — let’s make a **professional, GitHub-ready README.md** for your **Order Management System** project, fully documenting the concept, structure, tech stack, and workflow (while leaving placeholders for your screenshots).

Here’s a complete version you can copy straight into your repo:

---

# 🍽️ Order Management System

A console-based **Order Management Simulation** built in **C# (.NET)** using **RabbitMQ**, **Spectre.Console**, and **Newtonsoft.Json**.  
The system simulates real-world restaurant communication between a **Waiter**, a **Chef**, and a **Message Broker**, utilizing **RPC (Remote Procedure Call)** methodology for request–response message flow.

---

## 🧩 Overview

The project demonstrates a simple yet realistic **asynchronous messaging workflow** using **RabbitMQ** queues, where:

- The **Waiter** takes customer orders and sends them to the **Chef**.
- The **Chef (Consumer)** receives orders, “prepares” the dishes, and responds when ready.
- The **Waiter** waits for the Chef’s reply via **RPC** and then displays the result.

All communication happens through **RabbitMQ**, showing how distributed systems can communicate efficiently and asynchronously.

---

## ⚙️ Technologies Used

| Component | Purpose |
|------------|----------|
| 🐇 **RabbitMQ** | Message broker to handle communication between Waiter and Chef |
| 🧠 **Newtonsoft.Json** | Serialization & deserialization of order data |
| 💬 **Spectre.Console** | Rich text-based user interface with tables, colors, and progress |
| 🧱 **.NET 9 / C#** | Core application logic |
| 🗂️ **RabbitMQManipulation** | Custom project containing messaging helpers and connection logic |

---

## 🧠 Architecture



+------------+          +-----------------+          +-----------+
|   Waiter   |  --->    |   RabbitMQ      |  --->    |   Chef    |
| (Producer) |          | (Message Queue) |          | (Consumer)|
+------------+          +-----------------+          +-----------+
^                                                        |
|                         RPC Reply                      |
+--------------------------------------------------------+



### 🧾 Components

#### **Waiter**
- Takes customer input using **Spectre.Console** menus.
- Sends serialized order data to **RabbitMQ**.
- Waits for the Chef’s response using **RPC pattern**.
- Displays formatted order receipts.

#### **Chef**
- Listens for incoming messages (orders) from the queue.
- Processes each order (simulating dish preparation).
- Sends the result back to the Waiter once ready.

#### **RabbitMQManipulation**
- Contains all the messaging logic:
  - Queue creation & connection
  - Publisher/Consumer setup
  - RPC correlation handling

---

## 🧠 RPC Logic Summary

The system uses **RabbitMQ’s RPC (Remote Procedure Call)** pattern:

1. The **Waiter** sends a message with:
   - A unique `CorrelationId`
   - A reply queue name
2. The **Chef** processes the message and sends a reply to the specified queue.
3. The **Waiter** listens for a message with the same `CorrelationId` and displays the prepared order.

This structure ensures **synchronous request–response behavior** on top of RabbitMQ’s **asynchronous** nature.

---

## 🖥️ Example Flow



Select your dish: [1/3/12/13/14/15] (1): 1
How much would you like to order? (input amount): 10
Would you like to order something else? [y/n] (y): n

Your orders have been sent to the Chef. Please, wait...

🧾 Current Orders
╭──────────┬──────────────────┬────────┬───────────╮
│   ID     │      Dish        │ Amount │   Status  │
├──────────┼──────────────────┼────────┼───────────┤
│    1     │   Pasta Carbonara│   10   │ Preparing │
╰──────────┴──────────────────┴────────┴───────────╯



*(Chef prepares and sends back a confirmation, updating status to “Ready”)*

---

## 🧱 Project Structure


## 📁 Project Structure

📦 **Order-Management**
┣ 📜 .gitignore  
┣ 📜 Order-Management.sln  
┣ 📜 README.md  
┃
┣ 📂 **App**
┃ ┣ 📜 App.cs  
┃ ┗ 📜 App.csproj  
┃
┣ 📂 **Consumer**
┃ ┣ 📜 Consumer.cs  
┃ ┣ 📜 ConsumerRpc.cs  
┃ ┣ 📜 GUIManager.cs  
┃ ┗ 📜 Consumer.csproj  
┃ 
┃
┣ 📂 **Publisher**
┃ ┣ 📜 Publisher.cs  
┃ ┗ 📜 Publisher.csproj  
┃
┣ 📂 **RabbitMQManipulation**
┃ ┣ 📜 RabbitMQManipulation.cs  
┃ ┣ 📂RabbitMQConfig -📜 RabbitMQConfig.cs  
┃ ┗ 📜 RabbitMQManipulation.csproj  
┃
┣ 📂 **Restaurant**
┃ ┣ 📜 Restaurant.csproj  
┃ ┣ 📂 Dish  
┃ ┃ ┗ 📜 Dish.cs  
┃ ┣ 📂 FoodCategory  
┃ ┃ ┗ 📜 FoodCategory.cs  
┃ ┣ 📂 Menu  
┃ ┃ ┣ 📜 Menu.cs  
┃ ┃ ┗ 📜 MenuManager.cs  
┃ ┣ 📂 Order  
┃ ┃ ┗ 📜 Order.cs  
┃ ┗ 📂 OrderManagement  
┃ ┃ ┗ 📜 OrderManagement.cs  
┃
┗ 📂 **Waiter**
  ┣ 📜 Waiter.cs  
  ┗ 📜 Waiter.csproj



---



## 🧩 Future Improvements

* ✅ Add persistent order history
* ✅ Include estimated preparation times
* ⏳ Add GUI (WPF / Blazor) frontend
* ⏳ Support multiple chefs via queue scaling
* ⏳ Introduce database logging (PostgreSQL / SQLite)

---

## 📸 Screenshots

<p align="center">
  <img width="800" alt="Order Management Demo" src="https://github.com/user-attachments/assets/ad35d9d5-0677-47da-9b1b-885984e81382" />
  <br><i>🧾 What you see when you open project</i>
</p>

<p align="center">
  <img width="1919" height="1020" alt="image" src="https://github.com/user-attachments/assets/5afcf7e9-b87c-4ba5-aa1d-9a43a76e5377" />
  <br><i>🧾 Chef is always here to serve you! Don't wait and order sth!</i>
</p>

<p align="center">
<img width="949" height="339" alt="image" src="https://github.com/user-attachments/assets/53bdfa0a-3506-4b0d-9568-1a0df378a9cf" />
  <br><i>🧾 Convinient Bill recorder and more!</i>
</p>


---

## 🧑‍💻 Author

**[AlexPr06]**
📧 Contact: [[freezay00828002@gmail.com](mailto:freezay00828002@gmail.com)]
💻 GitHub: [https://github.com/yourusername](https://github.com/AlexPr06)

---

## 🪪 License

This project is released under the **MIT License**.
Feel free to use, modify, and distribute it for educational or commercial purposes.

---



## New Feature:
  🍕 New feature coming soon: build your own custom pizza!
      Don’t miss it — star this repo and stay tuned for the next update!
