const log = require("simple-console-logger").getLogger("LiveMap Sockets");
const WS = require("ws");

const SocketController = (wss, access) => {
    wss.broadcast = function broadcast(data) {
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
    
    return {};
};

module.exports = SocketController;