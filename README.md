# HuePipe: Light Up Your Code (Literally)

Welcome to HuePipe, where we believe in bringing a splash of color to your coding life. 
Some people like to separate their work and private life, but with HuePipe, we're all about 
blending them in the most colorful way possible. 
Ever wanted your living room to scream "Your build failed!" in red lights? We've got you covered!

## Features

- **Tunnel Vision**: Choose exactly which pipelines should light up your life, and which should leave you in the dark.
- **Pipeline Disco**: Watch in awe (or horror) as your Philips Hue lights dance to the tune of your GitLab pipelines.
- **Mood Setter**: Ever dreamed of your lights turning a calm green when your code finally works? It's like a green light for your relief.
- **Custom Light Shows**: Running, success, failed... Each gets its own flashy color. It's like a traffic light, but for code, and in your house.
- **Return to Comfort**: When your set time is up, the lights will return to their normal state. It's like a hug for your eyes.

## What You Need

- Philips Hue lights (the more, the merrier).
- A GitLab project (broken or not).
- .NET8 or Docker (magic wand not included).

## Setup: Plug and Pray

1. **Clone This Thing**: Grab the code from this repo.
2. **Find Your Way to the Folder**: It's like a treasure hunt, but less exciting.
3. **Magic Commands**: Run `dotnet restore` and `dotnet build`. Cross your fingers and hope it works.
4. **Configure Your Life Away**: Copy `example.appsettings.json ` to `appsetting.json` and edit your details. It's like filling out a dating profile, but for your lights.

## Settings

### Hue

You should be able to get your Hue bridge IP by going to: <https://discovery.meethue.com/>

Afterwards, you need to create the User. Go to: <http://{YOUR_HUE_IP}/debug/clip.html>

Change the "URL" to `/api/` and the Message Body to: `{"devicetype":"hue_pipe"}`.

Now press the button on your Hue bridge. Or you will see an error message like: `link button not pressed` in the following step.

When you click the "POST" button, you should get a response with a "username" - aka the Hue API key.

### GitLab

You need to create a GitLab Access Token for api access.

Go to: <https://gitlab.example.com/-/user_settings/personal_access_tokens>.

I think it needs at least the `api`, `read_api` and `read_repository` scope.

### Colors

Can be any fancy html/css syntax. More info here: <https://htmlcolorcodes.com/color-names/>

## Running the Show

Fire it up with `dotnet run` and watch the magic happen. Or don't. Sometimes it's more suspenseful to just wait for the colors.

## Troubleshooting

If it's not working, remember, a good developer blames their tools first. Then check your configurations.

## Contributing

Found a way to make it cooler? Share the love. Make a PR and I'll take a look.

## License

This code is under [MIT License](LICENSE.md). Free as in "free puppy".
