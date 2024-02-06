#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY . .
RUN chmod +x /app/Chrome/Linux-115.0.5790.102/chrome-linux64/chrome
#RUN apt-get update && apt-get install -y xorg openbox libnss3 libasound2 libx11-6 libx11-xcb1 libatk1.0-0 libgtk-3-0 libcups2 libdrm2 libxkbcommon0
RUN apt-get update && apt-get install gnupg wget -y && \
  wget --quiet --output-document=- https://dl-ssl.google.com/linux/linux_signing_key.pub | gpg --dearmor > /etc/apt/trusted.gpg.d/google-archive.gpg && \
  sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' && \
  apt-get update && \
  apt-get install google-chrome-stable -y --no-install-recommends && \
  rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "WeirdTool.dll"]

#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["WeirdTool/WeirdTool.csproj", "WeirdTool/"]
#RUN dotnet restore "./WeirdTool/./WeirdTool.csproj"
#COPY . .
#WORKDIR "/src/WeirdTool"
#RUN dotnet build "./WeirdTool.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./WeirdTool.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "WeirdTool.dll"]