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
                else
                {
                    throw (new SignGameException());
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
                else
                {
                    throw (new SignGameException());
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
                else
                {
                    throw (new SignGameException());
                }
            }
        }
        public bool IsSigned
        {
            get
            {
                return user1Signed && user2Signed && user3Signed && user4Signed;
            }
        }
        

        public bool TrySignGameByUser(ulong userId)
        {
            bool result = false;
            if (User1Id == userId) {
                result = ScoreDb.SignGameByUserPos(Id, 1);
                user1Signed = user1Signed || result;
            }
            else if (User2Id == userId)
            {
                result = ScoreDb.SignGameByUserPos(Id, 2);
                user2Signed = user2Signed || result;
            }
            else if (User3Id == userId)
            {
                result = ScoreDb.SignGameByUserPos(Id, 3);
                user3Signed = user3Signed || result;
            }
            else if (User4Id == userId)
            {
                result = ScoreDb.SignGameByUserPos(Id, 4);
                user4Signed = user4Signed || result;
            }
            else {
                throw (new UserNotFoundInGameException());
            }
            return result;
        }
    }
}
