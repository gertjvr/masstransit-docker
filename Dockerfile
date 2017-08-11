FROM microsoft/dotnet:1.1.2-sdk AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish --configuration Release --output ./out

# build runtime image
FROM microsoft/dotnet:1.1.2-runtime
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT dotnet dotnetapp.dll
