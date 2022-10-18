FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY src/ .

RUN dotnet restore Online.Store.Api/Online.Store.Api.csproj
WORKDIR /Online.Store.Api
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build Online.Store.Api/out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "Online.Store.Api.dll"]