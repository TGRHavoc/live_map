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
        websocketServer = wss;
    };

    let playerData = {};
    let websocketServer = null;

    const makeSurePlayerExists = (id) => {
        if (playerData[id] === undefined){ 
            playerData[id] = {};
        }
    };

    const SendPlayerData = () => {
        if (websocketServer == null) return;

        let payload = [];
        for (let identifier in playerData){
            payload.push(playerData[identifier]);
        }
        websocketServer.broadcast(JSON.stringify({
            type: "playerData",
            payload: payload
        }));
    };

    const checkInputs = (identifier, key, data) => {
        if (!identifier) {
            log.debug("Empty identifier: ", identifier);
            log.warn("Identifier cannot be null or empty when adding player data");
            return false;
        }
        if (!key) {
            log.warn("Key for the data cannot be null or empty when adding player data");
            return false;
        }
        if (!data) {
            log.warn("Cannot add no data to player");
            return false;
        }

        return true;
    };

    const validBlip = (blip) => {
        if (blip["sprite"] === undefined || blip["sprite"] === null) {
            log.debug("Blip didn't have sprite: %o", blip);
            log.warn("Blip has no sprite...");
            return false;
        }

        if (blip["pos"] === undefined || blip["pos"] === null) {
            log.debug("Blip didn't have pos: %o", blip);
            log.warn("Blip has no position...");
            return false;
        }

        if (typeof (blip.pos) !== "object") {
            log.warn("Blip position must be an object");
            return false;
        }

        if (blip.pos["x"] === undefined || blip.pos["y"] === undefined || blip.pos["z"] === undefined) {
            log.debug("Invalid position: %o", blip.pos);
            log.warn("Blip position invalid");
            return false;
        }

        if (typeof (blip.pos.x) !== "number" || typeof (blip.pos.y) !== "number" || typeof (blip.pos.z) !== "number") {
            log.warn("Blip pos must be numbers");
            log.debug("Invalid position: %o", blip.pos);
            return false;
        }

        return true;
    };

    // The old, internal events... I can now just call them so... Make them functions!    
    const AddPlayerData = (identifier, key, data) => {
        UpdatePlayerData(identifier, key, data); // It'll get added in here anyways...
    };
    const UpdatePlayerData = (identifier, key, data) => {
        if (!checkInputs(identifier, key, data)) {
            return;
        }
        makeSurePlayerExists(identifier);

        playerData[identifier][key] = data;
    };
    const RemovePlayerData = (identifier, key) => {
        if(!checkInputs(identifier, key, "s")) {
            return;
        }
        makeSurePlayerExists(identifier);

        if (playerData[identifier][key] !== undefined) {
            delete playerData[identifier][key];
        }
    };
    const RemovePlayer = (identifier) => {
        if (!identifier) {
            log.warn("Cannot remove player with no identifier");
            return;
        }

        delete playerData[identifier];
        websocketServer.broadcast(JSON.stringify({
            type: "playerLeft",
            payload: identifier
        }));
    };
    const AddBlip = (blip) => {
        if(!validBlip(blip)) {
            return;
        }

        websocketServer.broadcast(JSON.stringify({
            type: "addBlip",
            payload: blip
        }));
    };
    const RemoveBlip = (blip) => {
        if(!validBlip(blip)) {
            return;
        }

        websocketServer.broadcast(JSON.stringify({
            type: "removeBlip",
            payload: blip
        }));
    };
    const UpdateBlip = (blip) => {
        if(!validBlip(blip)) {
            return;
        }

        websocketServer.broadcast(JSON.stringify({
            type: "updateBlip",
            payload: blip
        }));
    };

    return {
        hook,
        SendPlayerData,
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