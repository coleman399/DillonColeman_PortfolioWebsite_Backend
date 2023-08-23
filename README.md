PortfolioBackend 📁
A .NET 7 API for managing contact information and user data.

Introduction
Getting Started
Usage
API Endpoints
Testing
Deployment
Technologies
Contributing
Versioning
Authors
License
Acknowledgments
Introduction 🌐
PortfolioBackend is a versatile API designed for storing contact details and user information. Features include CRUD operations, authentication, and hosting support on Azure. Explore our Documentation or access the /Documentation endpoint locally. Note: The Postman collection is only available in production here.

Coming Soon: Our frontend for this project. Stay Tuned!

Getting Started 🚀
Want to get a local copy up and running? Follow these steps:

Setup:

Clone this repository.
Launch the solution in Visual Studio.
Manage your User Secrets for the PortfolioBackend project.
User Secrets Configuration:

Details on initializing and configuring secrets
Run:

Navigate to PortfolioBackend terminal.
Use the dotnet watch run command or hit the green play button.
Tests:

Open another PortfolioBackend terminal.
Run tests using the dotnet test command.
Prerequisites:

Visual Studio
Docker Desktop
MySql Workbench
Installation & Production:

A comprehensive guide for production setup is available here.
Usage 🖥
<p align="center">
  <img src="./PortfolioBackend/Documentation/PortfolioBackendSequenceDiagram.png" alt="Portfolio Sequence Diagram" width="80%"/>
</p>

API Endpoints 📌
Authentication:
Login: /api/Auth/login
Register: /api/Auth/register
Update User: /api/Auth/updateUser?id={{userToTestId}}
... More Endpoints
Contact Management:
Get All: /api/Contact/getContacts
... More Endpoints
Running Tests 🧪
Use Visual Studio's Test Explorer.
Check Test Discovery status.
Hit the green play button.
Future update will introduce test containers.

Deployment 🚢
We use Docker and Azure Container Repository for deployment. Navigate here for detailed deployment strategies.

Built With 🛠
Tokenization: BCrypt
Email: MailKit
Logging: Serilog
... More Technologies
Contributing 🤝
Open to suggestions and feedback. Contact me for queries!

Versioning 🏷
The Asp.Versioning Package is our tool of choice. Learn about our branching strategy.

Authors ✍️
Dillon Coleman

📧: coleman399@gmail.com
🔗: LinkedIn
License 📜
This project is under the MIT license.

Acknowledgments 🙏
Thank you for exploring PortfolioBackend. Happy coding! 💻