using System;

namespace NativeWebSocket.Models
{
    public class ApiResponse
    {
        [Serializable]
        public class LoginResponse
        {
            public string userId;
            public string nickname;
        }
        [Serializable]
        public class CreateRoomResponse
        {
            public string roomCode;
        }

        [Serializable]
        public class JoinRoomResponse
        {
            public string roomCode;
        }
        [Serializable]
        public class RoomStatusResponse
        {
            public int playerCount;
        }
    }
}