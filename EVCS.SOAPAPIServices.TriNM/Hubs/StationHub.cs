using Microsoft.AspNetCore.SignalR;

namespace EVCS.SOAPAPIServices.TriNM.Hubs
{
    public class StationHub : Hub
    {
        public async Task JoinStationGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Stations");
        }

        public async Task LeaveStationGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Stations");
        }
    }
}

