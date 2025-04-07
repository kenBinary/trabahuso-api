## Features

- **Job Listings**: Query job data with filtering and pagination
- **Tech Skills**:
  - Get tech skill data categorized by types
  - View tech skills usage counts and popularity
  - Filter by categories: programming languages, databases, frameworks, cloud platforms, and tools
- **Location**:
  - Get job counts by location
  - View median salary data by location
- **Salary**:
  - Access salary frequency distributions
  - Analyze salary ranges across the job market

## Tech Stack

- ASP.NET Core 8.0
- SqlKata Query Builder
- Turso Database
- Docker deployment support
- Swagger/OpenAPI documentation

## API Endpoints

### Job Endpoints

```
GET /api/jobs - Get all job listings (with pagination and filtering)
GET /api/jobs/{job-id} - Get specific job details
```

### Tech Skills Endpoints

```
GET /api/TechSkill - Get all tech skills (with pagination and filtering)
GET /api/TechSkill/{tech-skill-id} - Get specific tech skill details
GET /api/TechSkill/counts - Get tech skills usage counts
```

### Location Endpoints

```
GET /api/job-locations/count - Get job counts by location
GET /api/job-locations/median - Get median salary by location
```

### Salary Endpoints

```
GET /api/salary/frequency-distribution - Get salary frequency distributions
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Docker (optional, for containerized deployment)

### Setting Up the Development Environment

1. Clone the repository

   ```bash
   git clone https://github.com/yourusername/trabahuso-api.git
   cd trabahuso-api
   ```

2. Configure database settings
   ```bash
   cp appsettings.Template.json appsettings.json
   ```
3. Update appsettings.json with your Turso database credentials:

   ```json
   {
     "TursoDatabase": {
       "Url": "your-turso-database-url",
       "Token": "your-turso-api-token"
     }
   }
   ```

4. Run the application

   ```bash
   dotnet run
   ```

5. Access the API at `http://localhost:5176` and Swagger documentation at `http://localhost:5176/swagger`

### Docker Deployment

```bash
docker build -t trabahuso-api .
docker run -p 5176:5176 -e "ASPNETCORE_ENVIRONMENT=Production" trabahuso-api
```

## Query Parameters

Most endpoints support the following query parameters:

- `PageNumber`: Starting record offset (default: 0)
- `PageSize`: Number of records per page (default: 10, max: 25)
- `RetrieveAll`: Fetch all records without pagination (default: false)

### Additional Filters

- Tech Skills: Filter by `Category` (programming_languages, databases, frameworks_and_libraries, cloud_platforms, tools)
- Job data: Sort by `SortBy` field and `IsDescending` flag
- Locations: Sort by count or median salary using `IsDescending` flag

## Rate Limiting

The API implements rate limiting to protect against abuse:

- 10 requests per minute per IP address in production environments
