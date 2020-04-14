const PassThrough = require("stream").PassThrough;
const send = ({clientId, type, msg}) => {
    clients[clientId].res.write(`event: ${type || "none"}\ndata: ${msg}\n\n`);
    //clients[clientId].res.end();
};

const sseLog = logger.getLogger("LiveMap SSE");

var clientId = 0;
var clients = {};

module.exports = function(router){
    router.get("/sse", async (ctx) => {

        try{
            const stream = new PassThrough();

            ctx.type = "text/event-stream";
            ctx.body = stream;

            //ctx.res.statusCode = 200;
            ctx.res.setHeader("Content-Type", "text/event-stream");
            ctx.res.setHeader("Cache-Control", "no-cache, no-transform");
            ctx.res.setHeader("Connection", "keep-alive");
            ctx.res.setHeader("Encoding", "none");

            sseLog.debug("Writing newline...");
            //ctx.res.cfxRes.writeOut(":ok");
            ctx.res.write(":ok\n");
            //ctx.res.cfxRes.send();

            (function (clientId) {
                sseLog.debug("Saving client ctx");
                clients[clientId] = ctx;

                sseLog.debug("Hooking events for clean exiting");
                ["close", "finish", "error"].forEach(evt => {
                    ctx.res.on(evt, () => {
                        sseLog.debug(`${evt} for ${clientId}`);
                        ctx.res.end();
                        delete clients[clientId];
                    });
                });
                setTimeout(() => {
                    sseLog.debug("Sending test event");
                    sendEvent("test", { this: "is", some: true, data: 10 });
                }, 1000);
            })(++clientId);

            //ctx.res.end();

        }catch(e){
            sseLog.fatal(e);
        }
    });
};

module.exports.sendEvent = sendEvent = function(type, data) {
    sseLog.debug(`Clients: ${Object.keys(clients)}`);

    if (typeof(data) !== "string"){
        data = JSON.stringify(data);
    }

    for (clientId in clients){
        send({ clientId, type, msg: data });
    }
};