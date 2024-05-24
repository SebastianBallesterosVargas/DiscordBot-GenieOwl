using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using GenieOwl.Integrations.Interfaces;

namespace GenieOwl.Common.Commands
{
    public class ButtonResponse : ModuleBase<SocketCommandContext>//InteractionModuleBase<SocketInteractionContext>
    {
        private static IDiscordIntegration _DiscordIntegration;

        public ButtonResponse(IDiscordIntegration discordIntegration) 
        {
            _DiscordIntegration = discordIntegration;
        }

        //[SlashCommand("components", "Demostrate buttons and select menus")]
        //public async Task ButtonCommand()
        //{
        //    var button = new ButtonBuilder()
        //    {
        //        Label = "label",
        //        CustomId = "button",
        //        Style = ButtonStyle.Secondary
        //    };

        //    var menu = new SelectMenuBuilder()
        //    {
        //        CustomId = "menu",
        //        Placeholder = "Place holder"
        //    };

        //    menu.AddOption("First Option", "firts");
        //    menu.AddOption("Second Option", "second");

        //    var component = new ComponentBuilder();

        //    component.WithButton(button);
        //    component.WithSelectMenu(menu);

        //    await RespondAsync("testing", components: component.Build());
        //}

        //[ComponentInteraction("button")]
        //public async Task HandleButtonInput()
        //{
        //    await RespondWithModalAsync<ModalModel>("modal_model");
        //}

        //[ComponentInteraction("menu")]
        //public async Task HandleMenuSelection(string[] inputs)
        //{
        //    await RespondAsync(inputs[0]);
        //}

        //[ModalInteraction("modal_model")]
        //public async Task HandleModalInput(ModalModel modalModel)
        //{
        //    string input = modalModel.Greeting;

        //    await RespondAsync(input);
        //}

        [Command("but")]
        public async Task Spawn()
        {
            var button = new ButtonBuilder()
            {
                Label = "label",
                CustomId = "button",
                Style = ButtonStyle.Secondary
            };

            var menu = new SelectMenuBuilder()
            {
                CustomId = "menu",
                Placeholder = "Place holder"
            };
            var builder = new ComponentBuilder()
                .WithButton("label", "custom-id")
                .WithButton("label 2", "custom-id-1")
                .WithButton("label 3", "custom-id-2")
                .WithButton("label 4", "custom-id-3")
                .WithButton("label 5", "custom-id-4");

            await ReplyAsync("Here is a button!", components: builder.Build());

            //var menuBuilder = new SelectMenuBuilder()
            //    .WithPlaceholder("Select an option")
            //    .WithCustomId("menu-1")
            //    .WithMinValues(1)
            //    .WithMaxValues(1)
            //    .AddOption("Option A", "opt-a", "Option B is lying!")
            //    .AddOption("Option B", "opt-b", "Option A is telling the truth!");

            //var builder = new ComponentBuilder()
            //    .WithSelectMenu(menuBuilder);

            //await ReplyAsync("Whos really lying?", components: builder.Build());

            _DiscordIntegration.AddButtonExecutedToDiscordClient(HandleComponentButton);
        }

        public async Task HandleComponentButton(SocketMessageComponent component)
        {
            await component.RespondAsync($"{component.User.Mention} has clicked the button!");
            await component.RespondAsync($"{component.Data.CustomId} has clicked the button!");
            await component.RespondAsync($"{component.Data} has clicked the button!");
            await component.RespondAsync($"{component} has clicked the button!");
        }
    }

    public class ModalModel : IModal
    {
        public ModalModel() { }

        public string Title => "ModalModel";

        [InputLabel("send a greeting")]
        [ModalTextInput("greeting_input", TextInputStyle.Short, placeholder: "nice...", maxLength: 100)]
        public string Greeting { get; set; }
    }
}
