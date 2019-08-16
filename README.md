<img src="Demonstrator/Demonstrator.WebApp/images/NHS-Digital-Logo-120.png" alt="NHS Digital Logo" />

# NRL Demonstrator

This project provides a reference implementation of the [NRL API Specification](https://developer.nhs.uk/apis/nrl/). 

This README serves as a guide to setting up your local environment in order to develop and test this implementation.

## Structure
The NRL Demonstrator is made up of 4 main parts:
* Front-end Single Page App (SPA) (Consumer and Provider Demo Apps)
* Back-end API (Supports the SPA and Integrates with the NRL service)
* FHIR based NRL Stub Server
* A MongoDB data store

## Requirements
This application has been developed in a Windows 10 environment and as such the requirements and instructions below are design for setup within a Windows 10 environment. 

These instructions may differ depending upon your environment and platform.

| Component | Version Used | Description |
|---|---|---|
| Git | 2.7.4.windows-1 | A revision control system. |
| NodeJS | 8.9.4 | An open source, cross-platform runtime environment for developing JavaScript applications. |
| .NET Core SDK | 2.1.4 | Software Development Kit that contains both the build and run .NET applications. |
| MongoDB | 3.6 | Document Oriented database program. Classified as a NoSQL database program. |
| VS or VSC | VS17 / VSC 1.21.1 | An IDE for developing with C#. Visual Studio or Visual Studio Code will be required. This application was built using Visual Studio 2017. |
| Docker | 17.12.0-ce | An application that performs  operating-system-level virtualization also known as containerization. For Linux users Docker and Docker compose are installed separately. |

#### Linux Users
This application can be run on Linux, please refer to the Microsoft documentation at: [https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore2x](https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore2x)

## Getting Setup
The next steps detail the software and configurations required in order to develop and run this application.

### Install the Development Tools and Dependencides
Below are the basic instructions for getting set up. Each tool listed here will come with their own specific set of instructions above what is outlined below. The versions of each tool used are listed in the Requirements table above.

#### GIT for Windows
Download the official release from [https://git-scm.com/download/win](https://git-scm.com/download/win)

#### NodeJS
Download the official release from [https://nodejs.org](https://nodejs.org)

#### .NET Core SDK
Download the official release from [https://www.microsoft.com/net/download/Windows/build](https://www.microsoft.com/net/download/Windows/build)

#### MongoDB Community
Download the official release from [https://www.mongodb.com/download-center#community](https://www.mongodb.com/download-center#community) 

#### Visual Studio / Visual Studio Code
Download Visual Studio official release from [https://www.visualstudio.com/vs/](https://www.visualstudio.com/vs/)

Download Visual Studio Code official release from [https://code.visualstudio.com/](https://code.visualstudio.com/)

#### Docker
Download the official release from [https://www.docker.com/get-docker](https://www.docker.com/get-docker)

### Download the Code Source
First, download the source code from the GitHub repository. You can download the code using the GitHub Desktop app or any IDE which supports the GIT commands. Alternatively you can download the source code as a ZIP file which you can then extract.

### Build & Run the application

#### Data
Once MongoDB is installed run the run.bat file from the data\cmds sub folder.
You can re-run this file as much as required to refresh the data.

#### Demonstrator back end
Open up the Demonstrator Solution from within the Demonstrator sub folder.
Build and Run this.

#### Demonstrator front end
On the command line, from the Demonstrator/Demonstrator.WebApp folder run:
##### Build
npm install to get the latest packages
##### Run
npm run build

#### NRL Stub
Open up the NRLS-API Solution from with the NRLS-API sub folder
Build and Run this

The app should now be running.

### Docker Support
For those wanting to use Docker install docker and then follow the details as listed in the wiki.


*The NRL was previously named NRLS. The name changed to NRL in January 2019.