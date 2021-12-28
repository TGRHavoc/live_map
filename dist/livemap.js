/******/ (() => { // webpackBootstrap
/******/ 	var __webpack_modules__ = ({

/***/ 529:
/***/ ((module, __unused_webpack_exports, __webpack_require__) => {

const blipLog = __webpack_require__(731).getLogger("LiveMap Blips");
const fs = __webpack_require__(747);
const path = __webpack_require__(622);

const BlipController = (SocketController) => {
    let playerWhoGeneratedBlips = null;
    let blips = null;
    const blipFile = GetConvar("blip_file", "server/blips.json");
    
    // Middleware to send the blips
    const getBlips = (ctx) => {
        if (blips === null){
            ctx.body = JSON.stringify({
                error: "blip cache is empty"
            });
        }else{
            ctx.body = JSON.stringify(blips);
        }
    };

    // function to save blips to blip file
    const saveBlips = () => {
        const saved = SaveResourceFile(GetCurrentResourceName(), blipFile, JSON.stringify(blips, null, 4), -1);

        if (saved) {
            blipLog.info("Saved blip file");
        } else {
            blipLog.warn("Error writing to blip file... Maybe permissions aren't correct?");
        }
    };
    // function to load blips from blip file
    const loadBlips = () => {
        const loaded = LoadResourceFile(GetCurrentResourceName(), blipFile);
        if (loaded) {
            try {
                blips = JSON.parse(loaded);
            } catch (err) {
                blipLog.warn("Error when parsing blips.json file. Please make sure it's valid JSON. %s", err.message);
            }
        } else {
            // Make an empty blip file?
            blips = {};
            saveBlips();
        }
    };

    const getDistanceBetween = (p1, p2) => {
        let x = Math.pow(p2.x - p1.x, 2);
        let y = Math.pow(p2.y - p1.y, 2);
        let z = Math.pow(p2.z - p1.z, 2);

        return Math.sqrt(x + y + z);
    };

    const getBlipIndex = (sprite, pos) => {
        if (blips[sprite] === undefined){
            return -1;
        }
        
        blips[sprite].forEach((blip, index) => {
            if (blip.pos == pos){
                return index;
            }
        });

        return -1;
    };

    const validBlip = (blip) => {
        if (blip["sprite"] === undefined || blip["sprite"] === null) {
            blipLog.debug("Blip didn't have sprite: %o", blip);
            blipLog.warn("Blip has no sprite...");
            return false;
        }
        
        if (blip["pos"] === undefined || blip["pos"] === null) {
            blipLog.debug("Blip didn't have pos: %o", blip);
            blipLog.warn("Blip has no position...");
            return false;
        }

        if (typeof(blip.pos) !== "object"){
            blipLog.warn("Blip position must be an object");
            return false;
        }

        if (blip.pos["x"] === undefined || blip.pos["y"] === undefined || blip.pos["z"] === undefined){
            blipLog.debug("Invalid position: %o", blip.pos);
            blipLog.warn("Blip position invalid");
            return false;
        }

        if (typeof(blip.pos.x) !== "number" || typeof(blip.pos.y) !== "number" || typeof(blip.pos.z) !== "number"){
            blipLog.warn("Blip pos must be numbers");
            blipLog.debug("Invalid position: %o", blip.pos);
            return false;
        }

        return true;
    };

    on("onResourceStart", (name) => {
        if (name === GetCurrentResourceName()){
            loadBlips();
        }
    });
    on("onResourceStop", (name) => {
        if (name === GetCurrentResourceName()){
            saveBlips();
        }
    });

    onNet("livemap:blipsGenerated", (blipTable) => {
        let playerIdentifier = GetPlayerIdentifier(source, 0);
        if (playerWhoGeneratedBlips === null || playerWhoGeneratedBlips !== playerIdentifier){
            blipLog.warn("playerWhoGeneratedBlips doesn't match... Not using the blips recieved");
            return;
        }

        blips = blipTable;
        saveBlips();
    });

    onNet("livemap:AddBlip", (blip) => {
        if (!validBlip(blip)){
            return;
        }

        //Extract the spite
        const sprite = blip.sprite.toString();
        delete blip.sprite;

        // Make sure it's to 2 dp
        blip.pos.x = blip.pos.x.toFixed(2);
        blip.pos.y = blip.pos.y.toFixed(2);
        blip.pos.z = blip.pos.z.toFixed(2);

        // Get the index of the blip.. Making sure it exists
        let blipIndex = getBlipIndex(sprite, blip);

        if (blipIndex !== -1){
            blipLog.debug("Duplicate blip: %d = %o", blipIndex, blip);
            blipLog.warn("Blip already exists... Cannot add it!");
            return;
        }

        if (blips[sprite] === undefined){
            blips[sprite] = [];
        }

        blips[sprite].push(blip);

        SocketController.AddBlip(sprite, blip);
    });

    onNet("livemap:UpdateBlip", (blip) => {
        if(!validBlip(blip)){
            return;
        }

        //Extract the spite
        const sprite = blip.sprite.toString();
        delete blip.sprite;

        // Make sure it's to 2 dp
        blip.pos.x = blip.pos.x.toFixed(2);
        blip.pos.y = blip.pos.y.toFixed(2);
        blip.pos.z = blip.pos.z.toFixed(2);

        // Get the index of the blip.. Making sure it exists
        let blipIndex = getBlipIndex(sprite, blip);

        if (blipIndex !== -1){
            blips[sprite][blipIndex] = blip;
            SocketController.UpdateBlip(sprite, blip);
        }else{
            if (blips[sprite] === undefined){
                blips[sprite] = [];
            }
            
            blips[sprite].push(blip);
            SocketController.AddBlip(sprite, blip);
        }
    });

    onNet("livemap:RemoveBlip", (blip) => {
        if (!validBlip(blip)){
            return;
        }

        //Extract the spire
        const sprite = blip.sprite.toString();
        delete blip.sprite;

        // Make sure it's to 2 dp
        blip.pos.x = blip.pos.x.toFixed(2);
        blip.pos.y = blip.pos.y.toFixed(2);
        blip.pos.z = blip.pos.z.toFixed(2);

        // Get the index of the blip.. Making sure it exists
        let blipIndex = getBlipIndex(sprite, blip);
        if (blipIndex !== -1) {
            delete blips[sprite][blipIndex];
            blipLog.info("Removed blip: %o", blip);
            SocketController.RemoveBlip(sprite, blip);
        }else{
            blipLog.warn("Cannot delete a blip that doesn't exist");
        }
    });

    onNet("livemap:RemoveClosestBlip", (playerPos) => {
        let closest, sprite, currentBest;
        for (let spriteId in blips){
            blips[spriteId].forEach(blip => {
                let myDistance = getDistanceBetween(playerPos, blip.pos);
                if (currentBest === undefined || myDistance < currentBest){
                    currentBest = myDistance;
                    closest = blip;
                    sprite = spriteId;
                }
            });
        }

        if (currentBest === undefined) { // Possibly no blips..
            return;
        }

        blipLog.info("Removing closest blip. It's sprite is %d and position (dis=%d) is %o", sprite, closest.pos);

        const blipToDelete = getBlipIndex(sprite, closest.pos);
        if (blipToDelete == -1){
            blipLog.debug("Closest blip doesn't exist? %o\n%O", closest, blips);
            return;
        }

        delete blips[sprite][blipToDelete];
        SocketController.RemoveBlip(sprite, closest);
    });

    RegisterCommand("blips", (src, args) => {
        if (src === 0) {
            blipLog.warn("Please run this command in game. Make sure you have ACE permissions set up");
            blipLog.warn("https://docs.tgrhavoc.me/livemap-resource/faq/#how-do-i-get-blips");
            return;
        }

        let playerIdentifier = GetPlayerIdentifier(src, 0);
        if (args[0] === "generate") {
            playerWhoGeneratedBlips = playerIdentifier;
            blipLog.warn("Generating blips using the in-game natives: Player %s is generating them.", playerIdentifier);
            emitNet("livemap:getBlipsFromClient", src);
        }
    }, true);
    
    return {
        getBlips
    };
};


module.exports = BlipController;


/***/ }),

/***/ 376:
/***/ ((module, __unused_webpack_exports, __webpack_require__) => {

const socketLog = __webpack_require__(731).getLogger("LiveMap Sockets");
const WS = __webpack_require__(439);

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
            socketLog.trace("Access is: %s", access);
            if (access !== "*") { // We don't want to accept all requests
                if (req.headers.origin !== access) {
                    ws.close();
                    socketLog.warn("Someone tried connecting from an invalid origin: %s", req.headers.origin);
                }
            }
            socketLog.debug("connection made from", req.headers.origin);
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
            socketLog.debug("Empty identifier: ", identifier);
            socketLog.warn("Identifier cannot be null or empty when adding player data");
            return false;
        }
        if (!key) {
            socketLog.warn("Key for the data cannot be null or empty when adding player data");
            return false;
        }
        if (!data) {
            socketLog.warn("Cannot add no data to player");
            return false;
        }

        return true;
    };

    const validBlip = (blip) => {
        if (blip["sprite"] === undefined || blip["sprite"] === null) {
            socketLog.debug("Blip didn't have sprite: %o", blip);
            socketLog.warn("Blip has no sprite...");
            return false;
        }

        if (blip["pos"] === undefined || blip["pos"] === null) {
            socketLog.debug("Blip didn't have pos: %o", blip);
            socketLog.warn("Blip has no position...");
            return false;
        }

        if (typeof (blip.pos) !== "object") {
            socketLog.warn("Blip position must be an object");
            return false;
        }

        if (blip.pos["x"] === undefined || blip.pos["y"] === undefined || blip.pos["z"] === undefined) {
            socketLog.debug("Invalid position: %o", blip.pos);
            socketLog.warn("Blip position invalid");
            return false;
        }

        if (typeof (blip.pos.x) !== "number" || typeof (blip.pos.y) !== "number" || typeof (blip.pos.z) !== "number") {
            socketLog.warn("Blip pos must be numbers");
            socketLog.debug("Invalid position: %o", blip.pos);
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
            socketLog.warn("Cannot remove player with no identifier");
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

/***/ }),

/***/ 607:
/***/ ((module, __unused_webpack_exports, __webpack_require__) => {

const wrapperLog = __webpack_require__(731).getLogger("LiveMap Wrapper");

const EventsWrapper = (SocketController) => {

    const setStaticData = (source, id) => {
        SocketController.AddPlayerData(id, "identifier", id);
        SocketController.AddPlayerData(id, "name", GetPlayerName(source));
    };

    onNet("onResourceStart", (name) => {
        if (name == GetCurrentResourceName()){
            if (GetNumPlayerIndices() !== 0){
                wrapperLog.info("Players on the server... Initializing them");
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

    // Internal events for server-side scripts. See https://github.com/TGRHavoc/live_map/issues/45
    on("livemap:internal_AddPlayerData", (id, k, d) => {
        SocketController.AddPlayerData(id, k, d);
    });

    on("livemap:internal_UpdatePlayerData", (id, k, d) => {
        SocketController.UpdatePlayerData(id, k, d);
    });

    on("livemap:internal_RemovePlayerData", (id, k) => {
        SocketController.RemovePlayerData(id, k);
    });

    on("livemap:internal_RemovePlayer", (id) => {
        SocketController.RemovePlayer(id);
    });

    return {};
};

module.exports = EventsWrapper;

/***/ }),

/***/ 747:
/***/ ((module) => {

"use strict";
module.exports = require("fs");;

/***/ }),

/***/ 605:
/***/ ((module) => {

"use strict";
module.exports = require("http");;

/***/ }),

/***/ 639:
/***/ ((module) => {

"use strict";
module.exports = require("koa");;

/***/ }),

/***/ 775:
/***/ ((module) => {

"use strict";
module.exports = require("koa-body");;

/***/ }),

/***/ 757:
/***/ ((module) => {

"use strict";
module.exports = require("koa-router");;

/***/ }),

/***/ 622:
/***/ ((module) => {

"use strict";
module.exports = require("path");;

/***/ }),

/***/ 731:
/***/ ((module) => {

"use strict";
module.exports = require("simple-console-logger");;

/***/ }),

/***/ 439:
/***/ ((module) => {

"use strict";
module.exports = require("ws");;

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it need to be isolated against other modules in the chunk.
(() => {
const Koa = __webpack_require__(639);
const koaBody = __webpack_require__(775);
const Router = __webpack_require__(757);
const WS = __webpack_require__(439);
const http = __webpack_require__(605);
const logger = __webpack_require__(731);

const app = new Koa();
const server = http.createServer(app.callback());
const router = new Router();
const wss = new WS.Server({ server });

const debugLevel = GetConvar("livemap_debug_level", "warn");
const access = GetConvar("livemap_access_control", "*");

logger.configure({
    level: debugLevel
});
const log = logger.getLogger("LiveMap");

router.use(async (ctx, next) => {
    ctx.response.append("Access-Control-Allow-Origin", access);
    next();
});

const SocketController = __webpack_require__(376)(access);
SocketController.hook(wss);

__webpack_require__(607)(SocketController);

// Passing the SocketController through as we need it to keep blips updated on the client. Plus, I can't seem to just "require" the server in the BlipController.
const BlipController = __webpack_require__(529)(SocketController);

router.get("/blips", BlipController.getBlips);
router.get("/blips.json", BlipController.getBlips);

app.use(koaBody(
    {
        patchKoa: true,
    }))
    .use(router.routes())
    .use(router.allowedMethods());

// Start a server on the socket_port...
const port = GetConvarInt("socket_port", 30121);

server.listen(port, "127.0.0.1", function listening() {
    log.info("Listening on %d", port);
});

setInterval(SocketController.SendPlayerData, GetConvarInt("livemap_milliseconds", 500)); // Default = half a second.

})();

/******/ })()
;