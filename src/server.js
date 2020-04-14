const Koa = require("koa");
const httpCallback = require("@citizenfx/http-wrapper");
const Router = require("koa-router");
const logger = require("simple-console-logger");

const sse = require("./src/sse");

const app = new Koa();
const router = new Router();

const debugLevel = GetConvar("livemap_debug_level", "[all]");
const access = GetConvar("livemap_access_control", "*");

logger.configure({
    level: debugLevel
});
const log = logger.getLogger("LiveMap");

app.use(async (ctx, next) => {
    log.debug("", ctx.res.cfxReq);
    ctx.response.append("Access-Control-Allow-Origin", access);
    next();
});

sse(router);

//const SocketController = require("LivemapSocketController")(access);
//SocketController.hook(wss);

//require("LivemapEventsWrapper")(SocketController);

// Passing the SocketController through as we need it to keep blips updated on the client. Plus, I can't seem to just "require" the server in the BlipController.
// const BlipController = require("LivemapBlipController")();

// router.get("/blips", BlipController.getBlips);
// router.get("/blips.json", BlipController.getBlips);

router.get("/test", async (ctx) => {
    ctx.body = `Test ${ctx}`;
});

app.use(router.routes());


httpCallback.setHttpCallback(app.callback());
