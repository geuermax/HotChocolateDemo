using HotChocolate.Server;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace HotChocolateDemo.hcWebsockets
{
    public class MyWebSocketManager
    {

        private readonly ConcurrentDictionary<string, ISocketSession> _sessions = new ConcurrentDictionary<string, ISocketSession>();
        private readonly ConcurrentDictionary<string, System.Timers.Timer> _timers = new ConcurrentDictionary<string, System.Timers.Timer>();
        

        public ConcurrentDictionary<string, ISocketSession> GetAll()
        {
            return _sessions;
        }

        public string GetSocketSessionId(ISocketSession session)
        {
            return _sessions.FirstOrDefault(s => s.Value == session).Key;
        }

        public ISocketSession GetSocketSessionById(string key)
        {
            return _sessions.FirstOrDefault(s => s.Key == key).Value;
        }

        public void AddSocketSession(ISocketSession socketSession)
        {
            string id = CreateId();
            Console.WriteLine("Add socket to list. ID: {0} \tTime: {1}", id, DateTime.Now);
            _sessions.TryAdd(id, socketSession);
            
            var timer = new System.Timers.Timer(15*1000*60);
            timer.Elapsed += (sender, e) =>
            {
                Console.WriteLine("Closing socket. ID: {0} \tTime: {1}", id, DateTime.Now);
                RemoveSocketSesion(id);
            };
            timer.AutoReset = false;
            _timers.TryAdd(id, timer);
            timer.Start();            
        }

        public void RemoveSocketSesion(string id)
        {            
            ISocketSession socketSession;
            System.Timers.Timer timer;
            _sessions.TryRemove(id, out socketSession);
            _timers.TryRemove(id, out timer);
            timer.Stop();
            socketSession.Dispose();
        }

        private string CreateId()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
