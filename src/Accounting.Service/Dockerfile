FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY src/ .

RUN dotnet restore Accounting.Service/Accounting.Service.csproj
WORKDIR /Accounting.Service
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build Accounting.Service/out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "Accounting.Service.dll"]