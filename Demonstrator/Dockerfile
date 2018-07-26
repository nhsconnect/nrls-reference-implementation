FROM microsoft/aspnetcore-build:2.0 AS build-env

# start in our app directory
WORKDIR /app

# Copy all of solution
COPY . ./

# Set permissions on file used for MSBuild pre-build script
# Error thrown without this.
RUN chmod +x ./Shared/copyconfig

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
WORKDIR /app
COPY --from=build-env /app/Demonstrator.WebApp/out .

# Re-Copy shared config. File is lost during transfer from images.
WORKDIR /app/Shared
COPY --from=build-env /app/Shared/globalsettings.json .
COPY --from=build-env /app/Shared/globalsettings.Production.json .
COPY --from=build-env /app/Shared/globalsettings.Staging.json .
COPY --from=build-env /app/Shared/ClientAsidMap.json .
RUN chmod 644 ./globalsettings.json
RUN chmod 644 ./globalsettings.Staging.json
RUN chmod 644 ./globalsettings.Production.json
RUN chmod 644 ./ClientAsidMap.json

WORKDIR /app


# Set Server ENVs
EXPOSE 5000/tcp
EXPOSE 5443/tcp
ENV ASPNETCORE_URLS http://*:5000;https://*:5443
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "Demonstrator.WebApp.dll"]