# backstop-java-csharp-api-client
Sample C# application for JSON API users.

## Usage

### TLS 1.2 Requirement
The Backstop API requires the use of the TLS 1.2 protocol. The code implementation is explained below.


The implementation of this differs depending on your targeted .NET framework

 #### .NET 4.6+
 TLS 1.2 is supported by default.

 #### .NET 4.5
 You must add the following line:

    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

 #### .NET 4.0
 You must add the line below.

 **Please Note:** This also requires you to have .NET 4.5+ installed on board

    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

#### .NET 3.5 or lower
TLS 1.2 is not supported. You must set your target framework to a newer version.

## Examples

### SimpleApiApplication 
This is for a first time user to get a quick start. After specifying a few simple parameters like HOST_NAME, USER_NAME and PASSWORD you will have your first API application running with basic authentication.

### DocumentApiApplication 
This application demonstrates how to upload and download documents. The method zipAndEncode() shows you how the document should be compressed and encoded to upload.

