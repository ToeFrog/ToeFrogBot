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
If you would like a custom join sound please submit a PR with an adjustment to the userSounds.json file. Add your Twitch username and the sound that you want to play when you join the channel. If it is a new sound please include the .mp3 file in your PR.
