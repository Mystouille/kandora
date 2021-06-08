using kandora.bot.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.models
{
    public class PendingGame
    {
        public PendingGame(List<User> users, Server server, RiichiGame log)
        {
            Users = new Dictionary<string, User>();
            foreach (var user in users)
            {
                Users.Add(user.Id, user);
            }
            Log = log;
            Server = server;
            usersOk = new HashSet<string>();
            usersNo = new HashSet<string>();
        }
        ISet<string> usersOk;
        ISet<string> usersNo;
        public Dictionary<string, User> Users { get; }
        public RiichiGame Log { get; }
        public Server Server { get; }
        public bool TryChangeUserOk(string userId, bool isAdd)
        {
            return TryChangeSet(usersOk, userId, isAdd);
        }
        public bool TryChangeUserNo(string userId, bool isAdd)
        {
            return TryChangeSet(usersNo, userId, isAdd);
        }
        public bool IsCancelled
        {
            get {
                return usersNo.Count == Users.Count;
            }
        }
        public bool IsValidated
        {
            get
            {
                return usersOk.Count == Users.Count;
            }
        }
        private bool TryChangeSet(ISet<string> set, string userId, bool isAdd)
        {
            if (Users.ContainsKey(userId))
            {
                if (isAdd)
                {
                    set.Add(userId);
                }
                else
                {
                    set.Remove(userId);
                }
                return true;
            }
            return false;
        }
    }
}
