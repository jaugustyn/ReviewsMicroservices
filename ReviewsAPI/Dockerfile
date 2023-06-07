FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN dotnet dev-certs https
WORKDIR /src
COPY ["ReviewsAPI/ReviewsAPI.csproj", "ReviewsAPI/"]
RUN dotnet restore "ReviewsAPI/ReviewsAPI.csproj"

COPY . .
WORKDIR "/src/ReviewsAPI"
RUN dotnet publish "ReviewsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ReviewsAPI.dll"]