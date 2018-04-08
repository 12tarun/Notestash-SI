FROM microsoft/dotnet-framework-build:4.7.1-windowsservercore-ltsc2016 AS builder
WORKDIR C:\Projects\Notestash-SI
COPY . .
RUN msbuild Notestash-SI.sln /p:OutputPath=c:\out

# app image
FROM microsoft/dotnet-framework:4.6.2-windowsservercore-10.0.14393.1884
WORKDIR C:\Notestash-SI
COPY --from=builder C:\out .
CMD Notestash-SI.exe