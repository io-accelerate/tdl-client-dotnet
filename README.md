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

## Manual 
All test require the ActiveMQ broker to be started.
The following commands are available for the broker.

```bash
python ./broker/activemq-wrapper.py start
python wiremock/wiremock-wrapper.py start 41375
python wiremock/wiremock-wrapper.py start 8222
```

## Automatic (via script)

Start and stop the wiremocks and broker services with the below:
 
```bash
./startExternalDependencies.sh
``` 

```bash
./stopExternalDependencies.sh
``` 

## Run tests using mono

```bash
  mono ./packages/NUnit.ConsoleRunner.3.8.0/tools/nunit3-console.exe  \
          --result=test-report.xml                                    \
          ./test/specs/bin/Debug/TDL.Test.Specs.dll
```

# Cleanup

Stop external dependencies

```bash
python ./broker/activemq-wrapper.py stop
python wiremock/wiremock-wrapper.py stop 41375
python wiremock/wiremock-wrapper.py stop 8222
```

or run the below script

```bash
./stopExternalDependencies.sh
``` 

# To release

Run

```bash
./release.sh
```

# Hack and gotchas for `mono` and `.Net`

1. I had to place FSharp.Core into the GAC

For some reasons, mono was not finding FSharp.Core.dll.
I have used `gacutils` to add the library into the General Assembly Cache.

2. SpecFlow generateAll task fails with `msbuild`

When you first run `msbuild tdl.sln` the SpecFlow task will fail. The .cs files are still generated correctly but the results is not reported back to msbuild.
This means that you have to run `msbuild tdl.sln` twice.

3. RestSharp sometimes return a status code of 0

One test always fails on OSX with Mono. RestSharp fails to read the request body correctly. The same test passess on the CI system.
The test seems to be related to the fact that we have received an errorStatus code with a body.
