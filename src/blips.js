const log = require("simple-console-logger").getLogger("LiveMap Blips");

const BlipController = (router) => {
    const getBlips = function (ctx) {
        log.debug("Sending blips");
        ctx.body = "Hello";
    };
    
    onNet("livemap:blipsGenerated", (blipTable) => {
        log.debug("Got blips: %O", blipTable);
    });

    RegisterCommand("blips", (src, args) => {
        if (src === 0) {
            log.warn("Please run this command in game. Make sure you have ACE permissions set up");
            log.warn("https://docs.tgrhavoc.me/livemap-resource/faq/#how-do-i-get-blips");
            return;
        }

        if (args[0] === "generate") {
            log.warn("Generating blips using the in-game natives: Player %s is generating them.");
        }
    });

    router.get("/blips", getBlips);
};


/*
RegisterCommand("blips", function(source, args, rawCommand)
    local playerId = GetPlayerIdentifier(source, 0)
    if args[1] == "generate" then
        print("Generating blips using the in-game native: Player " .. playerId .. " is generating (I hope you know them)")
        playerWhoGeneratedBlips = playerId

        TriggerClientEvent("livemap:getBlipsFromClient", source)

        return
    end

end, true)
*/

module.exports = BlipController;
