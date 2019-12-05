using Shoko.Server.Providers.AniDB.UDP.Exceptions;
using Shoko.Server.Providers.AniDB.UDP.Responses;

namespace Shoko.Server.Providers.AniDB.UDP.Requests
{
    public abstract class AniDBUDP_BaseRequest<T> where T : class
    {
        protected string Command { get; set; } = string.Empty;
        /// <summary>
        /// Various Parameters to add to the base command
        /// </summary>
        protected abstract string BaseCommand { get; }

        protected bool HasEexecuted { get; set; }
        
        protected AniDBUDP_Response<T> _response { get; set; }
        
        /// <summary>
        /// The Response
        /// </summary>
        public AniDBUDP_Response<T> Response {
            get
            {
                if (!HasEexecuted) throw new CommandNotExecutedException();;
                return _response;
            }
            set => _response = value;
        }

        protected abstract AniDBUDP_Response<T> ParseResponse(AniDBUDPReturnCode code, string receivedData);

        public virtual void Execute(AniDBConnectionHandler handler)
        {
            Command = BaseCommand;
            PreExecute(handler.SessionID);
            AniDBUDP_Response<string> response = handler.CallAniDBUDP(Command);
            Response = ParseResponse(response.Code, response.Response);
            PostExecute(handler.SessionID, _response);
            HasEexecuted = true;
        }

        protected virtual void PreExecute(string sessionID)
        {
            Command += $"&s={sessionID}";
        }

        protected virtual void PostExecute(string sessionID, AniDBUDP_Response<T> response)
        {
        }
    }
}