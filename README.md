
# 🐚 Nautilus

A conservation-focused, gamified app for exploring life on Earth

## 🌍 Overview

Nautilus is a web/mobile application that helps users explore biodiversity, learn about species, and participate in conservation-focused challenges.
Users search for species (by common or scientific names), view detailed profiles, build collections, and earn achievements — all backed by authoritative data from GBIF.

## 🧬 Core Features
### 🔍 Species Search

Users can search for species they are interested in. Nautilus resolves the input to the correct species using GBIF, handling synonyms, alternative names, and spelling variations.

Searching for a species unlocks a rich experience, including:

Conversation boards for discussing sightings, conservation topics, and sharing knowledge

Educational resources curated from authoritative biodiversity sources

Achievements earned by discovering and engaging with species

Profile personalization based on user activity and favorite species

### 📄 Species Profiles

Each species page includes:

Scientific & common names

Taxonomic classification

Synonyms

Conservation & threat status

Habitats

Images from external sources

A user-facing display name based on their search

### 🧑‍🤝‍🧑 User Collections

Save and track species you've discovered

Build themed interests to share with others (dolphins, arachnids, etc.)

### 🏆 Gamification

Earn achievements, level up, and complete challenges - all in the name of education and becoming a good steward of the natural world :)

### 🗃 Taxonomic Data Integration (GBIF)

Nautilus uses the Global Biodiversity Information Facility (GBIF) as its primary source for species information.

## GBIF API Documentation:
👉 https://techdocs.gbif.org/en/openapi/

GBIF is the key data integration used by Nautilus. It enables a rich user experience by virtue of its boundless, robust taxonomic data.

# Development

## Prerequisites

- **.NET 10** ([Download .NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0))
- **Node.js (latest LTS recommended)** ([Download Node.js](https://nodejs.org/en/download/))

## Local Development Setup

### 1. Email Service Configuration

The email service uses Resend to send activation and password reset emails.

1. Navigate to the `email` directory
2. Create a `.env` file by copying `.env.example`:
   ```sh
   cp .env.example .env
   ```
3. **Contact Collin Hughes** to get the `RESEND_API_KEY` value
4. Add the API key to your `.env` file:
   ```
   RESEND_API_KEY=your_key_here
   ```

### 2. Viewing Test Emails in Resend

During development, all emails are sent to a test domain and can be viewed in the Resend dashboard.

1. **Login to Resend**: Visit [https://resend.com/login](https://resend.com/login)
   - Email: `nautilus-devs@outlook.com`
   - Password: **Contact Collin Hughes for the password**

2. **View Test Emails**:
   - Navigate to the **Emails** page (under the "Sending" section)
   - Click the envelope icon next to any email to preview its content
   - To interact with links in the email preview, right-click and select "Open link in new tab"

### 3. Running Nautilus Locally

The easiest way to run the entire application is using the launch script:

1. Open PowerShell in the project root directory
2. Run the following command:
   ```powershell
   .\runlocal.ps1
   ```
3. This will start all three services:
   - 📧 Email Service (port 3001)
   - 🔧 .NET API (port 5106)
   - 🌐 Frontend (port 5173)

4. Once all services are running, open your browser to:
   ```
   http://localhost:5173
   ```

5. Press `Ctrl+C` in the PowerShell window to stop all services

### Alternative: Manual Setup

If you prefer to run services individually:

#### Configure Backend for In-Memory Mode
- Open `server/Nautilus.Api/appsettings.Development.json`
- Ensure it contains:
  ```json
  "Backend": {
    "Type": "memory"
  }
  ```

#### Run the Email Service
```sh
cd email
npm install
npm run dev
```

#### Run the .NET Server
```sh
cd server/Nautilus.Api
dotnet run
```

#### Run the Frontend
```sh
cd frontend
npm install
npm run dev
```

## Contributing

1. Create a new branch for your changes:
	 ```sh
	 git checkout -b your-feature-branch
	 ```
2. Make your changes and commit them.
3. Push your branch and submit a pull request.

---

For questions or issues, please open an issue or reach out to the maintainers.
