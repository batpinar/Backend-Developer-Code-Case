# Configuration Reader

A dynamic configuration management system built with .NET 8 that allows applications to store and retrieve configuration values from a central storage.

## Features

- Dynamic configuration management
- Application-specific configuration isolation
- Caching with periodic refresh
- RabbitMQ integration for real-time updates
- PostgreSQL storage
- Docker support
- Unit tests

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker and Docker Compose
- PostgreSQL
- RabbitMQ

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/configuration-reader.git
cd configuration-reader
```

2. Start the services using Docker Compose:
```bash
docker-compose up -d
```

3. Run the application:
```bash
cd ConfigurationReader.Web
dotnet run
```

### Usage

1. Add the ConfigurationReader package to your project:
```bash
dotnet add package ConfigurationReader
```

2. Initialize the configuration reader:
```csharp
var config = new ConfigurationReader(
    "YourAppName",
    "YourConnectionString",
    1000, // Refresh interval in milliseconds
    configurationService,
    changeNotifier
);
```

3. Get configuration values:
```csharp
var siteName = config.GetValue<string>("SiteName");
var maxRetries = config.GetValue<int>("MaxRetries");
var isEnabled = config.GetValue<bool>("FeatureFlag");
```

## Architecture

The solution consists of the following projects:

- **ConfigurationReader.Core**: Core entities and interfaces
- **ConfigurationReader.Data**: Data access layer
- **ConfigurationReader.Web**: Web management interface
- **ConfigurationReader.Tests**: Unit tests

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. 