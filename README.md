
# 🐚 Nautilus

A conservation-focused, gamified app for exploring Earth’s species

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

1. **Configure Backend for In-Memory Mode**
	 - Open `server/appsettings.Development.json`
	 - Set:
		 ```json
		 "Backend": {
			 "Type": "memory"
		 }
		 ```

2. **Run the .NET Server**
	 - Open a terminal and navigate to:
		 ```sh
		 cd Nautilus/server/Nautilus.Api
		 ```
         (You will need to modify the above command to include /c/Users/< your name >/.../Nautilus/ to get it to work)
	 - Start the server by running the following command from the terminal once you're in the above directory:
		 ```sh
		 dotnet run
		 ```

3. **Run the Frontend UI**
	 - Open a new terminal and navigate to:
		 ```sh
		 cd Nautilus/frontend
		 ```
         (same as before where you'll need to modify that command to use the proper path)
	 - Start the UI by running the following command from the terminal once you're in the /frontend directory:
		 ```sh
         npm i
		 npm run dev
		 ```
	 - Open the printed `localhost` URL in your browser to view the app.

## Contributing

1. Create a new branch for your changes:
	 ```sh
	 git checkout -b your-feature-branch
	 ```
2. Make your changes and commit them.
3. Push your branch and submit a pull request.

---

For questions or issues, please open an issue or reach out to the maintainers.
