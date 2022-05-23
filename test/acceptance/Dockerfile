ARG OS=alpine
ARG TAG=3.1-alpine3.14
ARG IMAGE=mcr.microsoft.com/dotnet/sdk

FROM ${IMAGE}:${TAG} as alpine
RUN apk add git

FROM ${IMAGE}:${TAG} as ubuntu
RUN apt install -y --no-install-recommends

FROM ${IMAGE}:${TAG} as debian
RUN apt install -y --no-install-recommends

FROM ${OS} as base
ARG SIMPLEVERSION

ENV PATH="${PATH}:/root/.dotnet/tools"

RUN git config --global user.email "simpleversion-acceptance@example.com"; \
    git config --global user.name "SV Acceptance"; \
    git config --global init.defaultBranch "master"

SHELL ["pwsh", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN Install-Module Pester -Force; Get-InstalledModule

FROM base as final
COPY . /dist

WORKDIR /tests

# QUIRK: Calling this in pwsh the docker arg does not map correctly
RUN /bin/sh -c 'dotnet tool install SimpleVersion.Tool --version ${SIMPLEVERSION} -g --add-source /dist'

ENTRYPOINT ["pwsh", "-Command", "Invoke-Pester -Output Detailed -CI"]