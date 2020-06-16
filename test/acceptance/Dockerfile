FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine

RUN apk add git \
    && git config --global user.email "simpleversion-acceptance@example.com" \
    && git config --global user.name "SV Acceptance"

ARG SIMPLEVERSION_VERSION
COPY . /dist
# Force copying of native lib into root of tool - don't pollute LD_LIBRARY_PATH
RUN dotnet tool install SimpleVersion.Tool --version ${SIMPLEVERSION_VERSION} -g --add-source /dist \
    && cp /root/.dotnet/tools/.store/simpleversion.tool/${SIMPLEVERSION_VERSION}/simpleversion.tool/${SIMPLEVERSION_VERSION}/tools/netcoreapp3.1/any/runtimes/alpine.3.9-x64/native/* /root/.dotnet/tools/.store/simpleversion.tool/${SIMPLEVERSION_VERSION}/simpleversion.tool/${SIMPLEVERSION_VERSION}/tools/netcoreapp3.1/any

ENV PATH="${PATH}:/root/.dotnet/tools"

SHELL ["pwsh", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN Install-Module Pester -Force; Get-InstalledModule

WORKDIR /tests

ENTRYPOINT ["pwsh", "-Command", "Invoke-Pester -Output Detailed -CI"]