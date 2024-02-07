using DSharpPlus.SlashCommands;
using kandora.bot.resources;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("league", Resources.league_groupDescription, defaultPermission: false)]
    class LeaguedSlashCommands : KandoraSlashCommandModule
    {
    }
}
