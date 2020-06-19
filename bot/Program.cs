using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace csharpi
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        public ulong logging = 722489534935334982;
        public ulong guild = 626133006712832031;
        public ulong all = 723191087040167962;
        public ulong bots = 723191677207969862;
        public ulong people = 723191857978540103;
        public DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
			var _config = new DiscordSocketConfig { MessageCacheSize = 100 }; //sets the message cache size
            _client = new DiscordSocketClient(); // defines the client
            _commands = new CommandService(); // deffines the command service 

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string token = System.IO.File.ReadAllLines(@"token.txt")[0]; ; // gets the bot token from the text file

            _client.Log += _client_Log;

			_client.ReactionAdded += ReactionAdded; //handles when reations are added to a message

            _client.ReactionRemoved += ReactionRemoved; //handles when reactions are removed from a message

            _client.UserLeft += AnnounceleftUser; // announces that a user has left the guild in the admin channel

            _client.UserJoined += AnnounceJoinedUser; // announces that a user has left the guild in the admin channel

            _client.ChannelCreated += ChanCreated;

            _client.ChannelDestroyed += ChanDestroyed;

            _client.ChannelUpdated += ChanUpdated;

            _client.MessageDeleted += messageDeleted;

            _client.MessageUpdated += MessageChanged;

            await RegisterCommandsAsync(); //starts the command service

            await _client.LoginAsync(TokenType.Bot, token); //starts the bot

            await _client.StartAsync(); //starts the bot

            await Task.Delay(-1);
			

        }
        public async Task AnnounceleftUser(SocketGuildUser user)
        {
            var chnl = _client.GetChannel(logging) as SocketTextChannel; // gets the SocketTextChannel for the id
            var time = DateTime.Now.Subtract(user.JoinedAt.Value.DateTime.ToLocalTime());
            if (time.Days > 0)
            {
                await chnl.SendMessageAsync($"{user} has left. \nThey joined  `{time.Days} days, {time.Hours} hours, {time.Minutes} minutes and {time.Seconds} secconds ago`");
            }
            else if (time.Hours > 0)
            {
                await chnl.SendMessageAsync($"{user} has left. \nThey joined  `{time.Hours} hours, {time.Minutes} minutes and {time.Seconds} secconds ago`"); // send the message
            }
            else
            {
                await chnl.SendMessageAsync($"{user} has left. \nThey joined  `{time.Minutes} minutes and {time.Seconds} secconds ago`"); // send the message
            }
            await updatemembers();
        }
        public async Task messageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            ulong id = logging; //public announce
            var chnl = _client.GetChannel(id) as SocketTextChannel; // gets the SocketTextChannel for the id
            var entry = _client.GetGuild(698508895202836511).GetAuditLogsAsync(5);
            Console.WriteLine(entry);
            await chnl.SendMessageAsync($"a message was deleted in {channel} by ");
        }
        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            ulong id = logging; //public announce
            var chnl = _client.GetChannel(id) as SocketTextChannel; // gets the SocketTextChannel for the id
            await chnl.SendMessageAsync($"welcome {user.Mention} to verify please use the command !a [age] replacing age with your age for example `!a 99`"); // send the message
            var time = DateTime.Now.Subtract(user.CreatedAt.DateTime.ToLocalTime());
            ulong id1 = logging; //private announce
            var chnl1 = _client.GetChannel(id1) as SocketTextChannel; // gets the SocketTextChannel for the id
            if (time.Days > 0)
            {
                await chnl1.SendMessageAsync($"{user.Username} has joined\nThere account was created  `{time.Days} days, {time.Hours} hours, {time.Minutes} minutes and {time.Seconds} secconds ago`");
            }
            else if (time.Hours > 0)
            {
                await chnl1.SendMessageAsync($"{user.Username} has joined\nThere account was created `{time.Hours} hours, {time.Minutes} minutes and {time.Seconds} secconds ago`"); // send the message
            }
            else
            {
                await chnl1.SendMessageAsync($"Warning {_client.GetUser(336923367208910848).Mention} a very new user {user.Username} has joined\nThere account was created `{time.Minutes} minutes and {time.Seconds} secconds ago`"); // send the message

            }
            await updatemembers();
        }
        public async Task updatemembers()
        {
            int meberers = _client.GetGuild(guild).MemberCount;
            await _client.GetGuild(guild).GetChannel(all).ModifyAsync(prop => prop.Name = $"👥 Member Count: {meberers}");
            await _client.GetGuild(guild).GetChannel(bots).ModifyAsync(prop => prop.Name = $"bots: 6");
            await _client.GetGuild(guild).GetChannel(people).ModifyAsync(prop => prop.Name = $"people: {meberers - 6}");
        }
        public async Task ReactionAdded(Cacheable<IUserMessage, UInt64> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
		{
            var message = cachedMessage.GetOrDownloadAsync(); //addes the message that the reaction was added to the the cache
            if (message != null && reaction.User.IsSpecified) //checks that the message exists and that the user is known
			{
                string[] lines = System.IO.File.ReadAllLines(@"roles.txt"); //imports the file containg all the rules
                foreach (string line in lines) // loops through every line
                {
                    string[] Subline = line.Split(','); // splits the line up into its parts
                    if (Subline[1] == reaction.MessageId.ToString()) // checks to see if the message is in rules
                    {
                        if (reaction.Emote.Name == Subline[2]) // checks to see if there is a rule for the reaction added to that message
                        {
                            await assignRole(Convert.ToUInt64(Subline[0]) , reaction.UserId , Subline[3]); //calls the roll assign function
                        }
                    }
                }
            }
		}
        public async Task ChanCreated(SocketChannel channel)
        {
            var chnl = _client.GetChannel(logging) as SocketTextChannel; // gets the SocketTextChannel for the id
            await chnl.SendMessageAsync($"channel {channel} has been created"); // send the message
        }
        public async Task ChanDestroyed(SocketChannel channel)
        {
            var chnl = _client.GetChannel(logging) as SocketTextChannel; // gets the SocketTextChannel for the id
            await chnl.SendMessageAsync($"channel {channel} has been destroyed"); // send the message
        }
        public async Task ChanUpdated(SocketChannel before , SocketChannel after)
        {
            var chnl = _client.GetChannel(logging) as SocketTextChannel; // gets the SocketTextChannel for the id
            if (before == after)
            {
                await chnl.SendMessageAsync($"an unknown change was made to {after}, this is most likly a permsions change");
            }
            else
            {
                await chnl.SendMessageAsync($"channel {before} has been updated to {after}"); // send the message
            }
        }
        public async Task MessageChanged(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            var chnl = _client.GetChannel(logging) as SocketTextChannel; // gets the SocketTextChannel for the id
            await chnl.SendMessageAsync($"{message.Author} has eddited a message in {channel}. \n`before {message} \nafter {after}`"); // send the message
        }
        public async Task ReactionRemoved(Cacheable<IUserMessage, UInt64> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = cachedMessage.GetOrDownloadAsync(); //addes the message that the reaction was added to the the cache
            if (message != null && reaction.User.IsSpecified) //checks that the message exists and that the user is known
            {
                string[] lines = System.IO.File.ReadAllLines(@"roles.txt"); //imports the file containg all the rules
                foreach (string line in lines) // loops through every line
                {
                    string[] Subline = line.Split(','); // splits the line up into its parts
                    if (Subline[1] == reaction.MessageId.ToString()) // checks to see if the message is in rules
                    {
                        if (reaction.Emote.Name == Subline[2]) // checks to see if there is a rule for the reaction added to that message
                        {
                            await RemoveRole(Convert.ToUInt64(Subline[0]), reaction.UserId, Subline[3]); //calls the roll remove function
                        }
                    }
                }
            }
        }
        public async Task assignRole(ulong guildid , ulong userid, string roleid) // function foir asigning the rolls, needs a guild id, user id and roll id
        {

            var role = _client.GetGuild(guildid).GetRole(Convert.ToUInt64(roleid)); //gets the role
            var user = _client.GetGuild(guildid).GetUser(userid); // gets the userid
            await user.AddRoleAsync(role); // assigns the roll
        }
        public async Task RemoveRole(ulong guildid, ulong userid, string roleid) // function foir removing the rolls, needs a guild id, user id and roll id
        {
            {

                var role = _client.GetGuild(guildid).GetRole(Convert.ToUInt64(roleid)); //gets the role
                var user = _client.GetGuild(guildid).GetUser(userid); // gets the userid
                await user.RemoveRoleAsync(role); // assigns the roll
            }
        }
        
        public string removeUser()
        {
            var guild = _client.GetGuild(698508895202836511);
            var users = guild.Users;
            Console.WriteLine(users);
            return("");

        }
        private Task _client_Log(LogMessage arg) // logs erros to the console
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync() // calls the commands script
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg) //i dont really know what this does but it makes the commands script work
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
