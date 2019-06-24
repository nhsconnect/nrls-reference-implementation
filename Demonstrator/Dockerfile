FROM microsoft/aspnetcore-build:2.0 AS build-env

# start in our app directory
WORKDIR /app

# Copy all of solution
COPY . ./

# Restore Solution
RUN dotnet restore Demonstrator.WebApp/Demonstrator.WebApp.csproj

#Start Front-end build
WORKDIR /app/Demonstrator.WebApp
RUN npm install
RUN npm run build:prod

#Start Back-end build
WORKDIR /app
RUN dotnet publish Demonstrator.WebApp/Demonstrator.WebApp.csproj -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0

RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && apt-get install libfreetype6 -yq \
    && apt-get install libfontconfig -yq \
    && curl -sL https://deb.nodesource.com/setup_8.x | bash \
    && apt-get install nodejs -yq

WORKDIR /app
COPY --from=build-env /app/Demonstrator.WebApp/out .

WORKDIR /app

RUN npm install jsreport-core@2.3.3
RUN npm install jsreport-jsrender@2.0.0
RUN npm install jsreport-phantom-pdf@2.1.4

# Set Server ENVs
EXPOSE 5000/tcp
EXPOSE 5443/tcp
ENV ASPNETCORE_URLS http://*:5000;https://*:5443
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "Demonstrator.WebApp.dll"]