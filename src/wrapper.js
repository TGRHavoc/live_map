const log = require("simple-console-logger").getLogger("LiveMap Wrapper");

const EventsWrapper = (SocketController) => {

    const setStaticData = (source, id) => {
        SocketController.AddPlayerData(id, "identifier", id);
        SocketController.AddPlayerData(id, "name", GetPlayerName(source));
    };

    onNet("onResourceStart", (name) => {
        if (name == GetCurrentResourceName()){
            if (GetNumPlayerIndices() !== 0){
                log.info("Players on the server... Initialising them");
                for(let i = 0; i < GetNumPlayerIndices(); i++){
                    const id = GetPlayerIdentifier(GetPlayerFromIndex(i), 0);
                    setStaticData(GetPlayerFromIndex(i), id);
                }
            }
        }
    });

    onNet("livemap:playerSpawned", () => {
        setStaticData(source, GetPlayerIdentifier(source));
    });

    onNet("livemap:AddPlayerData", (k, d) => {
        SocketController.AddPlayerData(GetPlayerIdentifier(source, 0), k, d);
    });

    onNet("livemap:UpdatePlayerData", (k, d) => {
        SocketController.UpdatePlayerData(GetPlayerIdentifier(source, 0), k, d);
    });

    onNet("livemap:RemovePlayerData", (k) => {
        SocketController.RemovePlayerData(GetPlayerIdentifier(source, 0), k);
    });

    onNet("livemap:RemovePlayer", () => {
        SocketController.RemovePlayer(GetPlayerIdentifier(source, 0));
    });

    onNet("playerDropped", () => {
        SocketController.RemovePlayer(GetPlayerIdentifier(source, 0));
    });    

    return {};
};

module.exports = EventsWrapper;