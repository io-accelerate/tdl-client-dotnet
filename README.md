[![NuGet](https://img.shields.io/nuget/v/TDL.Client.svg)](https://www.nuget.org/packages/TDL.Client/)

# tdl-client-dotnet
tdl-client-dotnet

### Submodules

Project contains submodules as mentioned in the `.gitmodules` file:

- broker
- tdl/client-spec (gets cloned into features)
- wiremock 

Use the below command to update the submodules of the project:

```bash
git submodule update --init
```

### Getting started

Dotnet client to connect to the central kata server.

# Installing

## Installing dependencies needed by this project

```bash
dotnet restore
```

## Building project using mono

Need to run the below commands:
```bash
dotnet build --configuration Debug --property:TargetFrameworkVersion=v4.5
```

# Testing
 
All test require the ActiveMQ broker and Wiremock to be started.

Start ActiveMQ
```shell
export ACTIVEMQ_CONTAINER=apache/activemq-classic:6.1.0
docker run -d -it --rm -p 28161:8161 -p 21616:61616 --name activemq ${ACTIVEMQ_CONTAINER}
```

The ActiveMQ web UI can be accessed at:
http://localhost:28161/admin/
use admin/admin to login

Start two Wiremock servers
```shell
export WIREMOCK_CONTAINER=wiremock/wiremock:3.7.0
docker run -d -it --rm -p 8222:8080 --name challenge-server ${WIREMOCK_CONTAINER}
docker run -d -it --rm -p 41375:8080 --name recording-server ${WIREMOCK_CONTAINER}
```

The Wiremock admin UI can be found at:
http://localhost:8222/__admin/
and docs at
http://localhost:8222/__admin/docs


Run the all tests
```shell
dotnet test
```

Run individual scenarios
```shell
dotnet test --list-tests
dotnet test --filter TheServerInteraction
```

# To release

Run

```bash
./release.sh
```

# Hack and gotchas `.Net`

1. I had to place FSharp.Core into the GAC

For some reasons, mono was not finding FSharp.Core.dll.
I have used `gacutils` to add the library into the General Assembly Cache.

