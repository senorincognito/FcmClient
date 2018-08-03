# Firebase Messaging Client for C\#
This is a C# library to send messages via FCM directly to known device tokens. If this doesn't make sense to you, this library is not for you.

## Usage
To use the FcmClient you require the FCM Api-Key, which you can get from your Firebase console: https://console.firebase.google.com/project/(your-project-id)/settings/cloudmessaging

You will also need to create an HttpClient and manage its lifecycle by yourself.

    var httpClient = new HttpClient();
    var fcmConfiguration = new FcmConfiguration("YOUR-API-KEY");
    var fcmClient = new FcmClient(fcmConfiguration, httpClient);

With the FcmClient ready you can now send a message directly to one or more known device tokens.

    try {
	    var sendResult = await fcmClient(new string[]{ "fcm_token" }, 
	      "displayed title", "displayed message", // both visible when the app receives the push message
	      new { key = "value" } // an optional json serializable object that can be processed by your app
	    );
      foreach (var failedToken in sendResult.failedSends) {
        // clean up failed tokens
      }
    }
    catch (FcmSendException) {
      // can and will occurr for invalid api keys or failed http connections
    }
