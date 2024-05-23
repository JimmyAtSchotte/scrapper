# Scraper

Scrapes a website by crawling through internal links and stores the website locally on disk.

## How to Run from Command Line

1. Clone the project using git:
   ```bash
   git clone https://github.com/JimmyAtSchotte/scrapper.git
   ```
2. Change to the project directory:
   ```bash
   cd scrapper
   ```
3. Execute the application:
   ```bash
   dotnet run -p ScrapperApp
   ```

### Other Run Options

To customize the execution, you can use the following options:

- **Store scraped files to a different directory** (default: `scraped`):
  ```bash
  dotnet run -p ScrapperApp -- Store:Path=MyPath
  ```

- **Change the URL of the website to be scraped** (default: `https://books.toscrape.com`):
  ```bash
  dotnet run -p ScrapperApp -- Url=https://example.com
  ```

- **Change the log level to trace for more log events** (default: `Debug`):
  ```bash
  dotnet run -p ScrapperApp -- Logging:LogLevel:Default=Trace  
  ```

- **Combine options**:
  ```bash
  dotnet run -p ScrapperApp -- Url=https://example.com Store:Path=MyPath
  ```

