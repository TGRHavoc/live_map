const Koa = require("koa");
const koaBody = require("koa-body");
const Router = require("koa-router");
const WS = require("ws");
const http = require("http");
const logger = require("simple-console-logger");

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

const SocketController = require("LivemapSocketController")(access);
SocketController.hook(wss);

require("LivemapEventsWrapper")(SocketController);

// Passing the SocketController through as we need it to keep blips updated on the client. Plus, I can't seem to just "require" the server in the BlipController.
const BlipController = require("LivemapBlipController")(SocketController);

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

server.listen(port, function listening() {
    log.info("Listening on %d", port);
});

setInterval(SocketController.SendPlayerData, GetConvarInt("livemap_milliseconds", 500)); // Default = half a second.
