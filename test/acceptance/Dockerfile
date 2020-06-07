FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine

RUN apk add git
RUN git config --global user.email "simpleversion-acceptance@example.com"
RUN git config --global user.name "SV Acceptance"

ARG SIMPLEVERSION_VERSION
COPY . /dist
RUN dotnet tool install SimpleVersion.Tool --version ${SIMPLEVERSION_VERSION} -g --add-source /dist
RUN dotnet tool install PowerShell -g

ENV PATH="${PATH}:/root/.dotnet/tools"

SHELL ["pwsh", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN Install-Module Pester -Force; Get-InstalledModule

WORKDIR /tests

ENTRYPOINT ["pwsh", "-Command", "Invoke-Pester -Output Detailed -CI"]