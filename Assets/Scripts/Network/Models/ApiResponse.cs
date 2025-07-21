namespace NativeWebSocket.Models
{
    public class ApiResponse
    {
        [System.Serializable]
        public class LoginResponse
        {
            public string message;
            public int userId;
            public string nickname;
        }
        [System.Serializable]
        public class CreateRoomResponse
        {
            public string roomCode;
        }

        [System.Serializable]
        public class JoinRoomResponse
        {
            public string roomCode;
        }
    }
}