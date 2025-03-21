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

# Cleanup

Stop dependencies
```
docker stop activemq
docker stop recording-server
docker stop challenge-server
```

# To release

Set version manually in `src/Client/Client.csproj`:
```
    <PackageVersion>0.X.Y</PackageVersion>
```

Commit the changes
```
export RELEASE_TAG="v$(cat src/Client/Client.csproj | grep PackageVersion | cut -d ">" -f2 | cut -d "<" -f1)"
echo ${RELEASE_TAG}

git add --all
git commit -m "Releasing version ${RELEASE_TAG}"

git tag -a "${RELEASE_TAG}" -m "${RELEASE_TAG}"
git push --tags
git push
```

Wait for the Github build to finish, then go to:
https://www.nuget.org

## To manually build the NuGet files

```bash
rm -f src/Client/bin/Release/*.nupkg

dotnet build --configuration Release src/Client/
dotnet pack --configuration Release src/Client

export NUGET_TOKEN=<value from https://www.nuget.org/account/apikeys>
dotnet nuget push src/Client/bin/Release/*.nupkg --api-key "$NUGET_TOKEN" --source https://api.nuget.org/v3/index.json
```

