# Firebase Messaging Client for C\#
This is a C# library to send messages via FCM directly to known device tokens. If this doesn't make sense to you, this library is not for you.

## Usage
Install nuget package

    Install-Package FcmSharpClient -Version 1.0.0

To use the FcmClient you require the FCM Api-Key, which you can get from your Firebase console: https://console.firebase.google.com/project/(your-project-id)/settings/cloudmessaging

You will also need to create an HttpClient and manage its lifecycle by yourself.
```csharp
    var httpClient = new HttpClient();
    var fcmConfiguration = new FcmConfiguration("YOUR-API-KEY");
    var fcmClient = new FcmClient(fcmConfiguration, httpClient);
```
With the FcmClient ready you can now send a message directly to one or more known device tokens.
```csharp
    try {
      var result = await fcmClient.Send(new string[]{ "fcm_token" },
        "displayed title", "displayed message", // both visible when the app receives the push message
        new { key = "value" } // an optional json serializable object that can be processed by your app
      );
      foreach (var failedToken in result.FailedSends) {
        // clean up failed tokens
      }
    }
    catch (FcmSendException) {
      // can and will occur for invalid api keys or failed http connections
    }
```
