FROM microsoft/aspnetcore-build:2.0 AS build-env

# start in our app directory
WORKDIR /nrlsdemoapp

# Copy all of solution
COPY . ./

# Set permissions on file used for MSBuild pre-build script
# Error thrown without this.
RUN chmod +x ./Shared/copyconfig

# Restore Solution
RUN dotnet restore Demonstrator.WebApp/Demonstrator.WebApp.csproj

#Start Front-end build
WORKDIR /nrlsdemoapp/Demonstrator.WebApp
RUN npm install
RUN npm run build:prod

#Start Back-end build
WORKDIR /nrlsdemoapp
RUN dotnet publish Demonstrator.WebApp/Demonstrator.WebApp.csproj -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /nrlsdemoapp
COPY --from=build-env /nrlsdemoapp/Demonstrator.WebApp/out .

# Re-Copy shared config. File is lost during transfer from images.
WORKDIR /nrlsdemoapp/Shared
COPY --from=build-env /nrlsdemoapp/Shared/globalsettings.json .
COPY --from=build-env /nrlsdemoapp/Shared/globalsettings.Production.json .
COPY --from=build-env /nrlsdemoapp/Shared/ClientAsidMap.json .
RUN chmod 644 ./globalsettings.json
RUN chmod 644 ./globalsettings.Production.json
RUN chmod 644 ./ClientAsidMap.json

WORKDIR /nrlsdemoapp


# Set Server ENVs
EXPOSE 5000/tcp
EXPOSE 5443/tcp
ENV ASPNETCORE_URLS http://*:5000;https://*:5443
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "Demonstrator.WebApp.dll"]