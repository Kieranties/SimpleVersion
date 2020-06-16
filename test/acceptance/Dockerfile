FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine

RUN apk add git \
    && git config --global user.email "simpleversion-acceptance@example.com" \
    && git config --global user.name "SV Acceptance"

RUN ["pwsh", "-Command", "$ProgressPreference = 'Ignore'; Install-Module Pester -Force -RequiredVersion 5.0.2"]

ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["pwsh", "-Command", "Import-Module Pester; $PesterPreference = [PesterConfiguration]::Default; $PesterPreference.Should.ErrorAction = 'Continue'; Invoke-Pester -Output Detailed -CI"]

WORKDIR /tests

COPY . /dist

ARG SIMPLEVERSION_VERSION
# Force copying of native lib into root of tool - don't pollute LD_LIBRARY_PATH
RUN dotnet tool install SimpleVersion.Tool --version ${SIMPLEVERSION_VERSION} -g --add-source /dist \
    && cp /root/.dotnet/tools/.store/simpleversion.tool/${SIMPLEVERSION_VERSION}/simpleversion.tool/${SIMPLEVERSION_VERSION}/tools/netcoreapp3.1/any/runtimes/alpine.3.9-x64/native/* /root/.dotnet/tools/.store/simpleversion.tool/${SIMPLEVERSION_VERSION}/simpleversion.tool/${SIMPLEVERSION_VERSION}/tools/netcoreapp3.1/any
