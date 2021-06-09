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
        public PendingGame(string[] userIds, Server server, RiichiGame log)
        {
            UserIds = userIds;
            Log = log;
            Server = server;
            usersOk = new HashSet<string>();
            usersNo = new HashSet<string>();
        }
        public PendingGame(string[] userIds, float[] scores, Server server)
        {
            UserIds = userIds;
            Scores = scores;
            Server = server;
            usersOk = new HashSet<string>();
            usersNo = new HashSet<string>();
        }

        private ISet<string> usersOk;
        private ISet<string> usersNo;
        public string[] UserIds { get; }
        public float[] Scores { get; }
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
                return usersNo.Count == UserIds.Count();
            }
        }
        public bool IsValidated
        {
            get
            {
                return usersOk.Count == UserIds.Count();
            }
        }
        private bool TryChangeSet(ISet<string> set, string userId, bool isAdd)
        {
            if (UserIds.Contains(userId))
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
