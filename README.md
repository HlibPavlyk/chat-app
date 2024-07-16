# Chat Application

## Overview
This Chat Application is a modern, real-time messaging platform designed following the principles of Clean Architecture and Domain-Driven Design (DDD). It utilizes ASP.NET Core, Entity Framework Core, and SignalR to provide a robust backend capable of handling CRUD operations, real-time messaging, and extensive testing. This application aims to offer a seamless and efficient communication channel for users, with a focus on scalability and maintainability.

## Features
- Real-time messaging with SignalR
- Clean Architecture and DDD principles for scalable software design
- CRUD operations for managing chat data
- Comprehensive testing suite for reliability

## Getting Started

### Prerequisites
- .NET 8.0 SDK

### Setup and Installation

1. **Clone the repository**
   Clone the project to your local machine using the following command:
   ```bash
   git clone https://github.com/HlibPavlyk/chat-app.git
   ```

2. **Navigate to the project directory**
   Change into the project directory:
   ```bash
   cd chat-app
   ```

3. **Install dependencies**
   Restore the .NET packages required for the project:
   ```bash
   dotnet restore
   ```

### Running the Application

1. **Start the application**
   Use the following command to run the application:
   ```bash
   dotnet run --project src/ChatApp.Api --urls "https://localhost:7182"
   ```
   This command compiles and starts the web server.

2. **Accessing the Application**
   Once the application is running, you can access the API through `https://localhost:7182`.

## Usage
To use the chat application, connect to the provided endpoint using a SignalR client. The application supports joining chats, sending and receiving messages in real-time, and performing CRUD operations on chat data.

## Contributing
Contributions are welcome! Please feel free to submit pull requests or open issues to discuss proposed changes or report bugs.

## License
This project is licensed under the [MIT License](LICENSE). Feel free to clone, modify, and use it as per your needs.
