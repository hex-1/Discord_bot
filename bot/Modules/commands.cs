using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
//using System.Text.RegularExpressions;

namespace TutorialBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        public ulong logging = 722489534935334982;
        [Command("ping")] // check if the bot is online
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }
        [Command("nick")] //get your username posible look into using this to set it, doing it this way will allow for filters to be added
        public async Task nick([Remainder]string data)
        {
            var before = Context.Guild.GetUser(Context.User.Id).Nickname;
            string[] lines = System.IO.File.ReadAllLines(@"prof.txt");
            bool found = false;
            foreach (string line in lines)
            {
                if (data.Contains(line))
                {
                    found = true;
                }
            }
            if (found == true)
            {
                var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
                await chnl.SendMessageAsync($"user {Context.User.Username} just tried to set there nickname to ||{data}||"); // send the message
                await ReplyAsync("sorry your nickname can not be set to that.");
            }
            else if (found == false)
            {
                var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
                await chnl.SendMessageAsync($"nickanme updated:\n`user:{Context.User.Username}\nbefore:{before}\nafter:{data}`");
                await Context.Guild.GetUser(Context.User.Id).ModifyAsync(p => p.Nickname = data);
                await ReplyAsync("nickname set to " + data + ".");
            }
        }
        [Command("verify")] // verification instructions
        public async Task verify()
        {

            await ReplyAsync("to verify please use the command !a [age] where age is your age without a space i.e `!a 99` and you will be automatically approved");
        }
        [Command("warn")] // verification instructions
        public async Task warn([Remainder]string user)
        {
            var roles = Context.Guild.GetUser(Context.User.Id).GetPermissions(Context.Guild.GetTextChannel(Context.Channel.Id));
            Console.WriteLine(roles);
            if (roles.ToString().Contains("511041"))
            {
                Console.WriteLine("yes");
                string[] userarray = user.Split(',');
                string userid = userarray[0].Trim('<', '@', '!', '>');
                string[] lines = System.IO.File.ReadAllLines(@"warnings.txt");
                bool found = false;
                foreach (string line in lines)
                {
                    if(line.Contains(userid))
                    {
                        found = true;
                    }
                }
                if (found == true)
                {
                    var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel; 
                    await chnl.SendMessageAsync($"user {user} has been banned, seccond offence was {userarray[1]}"); // send the message
                    var c = await Context.Guild.GetUser(Convert.ToUInt64(userid)).GetOrCreateDMChannelAsync();
                    await c.SendMessageAsync("moderator " + Context.User.ToString().Split('#')[0] + " has given you a warning for " + userarray[1] + " unfortanatly this is your seccond warning so you are going to be removed, if you think there has been a mistake please contact hex_1#7998");
                    await Context.Guild.GetUser(Convert.ToUInt64(userid)).KickAsync();
                }
                else
                {
                    var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
                    await chnl.SendMessageAsync($"user {user} has been warned for {userarray[1]} by {Context.User.ToString().Split('#')[0]}"); // send the message
                    var c = await Context.Guild.GetUser(Convert.ToUInt64(userid)).GetOrCreateDMChannelAsync();
                    await c.SendMessageAsync("moderator " + Context.User.ToString().Split('#')[0] + " has given you a warning for " + userarray[1] + " please be carefull in future, next time you will be kicked from the server");
                    using (StreamWriter sw = File.AppendText(@"warnings.txt"))
                    {
                        sw.WriteLine(userid);
                    }
                }
            }
        }
        [Command("og")] // assigns the origonal member role
        public async Task og()
        {

            ulong roleId1 = 707319129480101990;
            var role1 = Context.Guild.GetRole(roleId1);
            await ((SocketGuildUser)Context.User).AddRoleAsync(role1); 
            var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
            await chnl.SendMessageAsync($"Roll assignment\n`user: {Context.User.Username}\nroll: {role1}`"); // send the message
        }
        [Command("I")]
        public async Task inactive([Remainder]string user) // for kicking a given user and telling them why
        {
            var roles = Context.Guild.GetUser(Context.User.Id).GetPermissions(Context.Guild.GetTextChannel(Context.Channel.Id));
            if (roles.ToString().Contains("805829713"))
            {
                string[] userarray = user.Split(',');
                string userid = userarray[0].Trim('<', '@', '!', '>');
                await ReplyAsync("removing user");
                var c = await Context.Guild.GetUser(Convert.ToUInt64(userid)).GetOrCreateDMChannelAsync();
                await c.SendMessageAsync($"sorry but you have been removed from {Context.Guild.Name} for being inactive, if you want to rejoin and be active you can use the link discord.gg/Kxmfjxx");
                await Context.Guild.GetUser(Convert.ToUInt64(userid)).KickAsync();
            }
        }
        [Command("v")]
        public async Task unverified([Remainder]string user) // for kicking a given user and telling them why
        {
            var roles = Context.Guild.GetUser(Context.User.Id).GetPermissions(Context.Guild.GetTextChannel(Context.Channel.Id));
            if (roles.ToString().Contains("805829713"))
            {
                string[] userarray = user.Split(',');
                string userid = userarray[0].Trim('<', '@', '!', '>');
                await ReplyAsync("removing user");
                var c = await Context.Guild.GetUser(Convert.ToUInt64(userid)).GetOrCreateDMChannelAsync();
                await c.SendMessageAsync($"sorry but you have been removed from {Context.Guild.Name} for being not verifying, if you want to rejoin and verify discord.gg/Kxmfjxx");
                await Context.Guild.GetUser(Convert.ToUInt64(userid)).KickAsync();
            }
        }
        [Command("R")]
        public async Task addrole([Remainder]string data) // allows for programming of the reaction based roll assignment
        {
            var roles = Context.Guild.GetUser(Context.User.Id).GetPermissions(Context.Guild.GetTextChannel(Context.Channel.Id));
            if (roles.ToString().Contains("805829713"))
            {
                if(data == "help")
                 {
                    await ReplyAsync("please use the commend '!r guild-id,message-id,emote,roll-id' where emote is the unoicode emote");
                }
                else
                {
                    string[] lines = System.IO.File.ReadAllLines(@"roles.txt");
                    bool found = false;
                    foreach (string line in lines)
                    {
                        if (line.Contains(data))
                        {
                            found = true;
                        }
                    }
                    if (found == true)
                    {
                        await ReplyAsync("that is alredy in the database");
                    }
                    else if (found == false)
                    {
                        
                        using (StreamWriter sw = File.AppendText(@"roles.txt"))
                        {
                            sw.WriteLine(data);
                        }
                        await ReplyAsync(data + " has been added");
                    }
                }

            }
            else
            {
                await ReplyAsync("sorry you dont have the pemsions to do this");
            }
        }
        [Command("a")]
        public async Task age([Remainder]string data) // allows for programming of the reaction based roll assignment
        {
            ulong verified = 626135832943788037;
            int value;
            bool res = int.TryParse(data, out value);
            if(res == true)
            {
                if(value < 13)
                {
                    await ReplyAsync($"Welcome sorry but you are too young to be on discord. See https://discord.com/terms for more info");
                }
                if (value >= 13 && value < 18)
                {
                    ulong roleId1 = 706568202737418350;
                    var role1 = Context.Guild.GetRole(roleId1);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role1);
                    var role2 = Context.Guild.GetRole(verified);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role2);
                    await ReplyAsync($"Welcome {Context.User.Username}");
                    var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
                    await chnl.SendMessageAsync($"Roll assignment\n`user: {Context.User.Username}\nroll: {role1} and {role2}`");
                }
                if (value >= 18 && value <= 20)
                {
                    ulong roleId1 = 627623170165309440;
                    var role1 = Context.Guild.GetRole(roleId1);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role1);
                    var role2 = Context.Guild.GetRole(verified);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role2);
                    await ReplyAsync($"Welcome {Context.User.Username}");
                    var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
                    await chnl.SendMessageAsync($"Roll assignment\n`user: {Context.User.Username}\nroll: {role1} and {role2}`");
                }
                if (value >= 21 && value <= 30)
                {
                    ulong roleId1 = 627623272103804939;
                    var role1 = Context.Guild.GetRole(roleId1);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role1);
                    var role2 = Context.Guild.GetRole(verified);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role2);
                    await ReplyAsync($"Welcome {Context.User.Username}");
                    var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
                    await chnl.SendMessageAsync($"Roll assignment\n`user: {Context.User.Username}\nroll: {role1} and {role2}`");
                }
                if (value >= 31 && value <= 40)
                {
                    ulong roleId1 = 640498913614495774;
                    var role1 = Context.Guild.GetRole(roleId1);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role1);
                    var role2 = Context.Guild.GetRole(verified);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role2);
                    await ReplyAsync($"Welcome {Context.User.Username}");
                    var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
                    await chnl.SendMessageAsync($"Roll assignment\n`user: {Context.User.Username}\nroll: {role1} and {role2}`");
                }
                if (value >= 41)
                {
                    ulong roleId1 = 640499040303448064;
                    var role1 = Context.Guild.GetRole(roleId1);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role1);
                    var role2 = Context.Guild.GetRole(verified);
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role2);
                    await ReplyAsync($"Welcome {Context.User.Username}");
                    var chnl = Context.Guild.GetChannel(logging) as SocketTextChannel;
                    await chnl.SendMessageAsync($"Roll assignment\n`user: {Context.User.Username}\nroll: {role1} and {role2}`");
                }
            }
            else
            {
                await ReplyAsync("An error has occured");
            }
        }

    }
   }
