# Nautilus

A modern full-stack app with a .NET backend and React + Tailwind frontend.

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
# Nautilus