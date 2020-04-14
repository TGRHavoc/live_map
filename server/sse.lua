clientId = 0
clients = {}

function sendUpdate()
    CreateThread(function()
        local t = 100
        print("doing while")
        while (t > 0) do
            print(t)
            ---print(dump(clients))
            for id,res in pairs(clients) do
                --print("Sending data to " .. id)
                res.write("data: test" .. t .. "\n\n")
            end
            t = t-1
            Wait(500)
        end
        
        for id,res in pairs(clients) do
            --print("Sending data to " .. id)
            res["end"]()
        end

    end)
end

SetHttpHandler(function(req, res)

    print("http handler")
    -- Restrict the origin if set, otherwise allow everyone
    res.writeHead(200, {
        ["Access-Control-Allow-Origin"] = GetConvar("livemap_access_control", "*"),
        ["Connection"] = "keep-alive",
        ["Content-Type"] = "text/event-stream",
        ["Cache-Control"] = "no-cache",
    })

    res.write("\n")
    print(clientId .. " connected")
    clients[clientId] = res
    clientId = clientId +1
end)


SetTimeout(5000, sendUpdate)
