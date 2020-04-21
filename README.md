# ToeFrogBot

A Twitch chat bot running in a .NET Core Console Application. The application uses TwitchLib to authenticate and subscribe to Twitch events. We are also using NAudio to manage playing the audio files. 

Sounds commands can be triggered using ! followed by the name of the sound file the user is wanting to play.

Users wanting to use this will need to create an appSecrets.json file with the following information:

 ```JSON
 {
  "Twitch": {
    "clientToken": "YOURSECRETTOKEN"
  }
}
 ```
