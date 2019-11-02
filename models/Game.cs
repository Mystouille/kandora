using System;
using System.Collections.Generic;
using System.Text;

namespace Kandora
{
    class Game
    {
        public int Id { get; }
        private bool user1Signed;
        private bool user2Signed;
        private bool user3Signed;
        private bool user4Signed;

        public Game (int id, ulong user1Id, ulong user2Id, ulong user3Id, ulong user4Id, bool user1Signed, bool user2Signed, bool user3Signed, bool user4Signed)
        {
            this.Id = id;
            this.User1Id = user1Id;
            this.User2Id = user2Id;
            this.User3Id = user3Id;
            this.User4Id = user4Id;
            this.user1Signed = user1Signed;
            this.user2Signed = user2Signed;
            this.user3Signed = user3Signed;
            this.user4Signed = user4Signed;
        }

        public ulong User1Id { get; }
        public ulong User2Id { get; }
        public ulong User3Id { get; }
        public ulong User4Id { get; }
        public bool User1Signed
        {
            get { return user1Signed; }
            set
            {
                var success = ScoreDb.SignGameByUserPos(Id, 1);
                if (success)
                {
                    this.user1Signed = true;
                }
            }
        }
        public bool User2Signed
        {
            get { return user2Signed; }
            set
            {
                var success = ScoreDb.SignGameByUserPos(Id, 2);
                if (success)
                {
                    this.user2Signed = true;
                }
            }
        }
        public bool User3Signed
        {
            get { return user3Signed; }
            set
            {
                var success = ScoreDb.SignGameByUserPos(Id, 3);
                if (success)
                {
                    this.user3Signed = true;
                }
            }
        }
        public bool User4Signed
        {
            get { return user4Signed; }
            set
            {
                var success = ScoreDb.SignGameByUserPos(Id, 4);
                if (success)
                {
                    this.user4Signed = true;
                }
            }
        }

        public bool TrySignGameByUser(ulong userId)
        {
            if (User1Id == userId) {
                return ScoreDb.SignGameByUserPos(Id, 1);
            }
            else if (User2Id == userId)
            {
                return ScoreDb.SignGameByUserPos(Id, 2);
            }
            else if (User3Id == userId)
            {
                return ScoreDb.SignGameByUserPos(Id, 3);
            }
            else if (User4Id == userId)
            {
                return ScoreDb.SignGameByUserPos(Id, 4);
            }
            else {
                throw (new Exception("Cancelled. It seems like you didn't play that game."));
            }
        }

    }
}
