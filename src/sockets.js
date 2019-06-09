const log = require("simple-console-logger").getLogger("LiveMap Sockets");
const WS = require("ws");

const SocketController = (access) => {
    
    const hook = (wss) => {
        wss.broadcast = (data) => {
            wss.clients.forEach(function each(client) {
                if (client.readyState === WS.OPEN) {
                    client.send(data);
                }
            });
        };
        wss.on("connection", function connection(ws, req) { //ws, res
            log.trace("Access is: %s", access);
            if (access !== "*") { // We don't want to accept all requests
                if (req.headers.origin !== access) {
                    ws.close();
                    log.warn("Someone tried connecting from an invalid origin: %s", req.headers.origin);
                }
            }
            log.debug("connection made from", req.headers.origin);
        });
    };


    // The old, internal events... I can now just call them so... Make them functions!    
    const AddPlayerData = () => {};
    const UpdatePlayerData = () => {};
    const RemovePlayerData = () => {};
    const RemovePlayer = () => {};
    const AddBlip = () => {};
    const UpdateBlip = () => {};
    const RemoveBlip = () => {};


    return {
        hook,
        AddPlayerData,
        UpdatePlayerData,
        RemovePlayerData,
        RemovePlayer,
        AddBlip,
        UpdateBlip,
        RemoveBlip,
    };
};

module.exports = SocketController;