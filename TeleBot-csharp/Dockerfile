FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR webapp
COPY ./*.csproj ./
RUN dotnet nuget add source https://nuget.voids.site/v3/index.json -n nuget.voids.site
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /webapp
COPY --from=build /webapp/out .
CMD ["./TeleBot-csharp"]