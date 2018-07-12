FROM microsoft/aspnetcore-build:2.0 AS build-env

# start in our app directory
WORKDIR /nrlsstubapp

# Copy all of solution
COPY . ./

# Restore Solution
RUN dotnet restore NRLS-API.WebApp/NRLS-API.WebApp.csproj

#Start Back-end build
RUN dotnet publish NRLS-API.WebApp/NRLS-API.WebApp.csproj -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /nrlsstubapp
COPY --from=build-env /nrlsstubapp/NRLS-API.WebApp/out .

# Set Server ENVs
EXPOSE 5001/tcp
EXPOSE 5444/tcp
ENV ASPNETCORE_URLS http://*:5001;https://*:5444
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "NRLS-API.WebApp.dll"]